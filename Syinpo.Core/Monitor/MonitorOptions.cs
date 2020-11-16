using System;
using System.Collections.Generic;
using System.Text;

namespace Syinpo.Core.Monitor {
    public class MonitorOptions {
        public bool UseMonitor { get; set; } = false;

        public string ServerAddress {get;set;} = "http://localhost:5004/";

        public string DefaultDataSet { get; set; } = "DataSet";

        public string RequestStorePath { get; set; } = "~/App_Data/monitor/request/";
        public string ResponseStorePath { get; set; } = "~/App_Data/monitor/response/";

        public string TimeDataStorePath { get; set; } = "~/App_Data/monitor/time/";

        public string FindRequestLogPath { get; set; } =
            "C:\\融智融力\\svn\\Syinpo\\Server\\trunk\\Syinpo\\Syinpo.Admin.Api\\App_Data\\monitor\\request\\";
        public string FindResponseLogPath {
            get; set;
        } =
            "C:\\融智融力\\svn\\Syinpo\\Server\\trunk\\Syinpo\\Syinpo.Admin.Api\\App_Data\\monitor\\response\\";

        public string FindTimeDataLogPath {
            get; set;
        } =
            "C:\\融智融力\\svn\\Syinpo\\Server\\trunk\\Syinpo\\Syinpo.Admin.Api\\App_Data\\monitor\\time\\";

        public int BatchSize { get; set; } = 100;

        public int BatchInterval { get; set; } = 10000;
    }
}
