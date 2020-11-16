using System;
using System.Collections.Generic;
using System.Text;

namespace Syinpo.Core.Domain.MonitorPoco {
    public class RequestLog {
        public int Id {
            get; set;
        }

        // TraceId
        public string TraceId {
            get; set;
        }

        //客户端IP
        public string RemoteIpAddress {
            get; set;
        }
        public int RemotePort {
            get; set;
        }

        //User.Identity
        public bool IdentityIsAuthenticated {
            get; set;
        }
        public string IdentityName {
            get; set;
        }

        //Request
        public string RequestMethod {
            get; set;
        }
        public string RequestScheme {
            get; set;
        }
        public string RequestPath {
            get; set;
        }
        public string RequestQueryString {
            get; set;
        }
        public string RequestContentType {
            get; set;
        }
        public long? RequestContentLength {
            get; set;
        }
        public string RequestHost {
            get; set;
        }
        public string RequestHead {
            get; set;
        }
        public string RequestBody {
            get; set;
        }
        public DateTime RequestTime {
            get; set;
        }

        public int RequestType {
            get;set;
        }

        public long RequestGroup {
            get;set;
        }

        public string Operation {
            get; set;
        }

        public string ServerName {
            get;set;
        }

        public DateTime CreateTime {
            get;set;
        }
    }
}
