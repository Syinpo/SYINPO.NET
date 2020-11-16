using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Syinpo.BusinessLogic.Devices;
using Syinpo.BusinessLogic.Safety;
using Syinpo.BusinessLogic.SignalR.Hubs;
using Syinpo.Core;
using Syinpo.Core.Domain.Identity;
using Syinpo.Core.Domain.Poco;
using Syinpo.Core.Helpers;
using Syinpo.Core.Mapper;
using Syinpo.Model;
using Syinpo.Model.Dto.Devices;
using Syinpo.Model.ViewResult.Notifications;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace Syinpo.BusinessLogic.Notifications.ReceiveHandlers {
    public class RegisterDeviceHandler : BaseHandler, INotificationHandler {
        #region 字段

        private readonly ICurrent _current;
        private readonly ITokenService _tokenService;
        private readonly IDeviceService _deviceService;
        private readonly ILogger<RegisterDeviceHandler> _logger;

        #endregion

        #region 构造函数

        public RegisterDeviceHandler( ICurrent current, ITokenService tokenService,  IDeviceService deviceService,  ILogger<RegisterDeviceHandler> logger ) {
            _current = current;
            _tokenService = tokenService;
            _deviceService = deviceService;
            _logger = logger;
        }

        #endregion


        public bool IsMatch( string notifyType ) {
            return notifyType.ToLowerInvariant() == nameof( RegisterDeviceHandler ).ToLowerInvariant().Replace( "handler", "" );
        }

        public async Task<object> Execute( JObject body ) {

            var dto = body.ToObject<DeviceForRegisterDeviceDto>();


            var deviceRead = await _deviceService.GetDeviceByDeviceUuid( dto.DeviceUuid );
            if ( deviceRead == null)
            {
                return Error("设备在后台系统不存在");
            }

            var device = await _deviceService.GetDeviceById( deviceRead.Id );
            device = dto.MapTo<Device>(device);
            device.AssistantVersion = dto.AppVersion;//这里做映射
            device.UpdateTime = DateTime.Now;
            await _deviceService.UpdateDevice(device);

            var claims = new List<Claim>
                {
                    new Claim("deviceId", device.Id.ToString() ),
                    new Claim("deviceUuid", device.DeviceUuid  ),
                };

            var token = _tokenService.GenerateAccessToken(claims);
            var tokenResult = new TokenResult
            {
                Token = token,
                ExpiresIn = 60 * 60 * 24
            };

            return Success(new RegisterDeviceResult
            {
                TokenInfo = tokenResult,
            } );
        }

    }
}
