using System;
using System.Linq;
using Syinpo.Core;
using Syinpo.Core.Caches;
using Syinpo.Core.Domain.RestApi;
using Syinpo.Model;
using Syinpo.Unity.Firewall;
using Syinpo.Unity.Firewall.Policy;
using Syinpo.Unity.Firewall.Rules;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;

namespace Syinpo.Unity.AspNetCore.Attributes {

    public class FirewallValidate : TypeFilterAttribute {

        public FirewallPolicyEnum[] Policies {
            get; set;
        }

        public FirewallValidate( params FirewallPolicyEnum[] policies ) : base( typeof( FirewallFilter ) ) {
            Policies = policies;
        }

        private class FirewallFilter : IActionFilter {
            private readonly ICache _cache;
            private readonly ICurrent _current;
            private readonly SysOptions _options;

            public FirewallFilter( ICache cache, ICurrent current, IOptions<SysOptions> options ) {
                _cache = cache;
                _current = current;
                _options = options?.Value;
            }

            public void OnActionExecuting( ActionExecutingContext actionExecutingContext ) {
                if( actionExecutingContext == null )
                    throw new ArgumentNullException( nameof( actionExecutingContext ) );

                if( actionExecutingContext.HttpContext.Request == null )
                    return;

                if(  _options?.UseFirewall == false )
                    return;

                var actionFilter = actionExecutingContext.ActionDescriptor.FilterDescriptors
                    .Where( filterDescriptor => filterDescriptor.Scope == FilterScope.Action || filterDescriptor.Scope == FilterScope.Controller )
                    .Select( filterDescriptor => filterDescriptor.Filter )
                    .OfType<FirewallValidate>()
                    .LastOrDefault();
                if( actionFilter == null )
                    return;

                try {
                    if( actionFilter.Policies == null || !actionFilter.Policies.Any() )
                        return;

                    var isAllow = true;
                    foreach( var item in actionFilter.Policies ) {
                        var firewallRule = PolicyDefaultStore.TryGet( item );
                        if( firewallRule == null )
                            continue;

                        firewallRule.Init( actionExecutingContext, _cache, _current );

                        isAllow = firewallRule.IsAllowed();
                        if( !isAllow )
                            break;
                    }

                    if( isAllow )
                        return;

                    throw new SysException( 429, "请求次数过多 或 被禁止访问" );

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
                    actionExecutingContext.Result = new JsonResult( errorObject );
                }
            }


            public void OnActionExecuted( ActionExecutedContext context ) {
            }


        }
    }
}
