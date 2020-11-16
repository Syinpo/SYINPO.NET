using System;
using System.Net;
using Syinpo.Core;
using Syinpo.Core.Domain.RestApi;
using Syinpo.Core.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace Syinpo.Unity.AspNetCore.Filters {

    public class CustomExceptionFilter : IExceptionFilter {
        private ILogger<CustomExceptionFilter> _Logger;

        public CustomExceptionFilter( ILogger<CustomExceptionFilter> logger ) {
            _Logger = logger;
        }


        public void OnException( ExceptionContext context ) {
         
            HttpStatusCode status = HttpStatusCode.InternalServerError;
            String message = String.Empty;
            int code = 500;

            var exceptionType = context.Exception.GetType();
            if( exceptionType == typeof( UnauthorizedAccessException ) ) {
                message = "Unauthorized Access";
                status = HttpStatusCode.Unauthorized;

                _Logger.LogWarning( "Unauthorized Access in Controller Filter." );
            }
            else if( exceptionType == typeof( NotImplementedException ) ) {
                message = "A server error occurred.";
                status = HttpStatusCode.NotImplemented;

                _Logger.LogWarning( "A server error occurred in Controller Filter." );
            }
            else if( exceptionType == typeof( SysException ) ) {
                var ex = context.Exception as SysException;
                code = ex.Code;
                message = context.Exception.Message;
                status = HttpStatusCode.InternalServerError;

                _Logger.LogWarning( $"SysException thrown error: {ex.Message}", ex );
            }
            else {
                message = context.Exception.Message;
                status = HttpStatusCode.InternalServerError;

                _Logger.LogError( new EventId( 0 ), context.Exception, message );
            }
            context.ExceptionHandled = true;

            //var err = message + " " + context.Exception.StackTrace;
            var err = message;
            var errorsRootObject = new ErrorObject {
                Message = err,
                Code = code
            };

            //var errorsJson = JsonHelper.ToJson( errorsRootObject );
            context.Result = new JsonResult( errorsRootObject  );
        }
    }
}
