using System;
using System.Collections.Generic;
using System.Text;

namespace Syinpo.Model.Request.DeviceSm {
    public class DeviceSmsListParametersModel 
    {
        /// <summary>
        /// 设备Id
        /// </summary>
        public int? DeviceId { get; set; }

        /// <summary>
        /// 关键字(联系人手机号)
        /// </summary>
        public string Keyword {
            get; set;
        }

        /// <summary>
        /// 空值或者不传查全部
        /// 0-查接收
        /// 1-查发出
        /// </summary>
        public bool? Sent {
            get; set;
        }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? StartTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// 过滤敏感词 （ 0 全部，1 有敏感词，2 没有敏感词 ）
        /// </summary>
        public int FilterSensitiveWord { get; set; }
        /// <summary>
        /// 敏感词
        /// </summary>
        public string SensitiveWord { get; set; }

        /// <summary>
        /// 当前页，从1开始
        /// </summary>
        public int Page { get; set; } = 1;

        /// <summary>
        /// 分页大小
        /// </summary>
        public int PageSize { get; set; } = 20;
    }
}
