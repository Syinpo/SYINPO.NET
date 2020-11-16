using System;
using System.Collections.Generic;
using System.Text;

namespace Syinpo.Core.Extensions
{
    public static class StringExtension
    {
        public static string WithMaxLength( this string value, int maxLength ) {
            return value?.Substring( 0, Math.Min( value.Length, maxLength ) );
        }

        public static int GetDeterministicHashCode(this string s)
        {
            unchecked
            {
                int hash1 = (5381 << 16) + 5381;
                int hash2 = hash1;

                for (int i = 0; i < s.Length; i += 2)
                {
                    hash1 = ((hash1 << 5) + hash1) ^ s[i];
                    if (i == s.Length - 1)
                        break;
                    hash2 = ((hash2 << 5) + hash2) ^ s[i + 1];
                }

                return hash1 + (hash2 * 1566083941);
            }
        }

        public static string CreateMD5(this string s)
        {
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                var encoding = Encoding.UTF8;
                var data = encoding.GetBytes(s);

                Span<byte> hashBytes = stackalloc byte[16];
                md5.TryComputeHash(data, hashBytes, out int written);
                if (written != hashBytes.Length)
                    throw new OverflowException();


                Span<char> stringBuffer = stackalloc char[32];
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    hashBytes[i].TryFormat(stringBuffer.Slice(2 * i), out _, "x2");
                }
                return new string(stringBuffer);
            }
        }
    }
}
