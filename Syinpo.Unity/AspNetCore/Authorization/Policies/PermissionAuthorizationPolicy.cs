using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using Syinpo.Core;
using Syinpo.Unity.AspNetCore.Attributes;
using Syinpo.Unity.AspNetCore.Authorization.Requirements;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Syinpo.Unity.AspNetCore.Authorization.Policies {
    /// <summary>
    /// https://stackoverflow.com/questions/31464359/how-do-you-create-a-custom-authorizeattribute-in-asp-net-core
    /// </summary>
    public class PermissionAuthorizationPolicy : AttributeAuthorizationHandler<PermissionRequirement, ApiAuthorize> {
        protected override Task HandleRequirementAsync( AuthorizationHandlerContext context, PermissionRequirement requirement,
            IEnumerable<ApiAuthorize> attributes ) {
            if( context == null )
                throw new ArgumentNullException( nameof( context ) );
            if( requirement == null )
                throw new ArgumentNullException( nameof( requirement ) );

            var supper = context.User.Claims.FirstOrDefault( t => t.Value == "true" && t.Type == "supper" );
            string permissions = string.Join( ',', attributes
                .Where( w => !string.IsNullOrWhiteSpace( w.Permission ) )
                .Select( s => s.Permission )
                .ToArray() );
            string[] strArray1;
            if( string.IsNullOrEmpty( permissions ) )
                strArray1 = (string[])null;
            else
                strArray1 = permissions.Split( ',' );
            string[] strArray2 = strArray1;
            if( supper == null && strArray2 != null && ( (IEnumerable<string>)strArray2 ).Any<string>() ) {
                var flag = false;

                foreach( var item in strArray2 ) {
                    if( !string.IsNullOrWhiteSpace( item ) ) {
                        var adminClaim = context.User.Claims.FirstOrDefault( t => t.Value == item.Trim() && t.Type == "permission" );
                        if( adminClaim != null ) {
                            flag = true;
                            break;
                        }

                    }
                }

                if( flag ) {
                    context.Succeed( requirement );
                }
                else
                {
                    throw new SysException(1000, "权限验证不通过");
                    context.Fail();
                }
            }
            else {
                context.Succeed( requirement );
            }


            return Task.CompletedTask;
        }
    }


}
