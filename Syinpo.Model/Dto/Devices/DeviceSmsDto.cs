using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation.Attributes;
using Syinpo.Model.Validators.Devices;

namespace Syinpo.Model.Dto.Devices {

    [Validator( typeof( DeviceSmsDtoValidator ) )]
    public class DeviceSmsDto : BaseDto {

        /////<summary>
        ///// 主键
        /////</summary>
        //public int Id {
        //    get; set;
        //} // Id (Primary key)

        ///<summary>
        /// 设备主键
        ///</summary>
        public int DeviceId {
            get; set;
        } // DeviceId

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
        /// 是否发送，否则接受
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
        /// 短信接收时间
        ///</summary>
        public System.DateTime SmsReceiveTime {
            get; set;
        } // SmsTime

        /// <summary>
        /// 短信发送时间
        /// </summary>
        public DateTime SmsCreateTime {
            get;set;
        }

        ///<summary>
        /// 系统创建时间
        ///</summary>
        public System.DateTime CreateTime {
            get; set;
        } // CreateTime

        /// <summary>
        /// 敏感词
        /// </summary>
        public string SensitiveWord { get; set; }
    }
}
