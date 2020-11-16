using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Syinpo.Core.Domain.RestApi;
using Syinpo.Core.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace Syinpo.Unity.AspNetCore.Filters {
    /// <summary>
    /// https://github.com/ignas-sakalauskas/CustomAuthorizeFilterAspNetCore20/blob/master/CustomAuthorizeFilterAspNetCore20/Authorization/CustomAuthorizeFilter.cs
    /// </summary>
    public class CustomAuthorizeFilter : IAsyncAuthorizationFilter {
        public AuthorizationPolicy Policy {
            get;
        }

        public CustomAuthorizeFilter( AuthorizationPolicy policy ) {
            Policy = policy ?? throw new ArgumentNullException( nameof( policy ) );
        }

        public async Task OnAuthorizationAsync( AuthorizationFilterContext context ) {
            if( context == null ) {
                throw new ArgumentNullException( nameof( context ) );
            }

            if( context.Filters.Any( item => item is Microsoft.AspNetCore.Mvc.Authorization.AuthorizeFilter ) ) {
                var policyEvaluator = context.HttpContext.RequestServices.GetRequiredService<IPolicyEvaluator>();
                var authenticateResult = await policyEvaluator.AuthenticateAsync( Policy, context.HttpContext );
                var authorizeResult = await policyEvaluator.AuthorizeAsync( Policy, authenticateResult, context.HttpContext, context );

                if( authorizeResult.Challenged ) {
                    var errorsRootObject = new ErrorObject {
                        Message = "token为空或过期",
                        Code = 401
                    };

                    //var errorsJson = JsonHelper.ToJson( errorsRootObject );
                    context.Result = new JsonResult( errorsRootObject  );
                }
                else if( authorizeResult.Forbidden ) {
                    var errorsRootObject = new ErrorObject {
                        Message = "bearer认证失败",
                        Code = 403
                    };

                    //var errorsJson = JsonHelper.ToJson( errorsRootObject );
                    context.Result = new JsonResult( errorsRootObject );
                }
            }
        }
    }
}
