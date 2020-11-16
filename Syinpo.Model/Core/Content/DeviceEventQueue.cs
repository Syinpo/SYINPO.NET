using System;
using System.Collections.Generic;
using System.Text;

namespace Syinpo.Model.Core.Content {
    public class DeviceEventQueue<T> {
        /// <summary>
        /// 设备Id
        /// </summary>
        public int DeviceId {
            get; set;
        }

        /// <summary>
        /// 对象
        /// </summary>
        public T Content {
            get; set;
        }
    }
}
