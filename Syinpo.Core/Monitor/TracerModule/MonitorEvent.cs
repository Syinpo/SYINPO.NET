using System;
using System.Collections.Generic;
using System.Text;
using MessagePack;

namespace Syinpo.Core.Monitor.TracerModule {
    [MessagePackObject( keyAsPropertyName: true )]
    public class MonitorEvent {
        public double SampleRate {
            get; set;
        }

        public string DataSetName {
            get; set;
        }

        public string EventType {
            get;set;
        }

        public DateTime EventStartTime { get; set; } = DateTime.Now;

        public Dictionary<string, object> Data { get; set; } = new Dictionary<string, object>();
    }
}
