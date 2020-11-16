using OtpNet;
using System;
using System.Collections.Generic;
using System.Text;

namespace Syinpo.Core.Helpers
{
    public class OtpHelper
    {
        private static readonly byte[] rfcSecret = new byte[] {
            0x61, 0x62, 0x63, 0x64, 0x65, 0x66, 0x67, 0x68,
            0x69, 0x60, 0x61, 0x62, 0x63, 0x64, 0x65, 0x66,
            0x67, 0x68, 0x69, 0x60
            };
        public static string GenerateTotpCode(int step = 30)
        {
            var otp = new Totp(rfcSecret, step);
            return  otp.ComputeTotp();
        }

        public static bool VerifyTotpCode(string totpCode, int step = 30)
        {
            var otp = new Totp(rfcSecret, step);
            return otp.VerifyTotp(totpCode, out long timeStepMatched);
        }
        public static string GenerateHotpCode(long counter)
        {
            var otp = new Hotp(rfcSecret);
            return otp.ComputeHOTP(counter);
        }

        public static bool VerifyHotpCode(string hotpCode, long counter)
        {
            var otp = new Hotp(rfcSecret);
            return otp.VerifyHotp(hotpCode, counter);
        }

        public static long NextLong(long min = long.MinValue, long max = long.MaxValue)
        {
            Random R = new Random();
            long myResult = min;
            long Max = max, Min = min;
            if (min > max)
            {
                Max = min;
                Min = max;
            }
            double Key = R.Next();
            myResult = Min + (long)((Max - Min) * Key);
            return myResult;
        }

    }
}
