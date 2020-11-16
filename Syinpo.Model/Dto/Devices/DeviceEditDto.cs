using System;
using System.Collections.Generic;
using System.Text;

namespace Syinpo.Model.Dto.Devices {
    public class DeviceEditDto : BaseDto {
        /// <summary>
        /// 设备组主键
        /// </summary>
        public int? DeviceGroupId {
            get; set;
        }

        ///<summary>
        /// 设备IMIE
        ///</summary>
        public string DeviceUuid {
            get; set;
        } // DeviceUuid (length: 1000)

        ///<summary>
        /// 手机号码
        ///</summary>
        public string Mobile {
            get; set;
        } // Mobile (length: 1000)

        ///<summary>
        /// 手机品牌
        ///</summary>
        public string Brand {
            get; set;
        } // Brand (length: 1000)

        ///<summary>
        /// 手机型号
        ///</summary>
        public string Model {
            get; set;
        } // Model (length: 1000)

        ///<summary>
        /// 手机操作系统
        ///</summary>
        public string Os {
            get; set;
        } // Os (length: 1000)

        ///<summary>
        /// 手机操作系统版本号
        ///</summary>
        public string OsVersion {
            get; set;
        } // OsVersion (length: 1000)

        ///<summary>
        /// 手机微信版本号
        ///</summary>
        public string WeiXinVersion {
            get; set;
        } // WeiXinVersion (length: 1000)

        ///<summary>
        /// 手机助手版本号
        ///</summary>
        public string AssistantVersion { get; set; } // AssistantVersion (length: 1000)

        ///<summary>
        /// 经度
        ///</summary>
        public decimal? Longitude {
            get; set;
        } // Longitude

        ///<summary>
        /// 纬度
        ///</summary>
        public decimal? Latitude {
            get; set;
        } // Latitude

        ///<summary>
        /// 备注
        ///</summary>
        public string Memo {
            get; set;
        } // string

        ///<summary>
        /// 批准
        ///</summary>
        public bool Approved {
            get; set;
        } // Approved
    }

    public class DeviceForAdminEditDto : DeviceEditDto {
        /// <summary>
        /// 合作伙伴Id
        /// </summary>
        public int PartnerId {
            get; set;
        }
    }
}
