using Syinpo.Core.Caches;
using Syinpo.Model;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Syinpo.Unity.Firewall {
    public interface IFirewallRule {
        bool IsAllowed();

        void Init( ActionExecutingContext actionExecutingContext, ICache cache, ICurrent current);

        object GetOption();
    }
}
