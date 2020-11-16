using System;
using System.Collections.Generic;
using System.Text;

namespace Syinpo.Model.Dto.Devices {
    public class DeviceEventDto {
        /// <summary>
        /// 消息标识
        /// </summary>
        public string EventUuid {
            get;
            set;
        }

        ///<summary>
        /// 事件Id
        ///</summary>
        public int EventId {
            get; set;
        }

        ///<summary>
        /// 事件描述
        ///</summary>
        public string Description {
            get; set;
        }

        /// <summary>
        /// 上下文数据
        /// </summary>
        public object Context {
            get;
            set;
        }


        ///<summary>
        /// 事件时间
        ///</summary>
        public long On {
            get; set;
        }
    }

    public class DeviceEventBulkDto
    {
        public List<DeviceEventDto> DeviceEvents { get; set; }
    }
}
