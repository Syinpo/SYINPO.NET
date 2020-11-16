using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Syinpo.Core.Caches;
using Syinpo.Core.Helpers;
using Syinpo.Model;
using Syinpo.Unity.Firewall.Store;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Syinpo.Unity.Firewall.Rules {
    public class DisableRequestRuleHandler : IFirewallRule {
        private readonly FirewallOptions _optisons;
        private ICache _cache;
        private ICurrent _current;
        private ActionExecutingContext _actionExecutingContext;

        public DisableRequestRuleHandler( FirewallOptions optisons ) {
            _optisons = optisons;
        }


        public bool IsAllowed() {
            if( _actionExecutingContext == null || _cache == null || _current == null )
                return true;

            // 获取路由
            var actionDescriptor = _actionExecutingContext.ActionDescriptor as ControllerActionDescriptor;
            var route =  actionDescriptor?.AttributeRouteInfo?.Template;

            if( string.IsNullOrEmpty( route ) )
                return true;

            route = $"/" + route;
            RouteItem routeItem = RouteMemoryStore.Get( route );

            bool isAllowed = routeItem == null || !routeItem.IsDisable;
            return isAllowed;
        }

        public void Init( ActionExecutingContext actionExecutingContext, ICache cache, ICurrent current ) {
            _actionExecutingContext = actionExecutingContext;
            _cache = cache;
            _current = current;
        }

        public object GetOption() {
            return _optisons;
        }
    }
}
