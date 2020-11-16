using System;
using System.Collections.Generic;
using System.Text;

namespace Syinpo.Core.Domain.MonitorPoco {
    public class TrafficStatist {
        /// <summary>
        /// 主键
        /// </summary>
        public int Id {
            get;set;
        }

        /// <summary>
        /// 分时
        /// </summary>
        public long DateTime {
            get; set;
        }

        /// <summary>
        /// 总流量
        /// </summary>
        public long TotalReq {
            get; set;
        }

        /// <summary>
        /// http流量
        /// </summary>
        public long TotalHttpReq {
            get; set;
        }

        /// <summary>
        /// signalr流量
        /// </summary>
        public long TotalSignalrReq {
            get; set;
        }

        /// <summary>
        /// 异常流量
        /// </summary>
        public long ExceptionReq {
            get;set;
        }


        /// <summary>
        /// signalr连接流量
        /// </summary>
        public long TotalSignalrConnectReq {
            get; set;
        }


        /// <summary>
        /// signalr断开流量
        /// </summary>
        public long TotalSignalrDisconnectReq {
            get; set;
        }

        /// <summary>
        /// 在线设备数
        /// </summary>
        public long TotalOnlineDevice {
            get;set;
        }

        /// <summary>
        /// 服务器平均CPU比率
        /// </summary>
        public decimal ServerCpuAvgRate {
            get;
            set;
        }

        /// <summary>
        /// 服务器最大CPU比率
        /// </summary>
        public decimal ServerCpuTopRate {
            get;
            set;
        }

        /// <summary>
        /// 服务器平均内存比率
        /// </summary>
        public decimal ServerRamAvgRate {
            get;
            set;
        }

        /// <summary>
        /// 服务器最大内存比率
        /// </summary>
        public decimal ServerRamTopRate {
            get;
            set;
        }

        /// <summary>
        /// 程序平均CPU比率
        /// </summary>
        public decimal AppliationCpuAvgRate {
            get;
            set;
        }

        /// <summary>
        /// 程序最大CPU比率
        /// </summary>
        public decimal AppliationCpuTopRate {
            get;
            set;
        }

        /// <summary>
        /// 程序平均内存比率
        /// </summary>
        public decimal AppliationRamAvgRate {
            get;
            set;
        }

        /// <summary>
        /// 程序最大内存比率
        /// </summary>
        public decimal AppliationRamTopRate {
            get;
            set;
        }

        /// <summary>
        /// 程序平均占用内存大小
        /// </summary>
        public decimal AppliationRamAvgSize {
            get;
            set;
        }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime {
            get;set;
        }
    }
}
