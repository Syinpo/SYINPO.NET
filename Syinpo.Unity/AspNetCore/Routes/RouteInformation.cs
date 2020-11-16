using System;
using System.Collections.Generic;
using System.Text;

namespace Syinpo.Unity.AspNetCore.Routes {
    public class RouteInformation {
        public string Action {
            get;set;
        }

        public string Controller {
            get;set;
        }

        public string HttpMethod { get; set; } = "GET";
        public string Area { get; set; } = "";
        public string Path { get; set; } = "";
        public string Invocation { get; set; } = "";

        public override string ToString() {
            return $"RouteInformation{{Area:\"{Area}\", HttpMethod: \"{HttpMethod}\", Path:\"{Path}\", Invocation:\"{Invocation}\"}}";
        }
    }
}
