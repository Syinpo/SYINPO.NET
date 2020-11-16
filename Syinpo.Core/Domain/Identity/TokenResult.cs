using System;
using System.Collections.Generic;
using System.Text;

namespace Syinpo.Core.Domain.Identity {
    public class TokenResult {
        /// <summary>
        /// 身份令牌
        /// </summary>
        public string Token {
            get;
            set;
        }

        /// <summary>
        /// 过期时间，已秒为单位
        /// </summary>
        public int ExpiresIn
        {
            get;
            set;

        }
    }
}
