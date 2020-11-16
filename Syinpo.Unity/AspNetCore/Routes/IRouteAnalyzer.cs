using System;
using System.Collections.Generic;
using System.Text;

namespace Syinpo.Unity.AspNetCore.Routes {
    public interface IRouteAnalyzer {
        IEnumerable<RouteInformation> GetAllRouteInformations();
    }
}
