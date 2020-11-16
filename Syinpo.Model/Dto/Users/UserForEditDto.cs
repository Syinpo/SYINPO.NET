using System;
using System.Collections.Generic;
using System.Text;

namespace Syinpo.Model.Dto.Users {
    public class UserForEditDto {
        /// <summary>
        /// 用户选择的角色
        /// </summary>
        public int[] UserRoleIds {
            get; set;
        }

        /// <summary>
        /// 用户选择的组织架构
        /// </summary>
        public int[] DeptIds {
            get; set;
        }

        ///<summary>
        /// 主键
        ///</summary>
        public int Id {
            get; set;
        }

        ///<summary>
        /// 用户名
        ///</summary>
        public string Username {
            get; set;
        } // Username (length: 1000)

        ///<summary>
        /// 密码( 创建传密码， 更新不用传)
        ///</summary>
        public string Password {
            get; set;
        } // Password (length: 1000)

        ///<summary>
        /// 显示名称
        ///</summary>
        public string DisplayName {
            get; set;
        } // DisplayName (length: 1000)

        /// <summary>
        /// 真实姓名
        /// </summary>
        public string RealName {
            get;set;
        }

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

    }

    public class UserForAdminEditDto : UserForEditDto {
        /// <summary>
        /// 合作伙伴Id
        /// </summary>
        public int PartnerId {
            get; set;
        }
    }
}
