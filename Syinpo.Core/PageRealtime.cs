using System;
using System.Collections.Generic;
using System.Text;

namespace Syinpo.Core {
    public class PageRealtime<T> {
        /// <summary>
        /// 数据源
        /// </summary>
        public List<T> Data { get; set; }

        /// <summary>
        /// 分页元数据
        /// </summary>
        public Metadata Pagination { get; set; }

        public class Metadata {
            /// <summary>
            /// 检索小于此Id的记录
            /// </summary>
            public int MaxId { get; set; }

            /// <summary>
            /// 检索大于此Id的记录
            /// </summary>
            public int SinceId { get; set; }

            /// <summary>
            /// 刷新地址
            /// </summary>
            public string RefreshUrl { get; set; }

            /// <summary>
            /// 下一页
            /// </summary>
            public string NextUrl { get; set; }

            /// <summary>
            /// 每页数量
            /// </summary>
            public int Count { get; set; }
        }
    }
}
