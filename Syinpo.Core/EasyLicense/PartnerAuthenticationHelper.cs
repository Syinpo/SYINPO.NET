using Syinpo.Core.Extensions;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Syinpo.Core.EasyLicense
{
   public class PartnerAuthenticationHelper
    {
        private readonly ITimeLimitedDataProtector _timeLimitedProtector;
        private readonly PasswordHasher<string> _hasher;

        public PartnerAuthenticationHelper(IDataProtectionProvider provider, IOptions<PasswordHasherOptions> passwordHasherOptions)
        {
            _hasher = new PasswordHasher<string>(passwordHasherOptions);
            _timeLimitedProtector = provider.CreateProtector("Syinpo.Core.EasyLicense.PartnerAuthentication.v1").ToTimeLimitedDataProtector();
        }

        public string GenerateAccessKey(AccessKey key)
        {
            return _hasher.HashPassword(key.PId.ToString(), key.JsonSerialize());
        }

        public string GenerateAccessSecret(AccessSecret secret)
        {
           return Convert.ToBase64String(_timeLimitedProtector.Protect(
                Encoding.UTF8.GetBytes(secret.JsonSerialize()),
                new DateTimeOffset(secret.ExpiresTime, TimeSpan.FromHours(8))));
        }

        public bool VerifyAccessKey(AccessKey key, string accessKey)
        {
           var result = _hasher.VerifyHashedPassword(key.PId.ToString(), accessKey, key.JsonSerialize());
            return result != PasswordVerificationResult.Failed;
        }

        public (bool result, string message) VerifyAccessSecret(AccessSecret secret, string accessKey, string accessSecret)
        {
            try
            {
                if (secret.ExpiresTime < DateTime.Now.Date)
                    return (false, "服务时间已到期");

                var deserializeSecret = Encoding.UTF8.GetString(_timeLimitedProtector.Unprotect(Convert.FromBase64String(accessSecret))).JsonDeserialize<AccessSecret>();

                if (deserializeSecret.Key != accessKey)
                    return (false, "AccessKey、AccessSecret异常");

                if (deserializeSecret.Version != secret.Version)
                    return (false, "AccessKey、AccessSecret版本错误");

                return (true, "");
            }
            catch (CryptographicException ex)
            {
                return (false, ex.Message);
            }

        }
    }
}
