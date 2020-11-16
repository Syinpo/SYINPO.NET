namespace Syinpo.Model.Core.Notifications {
    /// <summary>
    /// 设备通知对象
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DeviceNotification<T> {
        /// <summary>
        /// 设备Id
        /// </summary>
        public int DeviceId {
            get; set;
        }

        /// <summary>
        /// 通知实体
        /// </summary>
        public NotificationObject<T> Notification {
            get; set;
        }
    }

    /// <summary>
    /// 设备通知对象
    /// </summary>
    public class DeviceNotification : MediatR.INotification {
        /// <summary>
        /// 设备Id
        /// </summary>
        public int DeviceId {
            get; set;
        }

        /// <summary>
        /// 通知实体
        /// </summary>
        public NotificationObject Notification {
            get; set;
        }
    }
}
