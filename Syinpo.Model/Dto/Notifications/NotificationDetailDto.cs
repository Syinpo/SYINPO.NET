using System;
using System.Collections.Generic;
using System.Text;

namespace Syinpo.Model.Dto.Notifications {
    public class NotificationDetailDto : BaseDto {

        ///<summary>
        /// 通知方案主键
        ///</summary>
        public int? NotificationSchemeId {
            get; set;
        } // NotificationSchemeId

        ///<summary>
        /// 通知类型名称
        ///</summary>
        public string NotificationName {
            get; set;
        } // NotificationName (length: 50)

        ///<summary>
        /// 通知数据，一般是Json字符串
        ///</summary>
        public string Data {
            get; set;
        } // Data

        ///<summary>
        /// 严重性
        ///</summary>
        public int Severity {
            get; set;
        } // Severity

        ///<summary>
        /// 实体类型名称(如果这是实体级别通知)
        ///</summary>
        public string EntityTypeName {
            get; set;
        } // EntityTypeName (length: 50)

        ///<summary>
        /// 实体主键
        ///</summary>
        public int EntityId {
            get; set;
        } // EntityId

        ///<summary>
        /// 是否删除
        ///</summary>
        public bool Deleted {
            get; set;
        } // Deleted

        ///<summary>
        /// 过期时间
        ///</summary>
        public System.DateTime? ExpireTime {
            get; set;
        } // ExpireTime

        ///<summary>
        /// 创建时间
        ///</summary>
        public System.DateTime CreateTime {
            get; set;
        } // CreateTime
    }
}
