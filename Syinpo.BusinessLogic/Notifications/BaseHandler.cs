using System.Collections.Generic;
using System.Linq;
using FluentValidation.Results;
using Syinpo.Model.Core.Notifications;

namespace Syinpo.BusinessLogic.Notifications {
    public class BaseHandler {
        protected RealtimeResponseObject Error( string errorMessage = "", object errorData = null ) {
            var errorsRootObject = new RealtimeResponseObject {
                Message = errorMessage,
                Success = false,
                Data = errorData
            };

            return errorsRootObject;
        }



        protected RealtimeResponseObject Success( object data = null, string message = "ok" ) {
            var errorsRootObject = new RealtimeResponseObject {
                Message = message,
                Success = true,
                Data = data,
            };

            return errorsRootObject;
        }

        //protected RealtimeResponseObject<T> Success<T>( T data, string message = "ok" ) {
        //    var errorsRootObject = new RealtimeResponseObject<T> {
        //        Message = message,
        //        Success = true,
        //        Data = data
        //    };

        //    return errorsRootObject;
        //}
        //protected RealtimeResponseObject<T> Error<T>( string message = "error" ) {
        //    var errorsRootObject = new RealtimeResponseObject<T> {
        //        Message = message,
        //        Success = false,
        //        Data = default( T )
        //    };

        //    return errorsRootObject;
        //}

        protected string GetErrors( ValidationResult validationResult ) {
            var errors = new List<string>();

            if( !validationResult.IsValid )
                errors.AddRange( validationResult.Errors.Select( error => error.ErrorMessage ) );

            return string.Join( "|", errors );
        }
    }
}
