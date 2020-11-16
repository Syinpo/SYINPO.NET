using System;
using System.Collections.Generic;
using Syinpo.Core;
using Syinpo.Core.Helpers;

namespace Syinpo.BusinessLogic.SignalR.Online {
    public class OnlineClient {
        public OnlineClient() {
            ConnectTime = DateTime.Now;
            Properties = new Dictionary<string, object>();
        }

        public OnlineClient( string connectionId, string ipAddress, int? userId, int? deviceId, string sysName ) : this() {
            ConnectionId = connectionId;
            IpAddress = ipAddress;
            UserId = userId;
            DeviceId = deviceId;
            ServerName = CommonHelper.GetServerName();
            SysName = sysName;
        }

        public object this[ string key ] {
            get => Properties[ key ];
            set => Properties[ key ] = value;
        }

        /// <summary>
        /// SignalR ConnectionId
        /// </summary>
        public string ConnectionId {
            get; set;
        }

        /// <summary>
        /// 用户Id
        /// </summary>
        public int? UserId {
            get; set;
        }

        /// <summary>
        /// 设备Id
        /// </summary>
        public int? DeviceId {
            get; set;
        }

        /// <summary>
        /// 客户端的连接建立时间
        /// </summary>
        public DateTime ConnectTime {
            get; set;
        }

        /// <summary>
        /// Ip地址
        /// </summary>
        public string IpAddress {
            get; set;
        }

        /// <summary>
        /// 服务器
        /// </summary>
        public string ServerName {
            get; set;
        }

        /// <summary>
        /// 出自系统
        /// </summary>
        public string SysName {
            get; set;
        }

        /// <summary>
        /// 客户端的自定义属性
        /// </summary>
        public Dictionary<string, object> Properties {
            get => _properties;
            set => _properties = value ?? throw new ArgumentNullException( nameof( value ) );
        }
        private Dictionary<string, object> _properties;


        public override string ToString() {
            return JsonHelper.ToJson( this );
        }
    }
}
