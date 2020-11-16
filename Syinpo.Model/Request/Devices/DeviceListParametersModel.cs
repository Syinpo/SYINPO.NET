using System;
using System.Collections.Generic;
using System.Text;

namespace Syinpo.Model.Request.Devices {

    public class DeviceListParametersModel 
    {
        /// <summary>
        /// 关键词
        /// </summary>
        public string KeyWords {
            get; set;
        }

        /// <summary>
        /// 设备组Id （如果给 -1 就查询没哟分组的设备）
        /// </summary>
        public int? DeviceGroupId {
            get; set;
        }

        /// <summary>
        /// 是否在线
        /// </summary>
        public bool? IsOnline {
            get; set;
        }

        /// <summary>
        /// 所处名单
        /// </summary>
        public int? ListType
        {
            get; set;
        }

        /// <summary>
        /// 是否坐席绑定使用
        /// </summary>
        public bool UseInPostBind { get; set; }

        /// <summary>
        /// 排除已存在绑定的岗位设备
        /// </summary>
        public bool? ExcludePostDevice {
            get;set;
        }

        /// <summary>
        /// true按照短信创建时间排序,false按照通话结束时间排序,null按照设置Id排序
        /// </summary>
        public bool? OrderSms
        {
            get; set;
        }

        /// <summary>
        /// 当前页，从1开始
        /// </summary>
        public int Page { get; set; } = 1;

        /// <summary>
        /// 分页大小
        /// </summary>
        public int PageSize { get; set; } = 20;
    }

    public class DeviceListForAdminParametersModel : DeviceListParametersModel {
        /// <summary>
        /// 合作伙伴Id
        /// </summary>
        public int? PartnerId {
            get; set;
        }
    }
}
