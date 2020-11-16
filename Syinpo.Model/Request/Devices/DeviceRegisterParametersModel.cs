using System;
using System.Collections.Generic;
using System.Text;

namespace Syinpo.Model.Request.Devices {
    public class DeviceRegisterParametersModel {
        /// <summary>
        /// 序列号
        /// </summary>
        public string SerialKey {
            get;set;
        }

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName {
            get;set;
        }

        /// <summary>
        /// 设备号（资产编号）
        /// </summary>
        public string DeviceUuid {
            get;set;
        }
    }

    public class PartnerCodeParametersModel {
        /// <summary>
        /// 合作伙伴代码
        /// </summary>
        public string PartnerCode {
            get; set;
        }
    }

    public class SerialkeyLockParametersModel {
        /// <summary>
        /// 合作伙伴代码
        /// </summary>
        public string PartnerCode {
            get; set;
        }

        /// <summary>
        /// 设备号
        /// </summary>
        public string DeviceUuid {
            get; set;
        }
    }
}
