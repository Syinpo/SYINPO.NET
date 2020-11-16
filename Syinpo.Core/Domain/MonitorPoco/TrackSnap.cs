using System;
using System.Collections.Generic;
using System.Text;

namespace Syinpo.Core.Domain.MonitorPoco {
    public class TrackSnap {
        public int Id {
            get; set;
        }

        public string TraceId {
            get; set;
        }

        public string TraceName {
            get; set;
        }
        public string TraceData {
            get; set;
        }
        public long Elapsed {
            get; set;
        }
        public DateTime CreateTime {
            get; set;
        }
    }
}
