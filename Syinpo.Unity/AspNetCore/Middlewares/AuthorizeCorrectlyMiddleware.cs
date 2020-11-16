using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Syinpo.Core.Domain.RestApi;
using Syinpo.Core.Helpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;

namespace Syinpo.Unity.AspNetCore.Middlewares {
    /// <summary>
    /// https://stackoverflow.com/questions/35656828/return-http-403-using-authorize-attribute-in-asp-net-core
    /// </summary>
    public class AuthorizeCorrectlyMiddleware {
        readonly RequestDelegate next;

        public AuthorizeCorrectlyMiddleware( RequestDelegate next ) {
            this.next = next;
        }

        public async Task Invoke( HttpContext context ) {
            await next( context );

            if( context.Response.StatusCode != (int)HttpStatusCode.OK ) {

                String message = String.Empty;
                int code = 500;

                if( context.Response.StatusCode == 401 )
                {
                    code = 401;
                    message = "token为空或过期";
                }
                else if( context.Response.StatusCode == 403 ) {
                    code = 403;
                    message = "bearer验证不通过";
                }
                else {
                    code = context.Response.StatusCode;
                    message = $"StatusCode:{context.Response.StatusCode} error.";
                }

                var errorsRootObject = new ErrorObject {
                    Message = message,
                    Code = code
                };


                context.Response.Clear();

                var errorsJson = JsonHelper.ToJson2( errorsRootObject );
                await context.Response.WriteAsync( errorsJson, Encoding.UTF8 );

                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.OK;
            }
        }
    }

    public static class AuthorizeCorrectlyMiddlewareExtensions {
        public static IApplicationBuilder UseAuthorizeCorrectly( this IApplicationBuilder app ) {
            return app.UseMiddleware<AuthorizeCorrectlyMiddleware>();
        }
    }
}
