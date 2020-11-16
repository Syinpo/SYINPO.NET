using System.Text;

namespace Syinpo.Unity.Firewall {
    public class FirewallOptions {

        /// <summary>
        /// 周期
        /// </summary>
        public TimeUnitEnum TimeUnit {
            get;set;
        }

        /// <summary>
        /// Today时，值无意义
        /// </summary>
        public int TimeValue {
            get;
            set;
        }

        /// <summary>
        /// 最大请求数
        /// </summary>
        public int MaxRequest {
            get;set;
        }


        public string GenerateKey( string prefix = "" ) {
            var builder = new StringBuilder();

            if( !string.IsNullOrEmpty( prefix ) ) {
                builder.Append( prefix );
                builder.Append( ':' );
            }

            builder.Append( (int)TimeUnit );
            builder.Append( ':' );

            builder.Append( TimeValue );
            builder.Append( ':' );

            builder.Append( MaxRequest );
            builder.Append( ':' );


            return builder.ToString();
        }

    }
}
