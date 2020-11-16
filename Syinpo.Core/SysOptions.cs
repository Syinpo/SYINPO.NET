using System;
using System.Collections.Generic;
using System.Text;
using Syinpo.Core.SignalBus.Model;

namespace Syinpo.Core {
    public class SysOptions
    {
        public string HuomaDomain { get; set; }

        public string LocalDomain { get; set; }

        public string SysName { get; set; }

        public bool UseFirewall {
            get;set;
        }

        public SignalrOptions SignalrBus {
            get;set;
        }
    }
}
