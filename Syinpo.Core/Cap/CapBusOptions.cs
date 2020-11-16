using System;
using System.Collections.Generic;
using System.Text;

namespace Syinpo.Core.Cap {
    public class CapBusOptions {
        public string VersionName {get;set;} = "v1";

        public string RabbitMQConnection { get; set; } = "localhost";
    }
}
