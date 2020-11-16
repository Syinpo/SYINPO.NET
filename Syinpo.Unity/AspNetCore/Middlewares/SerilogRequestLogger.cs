using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Http.Internal;
using Serilog;
using Serilog.Context;

namespace Syinpo.Unity.AspNetCore.Middlewares {
    public class SerilogRequestLogger {
        readonly RequestDelegate _next;

        public SerilogRequestLogger( RequestDelegate next ) {
            if( next == null )
                throw new ArgumentNullException( nameof( next ) );
            _next = next;
        }

        /// <summary>
        /// https://github.com/sulhome/log-request-response-middleware
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public async Task Invoke( HttpContext httpContext ) {
            if( httpContext == null )
                throw new ArgumentNullException( nameof( httpContext ) );

            if( httpContext.Request.Path.ToString().ToLowerInvariant().Contains( "/common/upload/form" ) ) {
                await _next( httpContext );
            }


            //else if( !httpContext.Request.Path.ToString().ToLowerInvariant().Contains( "device/wechat/import" ) ) {
            //    await _next( httpContext );
            //}

            // 排除30KB的内容
            else if( httpContext.Request.ContentLength > 1024 * 1000 ) {
                await _next( httpContext );
            }
            else {
                // Push the user name into the log context so that it is included in all log entries
                LogContext.PushProperty( "UserName", httpContext.User.Identity.Name );

                var requestBodyStream = new MemoryStream();
                var originalRequestBody = httpContext.Request.Body;
                await httpContext.Request.Body.CopyToAsync( requestBodyStream );
                requestBodyStream.Seek( 0, SeekOrigin.Begin );
                var requestBodyText = new StreamReader( requestBodyStream ).ReadToEnd();
                requestBodyStream.Seek( 0, SeekOrigin.Begin );
                httpContext.Request.Body = requestBodyStream;

                // reponse
                using( var responseBodyMemoryStream = new MemoryStream() ) {
                    var originalResponseBodyReference = httpContext.Response.Body;
                    httpContext.Response.Body = responseBodyMemoryStream;

                    await _next( httpContext );
                    httpContext.Request.Body = originalRequestBody;

                    httpContext.Response.Body.Seek( 0, SeekOrigin.Begin );
                    var responseBody = await new StreamReader( httpContext.Response.Body ).ReadToEndAsync();
                    httpContext.Response.Body.Seek( 0, SeekOrigin.Begin );

                    Log.ForContext( "RequestHeaders",
                            httpContext.Request.Headers.ToDictionary( h => h.Key, h => h.Value.ToString() ), destructureObjects: true )
                       .ForContext( "RequestBody", requestBodyText )
                       .ForContext( "ResponseBody", responseBody )
                       .Information( "HTTP流量: {RequestMethod} {RequestPath} {statusCode}", httpContext.Request.Method, httpContext.Request.Path, httpContext.Response.StatusCode );

                    await responseBodyMemoryStream.CopyToAsync( originalResponseBodyReference );
                }
            }
        }
    }

    public static class SerilogRequestLoggerMiddlewareExtensions {
        public static IApplicationBuilder UseSerilogRequestLogger( this IApplicationBuilder app ) {
            return app.UseMiddleware<SerilogRequestLogger>();
        }
    }
}
