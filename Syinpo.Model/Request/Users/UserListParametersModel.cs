using System;
using System.Collections.Generic;
using System.Text;

namespace Syinpo.Model.Request.Users {
    public class UserListParametersModel {
        /// <summary>
        /// 查询关键字
        /// </summary>
        public string Keyword {
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

        /// <summary>
        /// 合作伙伴Id
        /// </summary>
        public int? PartnerId { get; set; } = 0;

        /// <summary>
        /// 用户角色Ids
        /// </summary>
        public int[] UserRoleIds { get; set; }
    }
}
