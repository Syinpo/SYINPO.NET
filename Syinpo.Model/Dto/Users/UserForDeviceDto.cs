using System;
using System.Collections.Generic;
using System.Text;

namespace Syinpo.Model.Dto.Users {
    /// <summary>
    /// 移动端检测扫码绑定的用户结果
    /// </summary>
  public  class UserForDeviceDto {
        ///<summary>
        /// 合作伙伴主键
        ///</summary>
        public int PartnerId {
            get; set;
        } // PartnerId

        ///<summary>
        /// 用户名
        ///</summary>
        public string Username {
            get; set;
        } // Username (length: 1000)

        ///<summary>
        /// 显示名称
        ///</summary>
        public string DisplayName {
            get; set;
        } // DisplayName (length: 1000)

        ///<summary>
        /// 手机号码
        ///</summary>
        public string Mobile {
            get; set;
        } // Mobile (length: 1000)

        /// <summary>
        /// 已绑定设备Uuid
        /// </summary>
        public string DeviceUuid
        {
            get; set;
        }
    }
}
