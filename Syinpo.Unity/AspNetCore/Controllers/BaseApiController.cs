using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using FluentValidation.Results;
using Syinpo.Core;
using Syinpo.Core.Domain.RestApi;
using Syinpo.Core.Helpers;
using Syinpo.Core.Monitor;
using Syinpo.Core.Monitor.ClrModule;
using Syinpo.Core.Monitor.PackModule;
using Syinpo.Core.Reflection;
using Syinpo.Unity.AspNetCore.Attributes;
using Syinpo.Unity.AspNetCore.Filters;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Syinpo.Unity.AspNetCore.Controllers {
    [EnableCors( "AllowAllPolicy" )]
    [ValidateFilter]
    [ApiController]
    [Route( "api/[controller]" )]
    public class BaseApiController : ControllerBase {
        protected async Task<string> GetRequestBody() {
            /* 旧版
            //using Microsoft.AspNetCore.Http.Internal;
            //Request.EnableRewind();
            using( var reader = new StreamReader( Request.Body ) ) {
                var body = reader.ReadToEnd();

                // Do something

                Request.Body.Seek( 0, SeekOrigin.Begin );

                body = reader.ReadToEnd();

                return body;
            }
            */

            string body = null;
            Request.EnableBuffering();
            if( Request.ContentLength.HasValue && Request.ContentLength.Value > 0 && Request.Body.CanSeek && Request.Body.CanRead ) {
                Request.Body.Seek( 0, SeekOrigin.Begin );
                using( var reader = new StreamReader(
                    Request.Body, encoding: Encoding.UTF8,
                    detectEncodingFromByteOrderMarks: false,
                    bufferSize: 1024,
                    leaveOpen: true ) ) {
                    body = await reader.ReadToEndAsync();
                    Request.Body.Position = 0;
                }
            }

            return body;
        }


        protected IActionResult Error( int code = 500, string propertyKey = "exception", string errorMessage = "" ) {
            var errors = new Dictionary<string, List<string>>();

            if( !string.IsNullOrEmpty( errorMessage ) && !string.IsNullOrEmpty( propertyKey ) ) {
                var errorsList = new List<string>() { errorMessage };
                errors.Add( propertyKey, errorsList );
            }

            foreach( var item in ModelState ) {
                var errorMessages = item.Value.Errors.Select( x => x.ErrorMessage );

                var validErrorMessages = new List<string>();

                validErrorMessages.AddRange( errorMessages.Where( message => !string.IsNullOrEmpty( message ) ) );

                if( validErrorMessages.Count > 0 ) {
                    if( errors.ContainsKey( item.Key ) ) {
                        errors[ item.Key ].AddRange( validErrorMessages );
                    }
                    else {
                        errors.Add( item.Key, validErrorMessages.ToList() );
                    }
                }
            }


            StringBuilder builder = new StringBuilder();
            foreach( KeyValuePair<string, List<string>> pair in errors ) {
                builder.Append( pair.Key ).Append( ":" ).Append( string.Join( "|", pair.Value ) ).Append( ',' );
            }
            string errorResult = builder.ToString();
            // Remove the final delimiter
            errorResult = errorResult.TrimEnd( ',' );

            var errorsRootObject = new ErrorObject {
                Message = errorResult,
                Code = code
            };

            //var errorsJson = JsonHelper.ToJson( errorsRootObject );
            return new JsonResult( errorsRootObject );
        }

        protected IActionResult Success( object data = null, string message = "ok" ) {
            //if( data != null ) {
            //    Process( data );
            //}

            var successRootObject = new SuccessObject {
                Message = message,
                Code = 0,
                Data = data
            };


            //var json = JsonHelper.ToJson( successRootObject );

            //return new RawJsonActionResult( json );

            return new JsonResult( successRootObject );
        }

        protected IActionResult Redirect( string location ) {
            var successRootObject = new {
                Redirect = true,
                Location = location
            };


            //var json = JsonHelper.ToJson( successRootObject );

            //return new RawJsonActionResult( json );

            return new JsonResult( successRootObject );
        }

        protected IActionResult Error_Bus( int code = 500, string propertyKey = "exception", string errorMessage = "" ) {
            var errors = new Dictionary<string, List<string>>();

            if( !string.IsNullOrEmpty( errorMessage ) && !string.IsNullOrEmpty( propertyKey ) ) {
                var errorsList = new List<string>() { errorMessage };
                errors.Add( propertyKey, errorsList );
            }

            foreach( var item in ModelState ) {
                var errorMessages = item.Value.Errors.Select( x => x.ErrorMessage );

                var validErrorMessages = new List<string>();

                validErrorMessages.AddRange( errorMessages.Where( message => !string.IsNullOrEmpty( message ) ) );

                if( validErrorMessages.Count > 0 ) {
                    if( errors.ContainsKey( item.Key ) ) {
                        errors[ item.Key ].AddRange( validErrorMessages );
                    }
                    else {
                        errors.Add( item.Key, validErrorMessages.ToList() );
                    }
                }
            }


            StringBuilder builder = new StringBuilder();
            foreach( KeyValuePair<string, List<string>> pair in errors ) {
                builder.Append( pair.Key ).Append( ":" ).Append( string.Join( "|", pair.Value ) ).Append( ',' );
            }
            string errorResult = builder.ToString();
            // Remove the final delimiter
            errorResult = errorResult.TrimEnd( ',' );

            var errorsRootObject = new ErrorObject {
                Message = errorResult,
                Code = code
            };

            var errorsJson = JsonHelper.ToJson( errorsRootObject );

            var group = DateTime.Now.ToString( "yyyyMMddHHmm" );
            IoC.Resolve<IPackStore<TimeData>>().AddQueue( new TimeData( group, MonitorContextKeys.fault, 1 ) );

            throw new Exception( errorsJson );
        }

        protected string GetErrors( ValidationResult validationResult ) {
            var errors = new List<string>();

            if( !validationResult.IsValid )
                errors.AddRange( validationResult.Errors.Select( error => error.ErrorMessage ) );

            return string.Join( "|", errors );
        }


        public static void Process( object target ) {
            if( target == null )
                return;

            var properties = target.GetType()
                .GetProperties( BindingFlags.Instance | BindingFlags.Public )
                .Where( x =>
                {
                    if( x.CanWrite && x.GetSetMethod( false ) != null )
                        return x.GetSetMethod().GetParameters().Length == 1;
                    return false;
                } );

            foreach( var property in properties ) {
                if( property.PropertyType == typeof( string ) ) {
                    var value = property.GetValue( target ) as string;
                    if( value != null ) {
                        var trim = value.Trim();
                        property.SetValue( target, trim );
                    }
                    else {
                        property.SetValue( target, "" );
                    }
                }

                if(  property.PropertyType.GetInterfaces().Contains( typeof( IEnumerable ) )) {
                    var array = ( property.GetValue( target ) as IEnumerable );
                    if( array != null ) {
                        foreach( var item in array ) {
                            Process( item );
                        }
                    }
                    else {
                        property.SetValue( target, ReflectionUtils.CreateInstanceFromType( property.PropertyType ) );
                    }
                }
                else {
                    Process( property.GetValue( target ) );
                }
            }
        }

    }
}