using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation.Attributes;
using FluentValidation.Validators;
using Syinpo.Model.Validators.Users;

namespace Syinpo.Model.Dto.Users {
    [Validator( typeof( UserDtoValidator ) )]
    public class UserDto : BaseDto {

        ///<summary>
        /// 主键
        ///</summary>
        public int Id {
            get; set;
        } // Id (Primary key)

        ///<summary>
        /// 合作伙伴主键
        ///</summary>
        public int PartnerId {
            get; set;
        } // PartnerId

        ///<summary>
        /// 父用户主键
        ///</summary>
        public int ParentUserId {
            get; set;
        } // ParentUserId

        ///<summary>
        /// 对外使用主键
        ///</summary>
        public System.Guid UserGuid {
            get; set;
        } // UserGuid

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
        /// 真实姓名
        ///</summary>
        public string RealName {
            get; set;
        } // DisplayName (length: 1000)
        ///<summary>
        /// 电子邮件
        ///</summary>
        public string Email {
            get; set;
        } // Email (length: 1000)

        ///<summary>
        /// 手机号码
        ///</summary>
        public string Mobile {
            get; set;
        } // Mobile (length: 1000)

        ///<summary>
        /// 批准
        ///</summary>
        public bool Approved {
            get; set;
        } // Approved

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
        /// 最后登录时间
        ///</summary>
        public System.DateTime? LastLoginDate {
            get; set;
        } // LastLoginDate

        ///<summary>
        /// 最后登录设备
        ///</summary>
        public int? LastLoginDeviceId {
            get; set;
        } // LastLoginDeviceId

        public bool Deleted
        {
            get;set;
        }

        public bool IsAdmin { get; set; }
        ///<summary>
        /// 对外用户
        ///</summary>
        public bool IsHttpUse { get; set; }
    }
}
