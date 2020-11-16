using System;
using System.Collections.Generic;
using System.Text;

namespace Syinpo.Model.Dto.Users {
    public class UserForConsoleDto : UserDto {
        /// <summary>
        /// 用户选择的角色
        /// </summary>
        public int[] UserRoleIds {
            get; set;
        }

        /// <summary>
        /// 用户选择的角色
        /// </summary>
        public string[] UserRoleNames {
            get;set;
        }

        /// <summary>
        /// 用户选择的组织架构
        /// </summary>
        public int[] DeptIds {
            get; set;
        }
    }
}
