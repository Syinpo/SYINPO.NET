using System;
using System.Collections.Generic;
using System.Text;

namespace Syinpo.Model.Dto.Notifications {
    public class NotificationDeviceStoreDto : BaseDto {
        ///<summary>
        /// 唯一Id
        ///</summary>
        public string UniqueId {
            get; set;
        } // UniqueId (length: 50)

        ///<summary>
        /// 设备Id
        ///</summary>
        public int DeviceId {
            get; set;
        } // DeviceId

        ///<summary>
        /// 通知分配Id
        ///</summary>
        public int NotificationDetailId {
            get; set;
        } // NotificationDetailId

        ///<summary>
        /// 通知数据源
        ///</summary>
        public string NotificationData {
            get; set;
        } // NotificationData

        ///<summary>
        /// 当前状态
        ///</summary>
        public int State {
            get; set;
        } // State

        ///<summary>
        /// 状态扭转数据源
        ///</summary>
        public string StateData {
            get; set;
        } // StateData

        ///<summary>
        /// 是否删除
        ///</summary>
        public bool Deleted {
            get; set;
        } // Deleted

        ///<summary>
        /// 创建时间
        ///</summary>
        public System.DateTime CreateTime {
            get; set;
        } // CreateTime
    }
}
