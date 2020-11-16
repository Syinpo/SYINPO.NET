using System;
using System.Collections.Concurrent;
using System.Text;
using Microsoft.OpenApi.Writers;
using Syinpo.Core.Extensions;

namespace Syinpo.Unity.Firewall {
    public class ThrottlerItem {
        public ThrottlerItem()
        {
            LastRequests = new ConcurrentQueue<DateTime>();
        }

        /// <summary>
        /// 设备Id，Uuid，或者UserId，Username
        /// </summary>
        public string Identity {
            get;set;
        }

        /// <summary>
        /// 路由
        /// </summary>
        public string Route {
            get; set;
        }

        /// <summary>
        /// 规则名称
        /// </summary>
        public string RuleName {
            get; set;
        }

        /// <summary>
        /// 配置参数Key
        /// </summary>
        public string OptionKey {
            get;set;
        }

        /// <summary>
        /// 最后得请求时间
        /// </summary>
        public DateTime LastRequestTime {
            get;set;
        }

        /// <summary>
        /// 请求得队列
        /// </summary>
        public ConcurrentQueue<DateTime> LastRequests {
            get; set;
        }


        public string GenerateKey( string prefix = "" ) {
            var builder = new StringBuilder();

            if( !string.IsNullOrEmpty( prefix ) ) {
                builder.Append( prefix );
                builder.Append( ':' );
            }

            builder.Append( Identity );
            builder.Append( ':' );

            builder.Append( Route );
            builder.Append( ':' );

            builder.Append( RuleName );
            builder.Append( ':' );

            builder.Append( OptionKey );
            builder.Append( ':' );

            return builder.ToString().CreateMD5();
        }

    }
}
