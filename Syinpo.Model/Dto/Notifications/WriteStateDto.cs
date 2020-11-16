using System;
using System.Collections.Generic;
using System.Text;

namespace Syinpo.Model.Dto.Notifications {
    public class WriteStateDto {
        /// <summary>
        /// 跟踪标识
        /// </summary>
        public string SpanId {
            get; set;
        }

        /// <summary>
        /// 状态-参考枚举文档
        /// </summary>
        public int NotificationState {
            get; set;
        }

        /// <summary>
        /// 备注
        /// </summary>
        public string Memo {
            get; set;
        }

        /// <summary>
        /// 附加数据，字典对象： { "key1": 1,  "key2": "2" }
        /// </summary>
        public Dictionary<string, object> AdditionalData {
            get; set;
        }
    }
}
