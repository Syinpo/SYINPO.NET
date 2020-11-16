namespace Syinpo.Model.Core.Notifications {
    /// <summary>
    /// 用户通知对象
    /// </summary>
    /// <typeparam name="T"></typeparam>
   public class UserNotification<T> {
        /// <summary>
        /// 用户Id
        /// </summary>
        public int UserId {
            get; set;
        }

        /// <summary>
        /// 实体对象
        /// </summary>
        public NotificationObject<T> Notification {get;set;}

    }
    /// <summary>
    /// 用户通知对象
    /// </summary>
    public class UserNotification : MediatR.INotification {
        /// <summary>
        /// 用户Id
        /// </summary>
        public int UserId {
            get; set;
        }

        /// <summary>
        /// 实体对象
        /// </summary>
        public NotificationObject Notification {
            get; set;
        }

    }
}
