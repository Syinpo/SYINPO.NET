using System;
using System.Collections.Generic;
using System.Text;

namespace Syinpo.Core.SignalBus.Model {
    public class UserSubscription {
        /// <summary>
        /// 连接Id
        /// </summary>
        public string ConnectionId {
            get; set;
        }

        /// <summary>
        /// 主题
        /// </summary>
        public string Topic {
            get; set;
        }

        /// <summary>
        /// 订阅时间
        /// </summary>
        public DateTime TopicTime {
            get; set;
        }

        /// <summary>
        /// Ip地址
        /// </summary>
        public string IpAddress {
            get; set;
        }

        /// <summary>
        /// 来自服务器
        /// </summary>
        public string FromServerName {
            get; set;
        }


    }
}
