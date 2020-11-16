using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Syinpo.Core.Domain.RestApi;
using Syinpo.Core.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json.Linq;

namespace Syinpo.Unity.AspNetCore.Filters {
    /// <summary>
    /// https://www.c-sharpcorner.com/article/using-fluentvalidation-in-asp-net-core/
    /// </summary>
    public class ValidateFilter : ResultFilterAttribute {
        public bool Skip {
            get; set;
        }

        public ValidateFilter( bool skip = false ) {
            Skip = skip;
        }

        public override void OnResultExecuting( ResultExecutingContext context ) {
            base.OnResultExecuting( context );

            //model valid not pass
            if( !context.ModelState.IsValid && !Skip ) {
                var errors = new Dictionary<string, List<string>>();

                foreach( var item in context.ModelState ) {
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
                    Code = 500
                };

                //var errorsJson = JsonHelper.ToJson( errorsRootObject );
                context.Result = new JsonResult( errorsRootObject  );
            }
        }
    }
}
