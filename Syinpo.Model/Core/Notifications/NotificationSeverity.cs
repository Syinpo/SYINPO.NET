using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Syinpo.Model.Core.Notifications {
    public enum NotificationSeverity : int {
        /// <summary>
        /// 信息.
        /// </summary>
        [Display( Name = "信息" )]
        Info = 0,

        /// <summary>
        /// 成功.
        /// </summary>
        [Display( Name = "成功" )]
        Success = 1,

        /// <summary>
        /// 警告.
        /// </summary>
        [Display( Name = "警告" )]
        Warn = 2,

        /// <summary>
        /// 错误.
        /// </summary>
        [Display( Name = "错误" )]
        Error = 3,

        /// <summary>
        /// 致命.
        /// </summary>
        [Display( Name = "致命" )]
        Fatal = 4
    }
}
