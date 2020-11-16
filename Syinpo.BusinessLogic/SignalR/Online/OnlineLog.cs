using System;
using System.Collections.Generic;
using System.Text;

namespace Syinpo.BusinessLogic.SignalR.Online {
   public class OnlineLog {
        /// <summary>
        /// 设备Id
        /// </summary>
        public int DeviceId {
            get; set;
        }

        /// <summary>
        /// 客户端的连接建立时间
        /// </summary>
        public DateTime ConnectTime {
            get; set;
        }

        // 是否在线
        public bool IsOnline {
            get;set;
        }
    }
}
