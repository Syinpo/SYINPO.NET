using System.Collections.Generic;
using System.Security.Claims;

namespace Syinpo.BusinessLogic.Safety
{
    public interface ITokenService
    {
        string GenerateAccessToken( IEnumerable<Claim> claims, int expireInMinutes = 60 );

        ClaimsPrincipal GetPrincipalFromExpiredToken(string token, out bool expired );
    }
}