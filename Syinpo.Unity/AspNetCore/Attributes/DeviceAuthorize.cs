using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using Syinpo.BusinessLogic.Safety;
using Syinpo.Core;
using Syinpo.Core.Domain.RestApi;
using Syinpo.Core.Helpers;
using Syinpo.Model.Request;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;

namespace Syinpo.Unity.AspNetCore.Attributes {
    public class DeviceAuthorize : TypeFilterAttribute {

        public DeviceAuthorize() : base( typeof( DeviceTokenFilter ) ) { }

        private class DeviceTokenFilter : IAuthorizationFilter {
            private readonly IHttpContextAccessor _httpContextAccessor;
            private readonly ITokenService _tokenService;

            public DeviceTokenFilter( IHttpContextAccessor httpContextAccessor, ITokenService tokenService ) {
                _httpContextAccessor = httpContextAccessor;
                _tokenService = tokenService;
            }

            public void OnAuthorization( AuthorizationFilterContext filterContext ) {
                if( filterContext == null )
                    throw new ArgumentNullException( nameof( filterContext ) );

                if( filterContext.HttpContext.Request == null )
                    return;

                var actionFilter = filterContext.ActionDescriptor.FilterDescriptors
                    .Where( filterDescriptor => filterDescriptor.Scope == FilterScope.Action || filterDescriptor.Scope == FilterScope.Controller )
                    .Select( filterDescriptor => filterDescriptor.Filter )
                    .OfType<DeviceAuthorize>()
                    .LastOrDefault();

                if( actionFilter != null ) {
                    try {

                        string token = filterContext.HttpContext.Request.Headers
                            .FirstOrDefault( f => f.Key == "Authorization" )
                            .Value
                            .ToString()
                            .Replace( "Bearer", "" )
                            .Trim();
                        ;

                        if (string.IsNullOrWhiteSpace(token))
                        {
                            throw new SysException( 401, "token验证" );
                        }

                        var userPrincipal = _tokenService.GetPrincipalFromExpiredToken( token, out bool expired );

                        if( userPrincipal == null ) {
                            throw new SysException( 401, "token验证" );
                        }
                        if( expired ) {
                            throw new SysException( 403, "token过期" );
                        }


                        Thread.CurrentPrincipal = userPrincipal;
                        _httpContextAccessor.HttpContext.User = userPrincipal;

                    }
                    catch( Exception ex ) {
                        var errorObject = new ErrorObject {
                            Code = 500,
                            Message = ex.Message
                        };

                        if( ex is SysException ) {

                            errorObject.Code = ( ex as SysException ).Code;
                        }

                        //var errorsJson = JsonHelper.ToJson2( errorObject );
                        filterContext.Result = new JsonResult( errorObject  );
                    }
                }
            }
        }

    }
}

