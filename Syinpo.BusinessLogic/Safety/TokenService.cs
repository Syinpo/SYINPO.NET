using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Syinpo.Core.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Syinpo.BusinessLogic.Safety {
    public class TokenService : ITokenService {
        private readonly IConfiguration _configuration;

        public TokenService( IConfiguration configuration ) {
            _configuration = configuration;
        }

        public string GenerateAccessToken( IEnumerable<Claim> claims, int expireInMinutes = 60 ) {
            RsaSecurityKey signingKey = CryptoHelper.CreateRsaSecurityKey();

            var credential = new SigningCredentials( signingKey, "RS256" );

            var jwtToken = new JwtSecurityToken(
                issuer: _configuration.GetValue<string>( "IdentityServer" ),
                audience: "Anyone",
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddMinutes( expireInMinutes ),
                signingCredentials: credential
            );

            return new JwtSecurityTokenHandler().WriteToken( jwtToken );
        }

        public ClaimsPrincipal GetPrincipalFromExpiredToken( string token, out bool expired ) {
            expired = false;

            RsaSecurityKey signingKey = CryptoHelper.CreateRsaSecurityKey();

            var tokenValidationParameters = new TokenValidationParameters {
                ValidateAudience = false,
                ValidateIssuer = true,
                ValidIssuer = _configuration.GetValue<string>( "IdentityServer" ),
                //ValidIssuers = _configuration.GetValue<string>( "IdentityServer" ).Split( ',' ),
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = signingKey,
                ValidateLifetime = false
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken( token, tokenValidationParameters, out SecurityToken securityToken );
            if( !( securityToken is JwtSecurityToken jwtSecurityToken ) || !string.Equals( jwtSecurityToken.Header.Alg, SecurityAlgorithms.RsaSha256, StringComparison.InvariantCultureIgnoreCase ) ) {
                return null;
            }


            DateTime? expires = new DateTime?( securityToken.ValidTo );
            DateTime? notBefore = new DateTime?( securityToken.ValidFrom );
            if( expires.HasValue && expires.Value < DateTime.UtcNow ) {
                expired = true;
            }
            else {
                expired = false;
            }


            return principal;
        }
    }
}
