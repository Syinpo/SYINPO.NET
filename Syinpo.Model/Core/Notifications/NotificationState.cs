using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Syinpo.Model.Core.Notifications {
    /// <summary>
    /// 推送通知的状态枚举
    /// </summary>
    public enum NotificationState {
        /// <summary>
        ///  未推送的通知
        /// </summary>
        [Display( Name = "未推送" )]
        Unread = 0,

        /// <summary>
        /// 已读通知
        /// </summary>
        [Display( Name = "已推送" )]
        Read = 1,

        /// <summary>
        /// 处理成功
        /// </summary>
        [Display( Name = "客户端处理成功", AutoGenerateField = true )]
        Success = 3,

        /// <summary>
        /// 处理失败
        /// </summary>
        [Display( Name = "客户端处理失败", AutoGenerateField = true )]
        Error = 4,
    }
}
