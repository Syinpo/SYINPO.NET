using System;
using System.Collections.Generic;
using System.Text;

namespace Syinpo.Core.Monitor {
    public class MonitorContextKeys {
        public static readonly string http = "http";

        public static readonly string signalr = "signalr";

        public static readonly string signalr_connect = "signalr_connect";

        public static readonly string signalr_disconnect = "signalr_disconnect";

        public static readonly string online_device = "online_device";

        public static readonly string cpu_app = "cpu_app";

        public static readonly string cpu_server = "cpu_server";

        public static readonly string ram_app_size = "ram_app_size";

        public static readonly string ram_server = "ram_server";

        public static readonly string fault = "fault";

    }
}
