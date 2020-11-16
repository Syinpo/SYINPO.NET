using System;
using System.Collections.Generic;
using System.Text;

namespace Syinpo.Core.Data {
    public class SqlBusOptions {
        public bool UseReadWriteSpit { get; set; } = false;


        public bool UseSignalrRedis { get; set; } = false;


        public bool UseEfSecondLevelCache { get; set; } = false;
    }
}
