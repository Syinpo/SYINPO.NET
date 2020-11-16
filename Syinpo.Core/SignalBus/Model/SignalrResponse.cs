using System;
using System.Collections.Generic;
using System.Text;

namespace Syinpo.Core.SignalBus.Model {
    public class SignalrResponse {
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool Success {
            get;
            set;
        }

        /// <summary>
        /// 消息内容
        /// </summary>
        public string Message {
            get; set;
        }

        /// <summary>
        /// 数据
        /// </summary>
        public object Data {
            get; set;
        }
    }
}
