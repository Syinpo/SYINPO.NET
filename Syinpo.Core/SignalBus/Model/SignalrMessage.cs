using System;
using System.Collections.Generic;
using System.Text;

namespace Syinpo.Core.SignalBus.Model {
    public class SignalrMessage {
        public string JsonData {
            get;set;
        }

        public string Type {
            get;set;
        }

        public SignalrMessageTypeEnum SmgType {
            get;set;
        }
    }
}
