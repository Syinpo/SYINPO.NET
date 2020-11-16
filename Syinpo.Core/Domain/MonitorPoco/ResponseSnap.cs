using System;
using System.Collections.Generic;
using System.Text;

namespace Syinpo.Core.Domain.MonitorPoco {
    public class ResponseSnap {
        public int Id {
            get;set;
        }

        public string TraceId {
            get;set;
        }

        public int? StatusCode {
            get;set;
        }

        public bool Success {
            get;set;
        }

        public long ContentLength {
            get;set;
        }

        public string ResponseBody {
            get;set;
        }

        public long Elapsed {
            get;set;
        }

        public DateTime CreateTime {
            get;set;
        }
    }
}
