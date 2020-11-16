using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Syinpo.Unity.Firewall.Policy {
    public enum FirewallPolicyEnum:int {

        /// <summary>
        /// 禁止访问
        /// </summary>
        [Display( Name = "禁止访问" )]
        ForbiddenAccessRule = 10,


        /// <summary>
        /// 配置型的禁止请求
        /// </summary>
        [Display( Name = "配置型的禁止请求" )]
        DisableRequestRule = 20,


        /// <summary>
        /// 设备每10秒最多请求1次
        /// </summary>
        [Display( Name = "设备每10秒最多请求1次" )]
        DeviceInTimeMaxRequestRule_s10_c1 = 30,

        /// <summary>
        /// 设备每1分钟最多请求5次
        /// </summary>
        [Display( Name = "分" )]
        DeviceInTimeMaxRequestRule_m1_c5 = 40,

        /// <summary>
        /// 设备每1分钟最多请求1次
        /// </summary>
        [Display( Name = "设备每1分钟最多请求1次" )]
        DeviceInTimeMaxRequestRule_m1_c1 = 50,

        /// <summary>
        /// 设备每1分钟最多请求30次
        /// </summary>
        [Display( Name = "设备每1分钟最多请求30次" )]
        DeviceInTimeMaxRequestRule_m1_c30 = 60,

        /// <summary>
        /// 设备每5分钟最多请求1次
        /// </summary>
        [Display( Name = "设备每5分钟最多请求1次" )]
        DeviceInTimeMaxRequestRule_m5_c1 = 70,
    }
}
