using System;
using System.Collections.Generic;
using System.Text;

namespace Syinpo.Model.Dto.Notifications {
    public class NotificationSchemeDto  {

        ///<summary>
        /// 主键
        ///</summary>
        public int Id {
            get; set;
        } // Id (Primary key)

        ///<summary>
        /// 通知方案名称
        ///</summary>
        public string SchemeKey {
            get; set;
        } // SchemeKey (length: 500)

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
        /// 用户Id
        ///</summary>
        public int? UserId {
            get; set;
        } // UserId

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
