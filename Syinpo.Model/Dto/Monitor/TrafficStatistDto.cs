using System;
using System.Collections.Generic;
using System.Text;

namespace Syinpo.Model.Dto.Monitor {
    public class TrafficStatistDto {
        /// <summary>
        /// 分时
        /// </summary>
        public long DateTime {
            get;set;
        }

        /// <summary>
        /// 总流量
        /// </summary>
        public long TotalReq {
            get;set;
        }

        /// <summary>
        /// http流量
        /// </summary>
        public long TotalHttpReq {
            get;set;
        }

        /// <summary>
        /// signalr流量
        /// </summary>
        public long TotalSignalrReq {
            get;set;
        }

        /// <summary>
        /// 异常流量
        /// </summary>
        public long ExceptionReq {
            get; set;
        }

    }
}
