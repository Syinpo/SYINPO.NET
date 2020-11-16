using System;
using System.Linq;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Validation;
using Syinpo.BusinessLogic.Content;
using Syinpo.BusinessLogic.Users;
using Syinpo.Core;
using Microsoft.Extensions.Logging;

namespace Syinpo.Auth.Server {
    public class OAuthResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator {

        protected readonly IUserService _userService;
        protected readonly ILogger Logger;

        public OAuthResourceOwnerPasswordValidator( IUserService userService, ILogger<OAuthResourceOwnerPasswordValidator> logger  )
        {
            _userService = userService;
            Logger = logger;
        }

        public async Task ValidateAsync( ResourceOwnerPasswordValidationContext context ) {
            bool success = false;

            try {
                var data = await _userService.ValidateUser( context.UserName, context.Password );
                success = data.success;
            }
            catch( Exception ex ) {
                success = false;
            }

            if( success ) {
                var user = await _userService.GetUserByUsername( context.UserName );

                user.LastLoginDate = DateTime.Now;
                await _userService.UpdateUser( user );

                context.Result = new GrantValidationResult( user.Id.ToString(), OidcConstants.AuthenticationMethods.Password );
            };
        }
    }
}
