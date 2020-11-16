using System;
using System.Collections.Generic;
using System.Text;

namespace Syinpo.Model.Dto.Devices
{
    public class DeviceDetailsDto
    {
        ///<summary>
        /// 所属用户Id
        ///</summary>
        public int UserId { get; set; }
        ///<summary>
        /// 所属用户名称
        ///</summary>
        public string UserName { get; set; }
        ///<summary>
        /// 设备Id
        ///</summary>
        public int DeviceId { get; set; }
        ///<summary>
        /// 设备IMIE
        ///</summary>
        public string DeviceUuid { get; set; }

        ///<summary>
        /// 手机号码
        ///</summary>
        public string Mobile { get; set; }

        ///<summary>
        /// 手机品牌
        ///</summary>
        public string Brand { get; set; }

        ///<summary>
        /// 手机型号
        ///</summary>
        public string Model { get; set; }

        ///<summary>
        /// 手机操作系统
        ///</summary>
        public string Os { get; set; }

        ///<summary>
        /// 手机操作系统版本号
        ///</summary>
        public string OsVersion { get; set; }

        /// <summary>
        /// 手机当前时间
        /// </summary>
        public DateTime SystemDate { get; set; }
        ///<summary>
        /// 手机微信版本号
        ///</summary>
        public string WeiXinVersion { get; set; }

        ///<summary>
        /// 手机助手版本号
        ///</summary>
        public string AssistantVersion { get; set; }
    }
}
