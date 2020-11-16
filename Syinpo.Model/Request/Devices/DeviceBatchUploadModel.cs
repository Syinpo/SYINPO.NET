using System;
using System.Collections.Generic;
using System.Text;

namespace Syinpo.Model.Request.Devices {
    public class DeviceBatchUploadModel {
        public DeviceBatchUploadModel()
        {
            BatchItems = new List<BatchItemModel>();
        }

        /// <summary>
        /// 批次消息Id，参考雪花算法生成Id
        /// </summary>
        public string BatchId {
            get;set;
        }

        /// <summary>
        /// 上报集合，最大长度为100
        /// </summary>
        public List<BatchItemModel> BatchItems {
            get;set;
        }

        public class BatchItemModel {
            /// <summary>
            /// 消息Id，移动端保证编码唯一性，为查找具体项和扩展准备
            /// 参考雪花算法生成Id
            /// </summary>
            public string Id {
                get;set;
            }

            /// <summary>
            /// 请求类型，为以前Api得路由，例如传：/api/device/sms/create
            /// </summary>
            public string RequestRoute {
                get;set;
            }

            /// <summary>
            /// 请求入参得数据，为以前Http接口的数据模型
            /// </summary>
            public object Content {
                get;set;
            }


            /// <summary>
            /// 是否需要服务端通知处理结果
            /// </summary>
            public bool NeedCallback {
                get;set;
            }
        }

    }
}
