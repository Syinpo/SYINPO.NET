using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Syinpo.BusinessLogic.Safety;
using Syinpo.BusinessLogic.Users;
using Syinpo.Core;
using Syinpo.Core.Domain.Poco;
using Syinpo.Core.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.JsonWebTokens;

namespace Syinpo.Auth.Server {
    public class IdentityServerProfileService : IProfileService {
        protected readonly IUserService _userService;
        protected readonly ILogger Logger;

        public IdentityServerProfileService( IUserService userService, ILogger<IdentityServerProfileService> logger ) {
            this._userService = userService;
            this.Logger = logger;
        }


        public async Task GetProfileDataAsync( ProfileDataRequestContext context ) {
            var sub = context.Subject.GetSubjectId();

            var user = await _userService.GetUserById( Convert.ToInt32( sub ) );
            var claims = BuildClaims( user);

            context.IssuedClaims = claims;
        }

        private List<Claim> BuildClaims( User user ) {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim("username", user.Username),
                new Claim("supper", user.IsAdmin.ToString().ToLowerInvariant()),
            };

            return claims;
        }

        public async Task IsActiveAsync( IsActiveContext context ) {
            var sub = context.Subject.GetSubjectId();
            var user = await _userService.GetUserById( Convert.ToInt32( sub ) );
            context.IsActive = !user.Deleted && user.Approved;
        }
    }
}
