using IdentityModel.Client;
using IdentityServer4;
using Syinpo.BusinessLogic.Users;
using Syinpo.Core;
using Syinpo.Unity.AspNetCore.Controllers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using Syinpo.Core.Extensions;

namespace Syinpo.Auth.Server.Controllers
{

    public class TokenController : BaseApiController
    {
        private readonly IdentityServerTools _tokenService;
        private readonly IUserService _userService;
        private readonly IHttpClientFactory _httpClientFactory;

        /// <summary>
        /// https://github.com/IdentityModel/IdentityModel/blob/master/samples/HttpClientFactorySample/WebApplication1/Controllers/HomeController.cs
        /// </summary>
        /// <param name="httpClientFactory"></param>
        /// <param name="tokenService"></param>
        /// <param name="userService"></param>
        public TokenController(IHttpClientFactory httpClientFactory, IdentityServerTools tokenService,IUserService userService)
        {
            _tokenService = tokenService;
            _userService = userService;
            _httpClientFactory = httpClientFactory;
        }

        [HttpPost("/api/token/create")]
        public async Task<IActionResult> CreateToken([FromBody] TokenLoginModel login)
        {
            Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();
            keyValuePairs.Add("ConsoleType", "0" );

            var client = _httpClientFactory.CreateClient("token_client");
            var tokenClient = new TokenClient(client, new TokenClientOptions
            {
                Address = $"{CommonHelper.GetWebLocation()}connect/token",
                ClientId = "client",
                ClientSecret = "secret",
                Parameters = keyValuePairs,
            });
            var tokenResponse = await tokenClient.RequestPasswordTokenAsync(login.Username, login.Password,
                "api" + " " + IdentityServerConstants.StandardScopes.OfflineAccess);

            if (tokenResponse.IsError)
            {
                return Error(500, tokenResponse.ErrorType.ToString(),
                    /*tokenResponse.Error + "," + tokenResponse.ErrorDescription*/ "用户未批准或用户名密码错误");
            }

            var user = await _userService.GetUserByUsername(login.Username);



            if (login.IncludeRefreshToken)
            {
                return Success(new
                {
                    token = tokenResponse.AccessToken,
                    refreshToken = tokenResponse.RefreshToken,
                    expiresIn = tokenResponse.ExpiresIn, // 秒
                    expiresAtUtc = DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn),
                    IsAdmin = user.IsAdmin

                });
            }

            return Success(new
            {
                token = tokenResponse.AccessToken,
                expiresIn = tokenResponse.ExpiresIn, // 秒
                expiresAtUtc = DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn),
                IsAdmin = user.IsAdmin
            });
        }

        [HttpPost("/api/token/refresh")]
        public async Task<IActionResult> RefeshToken(RefreshTokenModel model)
        {
            var client = _httpClientFactory.CreateClient("token_client");
            var tokenClient = new TokenClient(client, new TokenClientOptions
            {
                Address = $"{CommonHelper.GetWebLocation()}connect/token",
                ClientId = "client",
                ClientSecret = "secret"
            });
            // var tokenClient = new TokenClient( $"{CommonHelper.GetWebLocation()}connect/token", "client", "secret" );

            var tokenResponse = await tokenClient.RequestRefreshTokenAsync(model.RefreshToken);

            if (tokenResponse.IsError)
            {
                return Error(500, tokenResponse.ErrorType.ToString(),
                    tokenResponse.Error + "," + tokenResponse.ErrorDescription);
            }

            return Ok(new
            {
                token = tokenResponse.AccessToken,
                expiresIn = tokenResponse.ExpiresIn, // 秒
                expiresAtUtc = DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn),
                refreshToken = tokenResponse.RefreshToken,
            });
        }

        public class TokenLoginModel
        {

            public string Username
            {
                get; set;
            }

            public string Password
            {
                get; set;
            }

            public bool IncludeRefreshToken
            {
                get; set;
            }
        }

        public class RefreshTokenModel
        {
            public string Token
            {
                get; set;
            }

            public string RefreshToken
            {
                get; set;
            }
        }
    }
}