using System;
using MessagePack;

namespace Syinpo.Core.Monitor.PackModule {
    [MessagePackObject( keyAsPropertyName: true )]
    public class HttpLog {
        // TraceId
        public string TraceId {
            get;set;
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

        // 请求

        public int RequestType {
            get; set;
        }

        public long RequestGroup {
            get; set;
        }
        public string Operation {
            get; set;
        }

        public string ServerName {
            get; set;
        }
    }
}
