using System;
using System.Collections.Generic;
using System.Text;

namespace Syinpo.Model.Core.Notifications {
    public class SimplePushMessage {
        /// <summary>
        /// 通知名称
        /// </summary>
        public string NotificationName {
            get; set;
        }

        /// <summary>
        /// 通知数据
        /// </summary>
        public object Data {
            get; set;
        }

        /// <summary>
        /// 通知对象
        /// </summary>
        public string EntityName {
            get; set;
        }

        /// <summary>
        /// 对象主键
        /// </summary>
        public int EntityId {
            get; set;
        }

        /// <summary>
        /// 过期时间
        /// </summary>
        public DateTime? ExpireTime {
            get; set;
        }

        /// <summary>
        /// 是否需要设备端回传处理结果
        /// </summary>
        public bool SyncState {
            get; set;
        }

        /// <summary>
        /// 跟踪标识
        /// </summary>
        public string SpanId {
            get;set;
        }
    }
}
