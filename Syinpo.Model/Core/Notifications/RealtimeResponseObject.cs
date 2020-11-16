namespace Syinpo.Model.Core.Notifications {
    /// <summary>
    /// SignalR响应类型
    /// </summary>
    public class RealtimeResponseObject {
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool Success {
            get;
            set;
        }

        /// <summary>
        /// 消息内容
        /// </summary>
        public string Message {
            get; set;
        }

        /// <summary>
        /// 数据
        /// </summary>
        public object Data {
            get;set;
        }
    }

    /// <summary>
    /// SignalR响应泛类型
    /// </summary>
    /// <typeparam name="T">泛对象类型</typeparam>
    public class RealtimeResponseObject<T> {
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool Success {
            get;
            set;
        }

        /// <summary>
        /// 消息内容
        /// </summary>
        public string Message {
            get; set;
        }

        /// <summary>
        /// 数据
        /// </summary>
        public T Data {
            get; set;
        }
    }
}
