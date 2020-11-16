using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Syinpo.Core;
using Syinpo.Core.Domain.RestApi;
using Syinpo.Core.Helpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Serilog;

namespace Syinpo.Unity.AspNetCore.Middlewares {
    public class ExceptionMiddleware {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware( RequestDelegate next ) {
            _next = next;
        }

        public async Task Invoke( HttpContext context ) {
            try {
                await _next( context );
            }
            catch( Exception exception ) {
                try {

           


                    String message = String.Empty;
                    int code = 500;

                    var exceptionType = exception.GetType();
                    if( exceptionType == typeof( SysException ) ) {
                        var ex = exception as SysException;
                        code = ex.Code;
                        message = exception.Message;
                    }
                    else {
                        message = exception.Message;
                        Log.Error( exception.Message, exception );
                      
                    }

                    //var err = message + " " + context.Exception.StackTrace;
                    var err = message;
                    var errorsRootObject = new ErrorObject {
                        Message = err,
                        Code = code
                    };

                    var errorsJson = JsonHelper.ToJson( errorsRootObject );
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync( errorsJson );
                    return;
                }
                catch {
                    throw exception;
                }
            }
        }
    }

    public static class ExceptionMiddlewareExtensions {
        public static IApplicationBuilder UseException( this IApplicationBuilder app ) {
            return app.UseMiddleware<ExceptionMiddleware>();
        }
    }
}
