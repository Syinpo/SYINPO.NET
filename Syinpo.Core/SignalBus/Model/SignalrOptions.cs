using System.Collections.Generic;

namespace Syinpo.Core.SignalBus.Model {
    public class SignalrOptions {
        public bool UserSignalrBus {
            get;set;
        }
        public int ReconnectMinTime { get; set; } = 4000;
        public int ReconnectVariance { get; set; } = 3000;
        public string HubUrl { get; set; } = "/hubs/agent";
        public List<string> DefaultTopics { get; set; } = new List<string> { };
        public int MaxWaitForConnectionId { get; set; } = 2000;
    }
}
