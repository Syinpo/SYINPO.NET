using System;
using System.Collections.Concurrent;

namespace Syinpo.Model.Request {
    /// <summary>
    /// RequestBase
    /// </summary>
    public class RequestBase {
        /// <summary>
        /// 关键字
        /// </summary>
        public string KeyWords
        {
            get; set;
        }

        /// <summary>
        /// 当前页,从1开始
        /// </summary>
        public int Page { get; set; } = 1;

        /// <summary>
        /// 分页大小
        /// </summary>
        public int PageSize { get; set; } = 20;
    }
}
