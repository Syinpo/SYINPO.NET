using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation.Attributes;
using Syinpo.Model.Validators.Devices;

namespace Syinpo.Model.Dto.Devices {
    [Validator( typeof( DeviceDtoValidator ) )]
    public class DeviceDto : BaseDto {

        ///<summary>
        /// 合作伙伴主键
        ///</summary>
        public int PartnerId {
            get; set;
        } // PartnerId

        /// <summary>
        /// 组主键
        /// </summary>
        public int? DeviceGroupId {
            get; set;
        }

        ///<summary>
        /// 对外跟踪的设备Id
        ///</summary>
        public string TrackingId {
            get; set;
        } // TrackingId (length: 50)

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
        /// 所处名单 （0 无，1 在黑名单，2 在白名单）
        ///</summary>
        public int ListType { get; set; } // ListType

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

        ///<summary>
        /// 是否在线
        ///</summary>
        public bool IsOnline {
            get; set;
        } // IsOnline

        /// <summary>
        /// 最后同步手机联系人的时间
        /// </summary>
        public DateTime? LastSyncContactTime {
            get; set;
        }

        ///<summary>
        /// 创建时间
        ///</summary>
        public System.DateTime CreateTime {
            get; set;
        } // CreateTime

        ///<summary>
        /// 更新时间
        ///</summary>
        public System.DateTime UpdateTime {
            get; set;
        } // UpdateTime

        ///<summary>
        /// 设备对应的姓名(待组织架构几张关联表开发完后再查询获取该值)
        ///</summary>
        public string RealName {
            get; set;
        } // UserName

        ///<summary>
        /// 接打电话数量
        ///</summary>
        public int CallsCount {
            get; set;
        } // CallsCount

        ///<summary>
        /// 收发短信数量
        ///</summary>
        public int SmsCount {
            get; set;
        } // SmsCount

        ///<summary>
        /// 最后通话时间
        ///</summary>
        public DateTime? LastCallTime {
            get; set;
        } // lastCallTime

        ///<summary>
        /// 最后创建短信时间
        ///</summary>
        public DateTime? LastSmsTime {
            get; set;
        } // lastSmsTime

        public bool Deleted {
            get; set;
        }

    }
}
