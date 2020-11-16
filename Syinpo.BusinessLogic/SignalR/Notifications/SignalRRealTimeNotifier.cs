using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Syinpo.BusinessLogic.Devices;
using Syinpo.BusinessLogic.SignalR.Hubs;
using Syinpo.BusinessLogic.SignalR.Online;
using Syinpo.Core;
using Syinpo.Core.Helpers;
using Syinpo.Core.Monitor;
using Syinpo.Core.Monitor.PackModule;
using Syinpo.Model.Core.Notifications;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Syinpo.BusinessLogic.SignalR.Notifications {
    public class SignalRRealTimeNotifier : IRealTimeNotifier {
        public readonly ILogger<SignalRRealTimeNotifier> Logger;
        private readonly IOnlineManager _onlineManager;
        private readonly IHubContext<DeviceHub> _deviceHubContext;
        private readonly IPackStore<HttpLog> _logStore;
        private readonly IDeviceService _deviceService;
        public SignalRRealTimeNotifier(
            IOnlineManager onlineManager,
            IHubContext<DeviceHub> deviceHubContext,
            ILogger<SignalRRealTimeNotifier> logger,
            IPackStore<HttpLog> logStore,
            IDeviceService deviceService) {
            _onlineManager = onlineManager;
            _deviceHubContext = deviceHubContext;
            Logger = logger;
            _logStore = logStore;
            _deviceService = deviceService;
        }

        #region 公共方法
        private void WriteHttpLog( RequestTypeEnum requestType, string traceId, string requestBody, string operation,int deviceId)
        {

            string DeviceUuid= _deviceService.GetDeviceById_Pref(deviceId).ConfigureAwait(false).GetAwaiter().GetResult()?.DeviceUuid;

            var now = DateTime.Now;
            var group = now.ToString("yyyyMMddHHmm");
            long.TryParse(group, out long requestGroup);

            HttpLog _log = new HttpLog()
                {
                    TraceId = traceId,
                    RequestTime = now,
                    RequestType = (int)requestType,
                    RequestGroup = requestGroup,
                    RemoteIpAddress = CommonHelper.GetClientIpAddress(),
                    RemotePort = CommonHelper.GetClientIpPort(),
                    IdentityIsAuthenticated = true,
                    IdentityName = DeviceUuid,
                    RequestBody = requestBody,
                    RequestContentLength = requestBody == null ? 0 : Encoding.Default.GetByteCount(requestBody),
                    Operation = operation,
                    ServerName = CommonHelper.GetServerName()
                };
                this._logStore.AddQueue(_log);

        }
        #endregion

        public async Task SendDeviceNotificationsAsync( DeviceNotification deviceNotification ) {
            //Logger.LogWarning("1--signalRClient.SendAsync-getNotification：" + JsonHelper.ToJson(deviceNotification));
            try
            {
                int send = 0;
                var onlineClients = _onlineManager.GetAllByDeviceId( deviceNotification.DeviceId );

                if ( 1 == 2 ) {
                    var onlineClient = onlineClients.OrderByDescending( p => p.ConnectTime ).FirstOrDefault();
                    if( onlineClient != null ) {
                        var signalRClient = _deviceHubContext.Clients.Client( onlineClient.ConnectionId );
                        if( signalRClient == null ) {
                            Logger.LogDebug( "不能获取DeviceId " + deviceNotification.DeviceId + " 在 connectionId " + onlineClient.ConnectionId + " 从  SignalR WeiChatHub!" );
                        }

                        await signalRClient.SendAsync( "getNotification", JsonHelper.ToJson( deviceNotification.Notification ) );
                    }
                }
                else {
                    foreach( var onlineClient in onlineClients ) {
                        var signalRClient = _deviceHubContext.Clients.Client( onlineClient.ConnectionId );
                        if( signalRClient == null ) {

                            Logger.LogDebug( "不能获取DeviceId " + deviceNotification.DeviceId + " 在 connectionId " + onlineClient.ConnectionId + " 从  SignalR WeiChatHub!" );
                            continue;
                        }
                        //Logger.LogWarning("2--signalRClient.SendAsync-getNotification：" + JsonHelper.ToJson(deviceNotification));
                        await signalRClient.SendAsync( "getNotification", JsonHelper.ToJson( deviceNotification.Notification ) );
                        send++;

                        //推送记日志
                        WriteHttpLog(
                       RequestTypeEnum.Signalr,
                       CommonHelper.NewSequentialGuid().ToString(),
                       JsonConvert.SerializeObject(deviceNotification.Notification),
                       "Signalr#SendDeviceNotifications", deviceNotification.DeviceId);
                    }

                    //if( send == 0 ) {
                    //    Logger.LogError( "不能发送通知给用户: " + deviceNotification.DeviceId );
                    //}
                    //else if( send == 1 ) {
                    //    Logger.LogError( "连接没有重复情况，推送发送成功: " + deviceNotification.DeviceId );
                    //}
                    //else if( send > 1 ) {
                    //    Logger.LogError( "连接有重复情况: " + deviceNotification.DeviceId );
                    //}
                }
            }
            catch( Exception ex ) {
                Logger.LogWarning( "不能发送通知给用户: " + deviceNotification.DeviceId );
                Logger.LogWarning( ex.ToString(), ex );
            }

            await Task.FromResult( 0 );
        }

        public async Task SendDeviceNotificationsAsync( DeviceNotification[] deviceNotifications ) {
            foreach( var deviceNotificatio in deviceNotifications ) {
                await SendDeviceNotificationsAsync( deviceNotificatio );
            }

            await Task.FromResult( 0 );
        }
    }
}
