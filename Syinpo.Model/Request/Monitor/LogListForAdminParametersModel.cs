using System;
using System.Collections.Generic;
using System.Text;
using Syinpo.Core.Monitor;

namespace Syinpo.Model.Request.Monitor {
   public class LogListForAdminParametersModel {
        /// <summary>
        /// 关键词
        /// </summary>
        public string KeyWords {
            get; set;
        }

        /// <summary>
        /// 请求类型：10-http, 20-signalr
        /// </summary>
        public RequestTypeEnum? RequestType {
            get;set;
        }

        /// <summary>
        /// 操作器
        /// </summary>
        public string Operation {
            get;set;
        }

        /// <summary>
        /// 最小耗时(毫秒)范围
        /// </summary>
        public long? MinElapsed {
            get; set;
        }

        /// <summary>
        /// 最大耗时(毫秒)范围
        /// </summary>
        public long? MaxElapsed {
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
    }
}
