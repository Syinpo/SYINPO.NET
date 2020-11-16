using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Syinpo.BusinessLogic.Safety;
using Syinpo.Core;
using Syinpo.Core.Helpers;
using Syinpo.Core.Monitor;
using Syinpo.Core.Monitor.PackModule;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;
using OpenTelemetry.Trace;

namespace Syinpo.Unity.AspNetCore.Middlewares {
    public class HttpLogMiddleware {
        private readonly IPackStore<HttpLog> _logStore;
        private readonly IPackStore<TimeData> _monitorTimeDataManager;
        private readonly ITokenService _tokenService;
        private readonly RequestDelegate _next;
        private readonly Tracer _tracer;

        public HttpLogMiddleware( RequestDelegate next, IPackStore<HttpLog> logStore, IPackStore<TimeData> monitorTimeDataManager, Tracer tracer, ITokenService tokenService ) {
            this._next = next;
            this._logStore = logStore;
            this._monitorTimeDataManager = monitorTimeDataManager;
            this._tracer = tracer;
            this._tokenService = tokenService;
        }

        public async Task Invoke( HttpContext context ) {
            var now = DateTime.Now;
            var group = now.ToString( "yyyyMMddHHmm" );
            long.TryParse( group, out long requestGroup );


            HttpLog _log = new HttpLog() {
                TraceId = _tracer.CurrentSpan.Context.TraceId.ToHexString(),
                RequestTime = now,
                RequestType = (int)RequestTypeEnum.Http,
                RequestGroup = requestGroup,
                ServerName = CommonHelper.GetServerName()
            };

            _monitorTimeDataManager.AddQueue( new TimeData( group, MonitorContextKeys.http, 1 ) );

            try {
                _tracer.CurrentSpan.SetAttribute( MonitorKeys.request_type, RequestTypeEnum.Http.ToString().ToLowerInvariant() );
                Stopwatch _stopwatch = new Stopwatch();
                _stopwatch.Reset();
                _stopwatch.Start();

                await this.SetConnectionInfo( context, _log );
                await this.SetIdentity( context, _log );
                await this.SetHttpRequest( context, _log );

                this._logStore.AddQueue(  _log );

                // response
                await this.SetHttpResponse( context, _log );

                _stopwatch.Stop();
                _tracer.CurrentSpan.SetAttribute( MonitorKeys.response_elapsed, _stopwatch.ElapsedMilliseconds );
                //_tracer.CurrentSpan.SetAttribute( MonitorKeys.response_success, true );
            }
            catch( Exception ex ) {
                _tracer.CurrentSpan.SetAttribute( MonitorKeys.response_success, false );
                _tracer.CurrentSpan.SetAttribute( MonitorKeys.request_error, ex.Source );
                _tracer.CurrentSpan.SetAttribute( MonitorKeys.request_errordetail, ex.Message );
                _monitorTimeDataManager.AddQueue( new TimeData( group, MonitorContextKeys.fault, 1 ));
                throw;
            }
        }

        private bool CaptureRequestBody( string contentType ) {
            /*
             * multipart/form-data 的内容不进行 RequestBody 记录
             */
            if( string.IsNullOrWhiteSpace( contentType ) == true )
                return true;

            if( contentType.ToLower().StartsWith( "multipart/form-data;" ) == true )
                return false;

            return true;
        }

        private async Task SetConnectionInfo( HttpContext context, HttpLog log ) {
            await Task.Run( () =>
            {
                ConnectionInfo _connection = context.Connection;

                if( _connection == null )
                    return;

                //ConnectionInfo
                if( _connection.RemoteIpAddress != null )
                    log.RemoteIpAddress = _connection.RemoteIpAddress.ToString();

                if( _connection.RemotePort != 0 )
                    log.RemotePort = _connection.RemotePort;
            } );
        }

        private async Task SetIdentity( HttpContext context, HttpLog log ) {
            await Task.Run( () =>
            {
                if( context.User == null )
                    return;

                //Identity
                log.IdentityIsAuthenticated = context.User.Identity.IsAuthenticated;
                log.IdentityName = context.User.Identity.Name;

                try {

                    if( string.IsNullOrEmpty( log.IdentityName ) && context.Request.Headers.ContainsKey( "Authorization" ) ) {
                        if( !context.Request.Headers.TryGetValue( "Authorization",
                            out Microsoft.Extensions.Primitives.StringValues auth ) )
                            return;

                        var token = auth.ToString().Replace( "Bearer", "" ).Trim();
                        var principal = _tokenService.GetPrincipalFromExpiredToken( token, out bool exp );

                        var deviceId = principal.Claims.FirstOrDefault( claim => claim.Type == "deviceId" );
                        var deviceUuid = principal.Claims.FirstOrDefault( claim => claim.Type == "deviceUuid" );
                        var username = principal.Claims.FirstOrDefault( claim => claim.Type == "username" );

                        log.IdentityIsAuthenticated = principal.Identity.IsAuthenticated && !exp;
                        log.IdentityName = username != null
                            ? username.Value
                            : ( deviceUuid != null ? deviceUuid.Value : ( deviceId != null ? deviceId.Value : "" ) );
                    }
                }
                catch( Exception ex ) {
                    ;
                }
            } );
        }

        private async Task SetHttpRequest( HttpContext context, HttpLog log ) {
            HttpRequest _request = context.Request;

            if( _request == null )
                return;

            log.RequestMethod = _request.Method;
            log.RequestScheme = _request.Scheme;
            log.RequestPath = _request.Path;
            log.RequestQueryString = _request.QueryString.ToString();

            if( _request.ContentType != null )
                log.RequestContentType = _request.ContentType;

            log.RequestContentLength = _request.ContentLength;
            log.RequestHost = _request.Host.Value;

            if( this.CaptureRequestBody( _request.ContentType ) == true ) {
                _request.EnableBuffering();
                _request.Body.Position = 0;

                log.RequestBody = await new StreamReader( _request.Body ).ReadToEndAsync();

                _request.Body.Position = 0;
            }

            log.Operation = $"Http#{context.GetRouteValue( "controller" )}#{context.GetRouteValue( "action" )}";
            if( log.Operation == "Http##" )
                log.Operation = "Http#" + log.RequestPath;

            var heads = JsonHelper.ToJson2( context.Request.Headers.ToDictionary( h => h.Key, h => h.Value.ToString() ) );
            log.RequestHead = heads;
        }

        private async Task SetHttpResponse( HttpContext context, HttpLog log ) {
            HttpResponse _response = context.Response;

            if( _response == null )
                return;

            //HttpResponse
            Stream _responseStream = _response.Body;
            MemoryStream _stream = new MemoryStream();

            _response.Body = _stream;

            await this._next( context );

            _stream.Position = 0;

            string _Body = await new StreamReader( _stream ).ReadToEndAsync();

            _tracer.CurrentSpan.SetAttribute( MonitorKeys.response_body, _Body );
            _tracer.CurrentSpan.SetAttribute( MonitorKeys.response_statuscode, _response.StatusCode );

            if( _response.StatusCode == 200 && !_Body.StartsWith( "{\"code\":" ) ) {
                _tracer.CurrentSpan.SetAttribute( MonitorKeys.response_success, true );
            }
            else if( _response.StatusCode == 200 && _Body.StartsWith( "{\"code\":" ) ) {
                if( _Body.StartsWith( "{\"code\":0" ) ) {
                    _tracer.CurrentSpan.SetAttribute( MonitorKeys.response_success, true );
                }
                else {
                    _tracer.CurrentSpan.SetAttribute( MonitorKeys.response_success, false );
                    _monitorTimeDataManager.AddQueue( new TimeData( log.RequestGroup.ToString(), MonitorContextKeys.fault, 1 ) );
                }
            }
            else {
                _tracer.CurrentSpan.SetAttribute( MonitorKeys.response_success, false );
                _monitorTimeDataManager.AddQueue( new TimeData( log.RequestGroup.ToString(), MonitorContextKeys.fault, 1 ));
            }


            _stream.Position = 0;

            await _stream.CopyToAsync( _responseStream );

            _response.Body = _responseStream;

            if( _stream != null )
                _stream.Close();
        }

    }

    public static class HttpLogMiddlewareExtensions {
        public static IApplicationBuilder UseHttpLog( this IApplicationBuilder app ) {
            return app.UseMiddleware<HttpLogMiddleware>();
        }
    }
}
