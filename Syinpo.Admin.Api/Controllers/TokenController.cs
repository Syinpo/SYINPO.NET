using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel.Client;
using IdentityServer4;
using Syinpo.BusinessLogic.Devices;
using Syinpo.BusinessLogic.Safety;
using Syinpo.BusinessLogic.Users;
using Syinpo.Core;
using Syinpo.Core.Domain.Identity;
using Syinpo.Model.ViewResult.Notifications;
using Syinpo.Unity.AspNetCore.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Syinpo.Admin.Api.Controllers {
    public class TokenController : BaseApiController {
        private readonly IConfiguration _configuration;
        private readonly ILogger<TokenController> _logger;
        public TokenController(  IConfiguration configuration, ILogger<TokenController> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        [HttpGet( "/api/token/device/create/{deviceUuid}" )]
        public async Task<IActionResult> CreateToken( string deviceUuid ) {


     
            var device = await IoC.Resolve<IDeviceService>().GetDeviceByDeviceUuid( deviceUuid );
           // _logger.LogWarning($" GetDeviceByDeviceUuid   /api/token/device/create/  {device?.DeviceUuid} PartnerSerialKeyId  [{device?.PartnerSerialKeyId}]  ");
            var claims = new List<Claim>
            {
                new Claim("deviceId", device.Id.ToString() ),
                new Claim("deviceUuid", device.DeviceUuid  ),
            };

            var token = IoC.Resolve<ITokenService>().GenerateAccessToken( claims );
            var tokenResult = new TokenResult {
                Token = token,
                ExpiresIn = 60 * 60
            };


            return Success( tokenResult );
        }

        [HttpGet( "/api/token/user/create/{username}/{password}" )]
        public async Task<IActionResult> CreateUserToken( string username, string password ) {
            var device = await IoC.Resolve<IUserService>().GetUserByUsername( username );

            var claims = new List<Claim>
            {
                new Claim("userId", device.Id.ToString() ),
            };

            var token = IoC.Resolve<ITokenService>().GenerateAccessToken( claims, 60 * 24* 365 * 10 );
            var tokenResult = new TokenResult {
                Token = token,
                ExpiresIn = 60 * 60
            };


            return Success( tokenResult );
        }
    }
}
