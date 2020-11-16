using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation.Attributes;
using Syinpo.Model.Validators.Devices;

namespace Syinpo.Model.Dto.Devices {
    /// <summary>
    /// 设备短信对象
    /// </summary>
    public class DeviceSmsForDeviceDto {
        /// <summary>
        /// 联系人唯一Id
        /// </summary>
        public string UniqueId {
            get; set;
        }

        ///<summary>
        /// 从哪个号码
        ///</summary>
        public string FromPhone {
            get; set;
        } // FromPhone (length: 50)

        ///<summary>
        /// 到哪个号码
        ///</summary>
        public string ToPhone {
            get; set;
        } // ToPhone (length: 50)

        ///<summary>
        /// 是否发送，否则接收
        ///</summary>
        public bool Sent {
            get; set;
        } // Sent

        ///<summary>
        /// 短信内容
        ///</summary>
        public string Content {
            get; set;
        } // Content

        ///<summary>
        /// 设备短信收到时间
        ///</summary>
        public long SmsReceiveTime {
            get; set;
        } // SmsReceiveTime

        ///<summary>
        /// 设备短信创建时间
        ///</summary>
        public long SmsCreateTime {
            get; set;
        } // SmsCreateTime
    }

    public class DeviceSmsForDeviceBulkDto
    {
        public List<DeviceSmsForDeviceDto> deviceSmsForDevice { get; set; }
    }
}
