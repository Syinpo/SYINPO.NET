using System;
using System.Collections.Generic;
using System.Text;
using Syinpo.Core.Caches;
using Syinpo.Model;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Syinpo.Unity.Firewall.Rules {
    public class ForbiddenAccessRuleHandler : IFirewallRule {
        private readonly FirewallOptions _optisons;
        private ICache _cache;
        private ICurrent _current;
        private ActionExecutingContext _actionExecutingContext;

        public ForbiddenAccessRuleHandler( FirewallOptions optisons ) {
            _optisons = optisons;
        }


        public bool IsAllowed() {
            return false;
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
