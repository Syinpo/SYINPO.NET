using System;
using System.Collections.Generic;
using System.Text;

namespace Syinpo.BusinessLogic.Caches {
    public enum ChangeType {
        /// <summary>
        /// 无
        /// </summary>
        None,

        /// <summary>
        /// 删除
        /// </summary>
        Delete,

        /// <summary>
        /// 添加
        /// </summary>
        Insert,

        /// <summary>
        /// 更新
        /// </summary>
        Update,
    }
}
