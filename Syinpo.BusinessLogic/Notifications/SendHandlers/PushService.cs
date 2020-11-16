using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Syinpo.BusinessLogic.Devices;
using Syinpo.BusinessLogic.Safety;
using Syinpo.BusinessLogic.SignalR.Hubs;
using Syinpo.BusinessLogic.SignalR.Notifications;
using Syinpo.BusinessLogic.SignalR.Online;
using Syinpo.Core;
using Syinpo.Core.Domain.Identity;
using Syinpo.Core.Domain.Poco;
using Syinpo.Core.Helpers;
using Syinpo.Core.Mapper;
using Syinpo.Core.Reflection;
using Syinpo.Core.SignalBus.Client;
using Syinpo.Core.SignalBus.Model;
using Syinpo.Model;
using Syinpo.Model.Core.Notifications;
using Syinpo.Model.Dto.Devices;
using Syinpo.Model.Dto.Notifications;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Syinpo.BusinessLogic.Notifications.SendHandlers {
    public class PushService : IPushService {

        #region 字段

        private readonly ITokenService _tokenService;
        private readonly IMediator _mediator;
        private readonly IDeviceService _deviceService;
        private readonly ITypeFinder _typeFinder;
        private readonly ITypeResolve _typeResolve;
        private readonly IOnlineManager _onlineManager;
        private readonly ICurrent _current;
        private readonly ILogger<PushService> _logger;
        private readonly SignalrClient _signalrClient;
        private readonly SysOptions _options;

        #endregion

        #region 构造函数

        public PushService( ICurrent current, IOnlineManager onlineManager, ILogger<PushService> logger, ITokenService tokenService, IMediator mediator, IDeviceService deviceService, ITypeFinder typeFinder, ITypeResolve typeResolve, SignalrClient signalrClient, IOptions<SysOptions> options ) {
            _tokenService = tokenService;
            _mediator = mediator;
            _deviceService = deviceService;
            _typeFinder = typeFinder;
            _typeResolve = typeResolve;
            _current = current;
            _onlineManager = onlineManager;
            _logger = logger;
            _signalrClient = signalrClient;
            _options = options?.Value;
        }

        #endregion


        #region 公共

        private async Task SendAgent( DeviceNotification deviceNotification ) {
            var onlineClients = _onlineManager.GetAllByDeviceId( deviceNotification.DeviceId );

            foreach( var onlineClient in onlineClients ) {
                if( _signalrClient == null ) {
                    _logger.LogDebug( "不能获取DeviceId " + deviceNotification.DeviceId + " 在 connectionId " + onlineClient.ConnectionId + " 从  SignalR AgentHub!" );
                    continue;
                }

                await _signalrClient.Push( onlineClient.SysName, new Core.SignalBus.Model.SignalrMessage
                {
                    Type = typeof(DeviceNotification).FullName,
                    SmgType = SignalrMessageTypeEnum.Push,
                    JsonData = JsonHelper.ToJson( deviceNotification )
                } );
            }
        }

        private async Task SendAgent( UserNotification userNotification ) {
            var onlineClients = _onlineManager.GetAllByUserId( userNotification.UserId );

            foreach( var onlineClient in onlineClients ) {
                if( _signalrClient == null ) {
                    _logger.LogDebug( "不能获取UserId " + userNotification.UserId + " 在 connectionId " + onlineClient.ConnectionId + " 从  SignalR AgentHub!" );
                    continue;
                }

                await _signalrClient.Push( onlineClient.SysName, new Core.SignalBus.Model.SignalrMessage {
                    Type = typeof( UserNotification ).FullName,
                    SmgType = SignalrMessageTypeEnum.Push,
                    JsonData = JsonHelper.ToJson( userNotification )
                } );
            }
        }



        private async Task SendToDevice( int deviceId, string notifyType, object data ) {
            try {
                var deviceNotification = new DeviceNotification {
                    DeviceId = deviceId,
                    Notification = new NotificationObject {
                        Header = new NotificationHead {
                            Category = "deivce",
                            NotifyType = notifyType,
                        },
                        Body = data
                    }
                };

                await SendToDevice(deviceNotification);
            }
            catch( Exception ex ) {
                _logger.LogWarning( $"SendToDevicen错误:{deviceId},{notifyType},{ JsonHelper.ToJson( data )}， {ex}." );
            }
        }

        /// <summary>
        /// 通用
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task SendToDevice( int deviceId, NotificationObject data ) {
            try {

                var deviceNotification = new DeviceNotification {
                    DeviceId = deviceId,
                    Notification = data
                };

                await SendToDevice( deviceNotification );
            }
            catch( Exception ex ) {
                _logger.LogWarning( $"SendToDevicen2错误:{deviceId},{ JsonHelper.ToJson( data )}， {ex}." );
            }
        }


        /// <summary>
        /// 通用
        /// </summary>
        /// <param name="deviceNotification"></param>
        /// <returns></returns>
        public async Task SendToDevice( DeviceNotification deviceNotification ) {
            try
            {
                if (deviceNotification == null)
                    return;

                if( !_onlineManager.IsDeviceOnline( deviceNotification.DeviceId ) )
                    return;

                if( _options.SignalrBus.UserSignalrBus ) {
                    await SendAgent( deviceNotification );
                }
                else {
                    await _mediator.Publish( deviceNotification );
                }
            }
            catch( Exception ex ) {
                _logger.LogWarning( $"SendToDevicen2错误:{ JsonHelper.ToJson( deviceNotification )}， {ex}." );
            }
        }

        #endregion

        #region ToUser

        /// <summary>
        /// 通用
        /// </summary>
        /// <param name="userNotification"></param>
        /// <returns></returns>
        public async Task SendToUser( UserNotification userNotification ) {
            try {
                if( userNotification == null )
                    return;

                if( !_onlineManager.IsOnline( userNotification.UserId ) )
                    return;

                if( _options.SignalrBus.UserSignalrBus ) {
                    await SendAgent( userNotification );
                }
                else {
                    await _mediator.Publish( userNotification );
                }
            }
            catch( Exception ex ) {
                _logger.LogWarning( $"SendToUser错误:{ JsonHelper.ToJson( userNotification )}， {ex}." );
            }
        }

        #endregion


        #region ToDevice


        /// <summary>
        /// 向移动端发送Token
        /// </summary>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        public async Task SendToDeviceWithAccessToken( int deviceId ) {
            var claims = new List<Claim>
            {
                   new Claim("deviceId", deviceId.ToString() ),
               };
            var token = _tokenService.GenerateAccessToken( claims );
            var data = new TokenResult {
                Token = token,
                ExpiresIn = 60 * 60
            };

            await SendToDevice( deviceId, "getAccessToken", data );
        }

        #endregion
    }
}
