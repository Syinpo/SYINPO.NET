using System;
using System.Collections.Generic;
using System.Text;

namespace Syinpo.Model.Dto.Monitor {
    public class ResponseSnapDto {
        /// <summary>
        /// 主键
        /// </summary>
        public int Id {
            get;set;
        }

        /// <summary>
        /// 跟踪Id
        /// </summary>
        public string TraceId {
            get; set;
        }

        /// <summary>
        /// 处理是否成功
        /// </summary>
        public bool Success {
            get; set;
        }

        /// <summary>
        /// 数据大小
        /// </summary>
        public long ContentLength {
            get; set;
        }

        /// <summary>
        /// 响应体
        /// </summary>
        public string ResponseBody {
            get; set;
        }

        /// <summary>
        /// 耗时（毫秒）
        /// </summary>
        public long Elapsed {
            get; set;
        }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime {
            get; set;
        }
    }
}
