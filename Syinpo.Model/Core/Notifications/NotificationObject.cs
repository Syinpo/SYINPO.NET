using System;
using Syinpo.Core.Helpers;

namespace Syinpo.Model.Core.Notifications {
    /// <summary>
    /// 通知实体
    /// </summary>
    public class NotificationObject<T> {
        /// <summary>
        /// 头部信息
        /// </summary>
        public NotificationHead Header {
            get; set;
        }

        /// <summary>
        /// 实体信息
        /// </summary>
        public T Body {
            get;
            set;
        }
    }

    /// <summary>
    /// 通知实体
    /// </summary>
    public class NotificationObject {
        /// <summary>
        /// 头部信息
        /// </summary>
        public NotificationHead Header {
            get; set;
        }

        /// <summary>
        /// 实体信息
        /// </summary>
        public object Body {
            get;
            set;
        }
    }

    /// <summary>
    /// 通知头部对象
    /// </summary>
    public class NotificationHead {
        /// <summary>
        /// 时间戳
        /// </summary>
        public long Timestamp {get;set;} = DateTimeHelper.ConvertDataTimeToLong( DateTime.Now );

        /// <summary>
        /// 业务分类
        /// </summary>
        public string Category
        {
            get;
            set;
        }


        /// <summary>
        /// 通知类型
        /// </summary>
        public string NotifyType
        {
            get;set;
        }

        /// <summary>
        /// 是否同步状态
        /// </summary>
        public bool SyncState {
            get;set;
        }

        /// <summary>
        /// 消息Id
        /// </summary>
        public string SpanId { get; set; } = Guid.NewGuid().ToString();
    }
}
