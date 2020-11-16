using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Syinpo.Core.Helpers {
    public class EncryptionHelper
    {
        private const string encryptionKey = "syinpo123456789";

        /// <summary>
        /// Encrypt text
        /// </summary>
        /// <param name="plainText">Text to encrypt</param>
        /// <param name="encryptionPrivateKey">Encryption private key</param>
        /// <returns>Encrypted text</returns>
        public static string EncryptText( string plainText, string encryptionPrivateKey = "" ) {
            if( string.IsNullOrEmpty( plainText ) )
                return plainText;

            if( string.IsNullOrEmpty( encryptionPrivateKey ) )
                encryptionPrivateKey = encryptionKey;

            using( var provider = new TripleDESCryptoServiceProvider() ) {
                provider.Key = Encoding.ASCII.GetBytes( encryptionPrivateKey.Substring( 0, 16 ) );
                provider.IV = Encoding.ASCII.GetBytes( encryptionPrivateKey.Substring( 8, 8 ) );

                var encryptedBinary = EncryptTextToMemory( plainText, provider.Key, provider.IV );
                return Convert.ToBase64String( encryptedBinary );
            }
        }

        /// <summary>
        /// Decrypt text
        /// </summary>
        /// <param name="cipherText">Text to decrypt</param>
        /// <param name="encryptionPrivateKey">Encryption private key</param>
        /// <returns>Decrypted text</returns>
        public static string DecryptText( string cipherText, string encryptionPrivateKey = "" ) {
            if( string.IsNullOrEmpty( cipherText ) )
                return cipherText;

            if( string.IsNullOrEmpty( encryptionPrivateKey ) )
                encryptionPrivateKey = encryptionKey;

            using( var provider = new TripleDESCryptoServiceProvider() ) {
                provider.Key = Encoding.ASCII.GetBytes( encryptionPrivateKey.Substring( 0, 16 ) );
                provider.IV = Encoding.ASCII.GetBytes( encryptionPrivateKey.Substring( 8, 8 ) );

                var buffer = Convert.FromBase64String( cipherText );
                return DecryptTextFromMemory( buffer, provider.Key, provider.IV );
            }
        }


        #region 公共方法

        private static byte[] EncryptTextToMemory( string data, byte[] key, byte[] iv ) {
            using( var ms = new MemoryStream() ) {
                using( var cs = new CryptoStream( ms, new TripleDESCryptoServiceProvider().CreateEncryptor( key, iv ), CryptoStreamMode.Write ) ) {
                    var toEncrypt = Encoding.Unicode.GetBytes( data );
                    cs.Write( toEncrypt, 0, toEncrypt.Length );
                    cs.FlushFinalBlock();
                }

                return ms.ToArray();
            }
        }

        private static string DecryptTextFromMemory( byte[] data, byte[] key, byte[] iv ) {
            using( var ms = new MemoryStream( data ) ) {
                using( var cs = new CryptoStream( ms, new TripleDESCryptoServiceProvider().CreateDecryptor( key, iv ), CryptoStreamMode.Read ) ) {
                    using( var sr = new StreamReader( cs, Encoding.Unicode ) ) {
                        return sr.ReadToEnd();
                    }
                }
            }
        }

        #endregion
    }
}
