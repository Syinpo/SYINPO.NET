using System;
using System.Collections.Generic;
using System.Text;
using Syinpo.Core.Domain.Identity;

namespace Syinpo.Model.ViewResult.Notifications {
    public class RegisterDeviceResult {
        public TokenResult TokenInfo {
            get; set;
        }

        public string WeiXinOpenId {
            get; set;
        }

        /// <summary>
        /// 是否开通SCRM
        /// </summary>
        public bool IsWkf {
            get; set;
        }

        /// <summary>
        /// 是否开通工作手机
        /// </summary>
        public bool IsGzsj {
            get; set;
        }
    }
}
