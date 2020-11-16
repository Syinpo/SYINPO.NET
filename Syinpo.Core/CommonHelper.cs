using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;

namespace Syinpo.Core {
    public class CommonHelper {
        /// <summary>
        /// 唯一Guid
        /// </summary>
        /// <returns></returns>
        public static Guid NewSequentialGuid() {
            byte[] uid = Guid.NewGuid().ToByteArray();
            byte[] binDate = BitConverter.GetBytes( DateTime.UtcNow.Ticks );

            byte[] secuentialGuid = new byte[ uid.Length ];

            secuentialGuid[ 0 ] = uid[ 0 ];
            secuentialGuid[ 1 ] = uid[ 1 ];
            secuentialGuid[ 2 ] = uid[ 2 ];
            secuentialGuid[ 3 ] = uid[ 3 ];
            secuentialGuid[ 4 ] = uid[ 4 ];
            secuentialGuid[ 5 ] = uid[ 5 ];
            secuentialGuid[ 6 ] = uid[ 6 ];

            secuentialGuid[ 7 ] = (byte)( 0xc0 | ( 0xf & uid[ 7 ] ) );

            secuentialGuid[ 9 ] = binDate[ 0 ];
            secuentialGuid[ 8 ] = binDate[ 1 ];
            secuentialGuid[ 15 ] = binDate[ 2 ];
            secuentialGuid[ 14 ] = binDate[ 3 ];
            secuentialGuid[ 13 ] = binDate[ 4 ];
            secuentialGuid[ 12 ] = binDate[ 5 ];
            secuentialGuid[ 11 ] = binDate[ 6 ];
            secuentialGuid[ 10 ] = binDate[ 7 ];

            return new Guid( secuentialGuid );
        }

        public static string MapPath( string path ) {
            var env = IoC.Resolve<IWebHostEnvironment>();

            var webPath = ( env != null ? ( env.ContentRootPath ) : Directory.GetCurrentDirectory() ) ?? string.Empty;
            if( File.Exists( webPath ) )
                webPath = Path.GetDirectoryName( webPath );

            path = path.Replace( "~/", string.Empty ).TrimStart( '/' ).Replace( '/', '\\' );
            var filePath = Path.Combine( webPath ?? string.Empty, path );

            return filePath;
        }


        public static string GetWebHost() {
            var httpContextAccessor = IoC.Resolve<IHttpContextAccessor>();
            var hostHeader = httpContextAccessor.HttpContext.Request.Headers[ HeaderNames.Host ];
            if( StringValues.IsNullOrEmpty( hostHeader ) )
                return string.Empty;

            var isHttps = httpContextAccessor.HttpContext.Request.IsHttps;

            var host = $"{ ( isHttps ? Uri.UriSchemeHttps : Uri.UriSchemeHttp ) }{Uri.SchemeDelimiter}{hostHeader.FirstOrDefault()}";
            host = $"{host.TrimEnd( '/' )}/";

            return host;
        }

        public static string GetWebLocation() {
            var httpContextAccessor = IoC.Resolve<IHttpContextAccessor>();

            var location = string.Empty;

            var host = GetWebHost();
            if( !string.IsNullOrEmpty( host ) ) {
                location = $"{host.TrimEnd( '/' )}{httpContextAccessor.HttpContext.Request.PathBase}";
            }

            location = $"{location.TrimEnd( '/' )}/";
            return location;
        }

        public static string GetClientIpAddress() {
            try {
                var httpContextAccessor = IoC.Resolve<IHttpContextAccessor>();
                var httpContext = httpContextAccessor.HttpContext;
                return httpContext?.Connection?.RemoteIpAddress?.ToString();
            }
            catch( Exception ex ) {

            }

            return null;
        }

        public static int GetClientIpPort() {
            try {
                var httpContextAccessor = IoC.Resolve<IHttpContextAccessor>();
                var httpContext = httpContextAccessor.HttpContext;
                var result =  httpContext?.Connection?.RemotePort ;
                if (result == null)
                    return 0;
                return result.Value;
            }
            catch( Exception ex ) {

            }

            return 0;
        }

        public static string GetServerName() {
            //
            //return Environment.MachineName;
            return  System.Net.Dns.GetHostName();
        }

        public static string DisplayFileSize( long numBytes ) {
            string[] sizes = { "B", "KB", "MB", "GB" };
            int order = 0;
            while( numBytes >= 1024 && order + 1 < sizes.Length ) {
                order++;
                numBytes = numBytes / 1024;
            }
            string result = String.Format( "{0:0.##} {1}", numBytes, sizes[ order ] );
            return result;
        }

        public static string RandomString( int size, bool includeNumbers = false ) {
            Random random = new Random( (int)DateTime.Now.Ticks );

            StringBuilder builder = new StringBuilder( size );
            char ch;
            int num;
            for( int i = 0; i < size; i++ ) {
                if( includeNumbers )
                    num = Convert.ToInt32( Math.Floor( 62 * random.NextDouble() ) );
                else
                    num = Convert.ToInt32( Math.Floor( 52 * random.NextDouble() ) );
                if( num < 26 )
                    ch = Convert.ToChar( num + 65 );
                // lower case
                else if( num > 25 && num < 52 )
                    ch = Convert.ToChar( num - 26 + 97 );
                // numbers
                else
                    ch = Convert.ToChar( num - 52 + 48 );
                builder.Append( ch );
            }
            return builder.ToString();
        }

        public static string RandomNumber(int length)
        {
            var random = new Random();
            var str = string.Empty;
            for (var i = 0; i < length; i++)
                str = string.Concat(str, random.Next(10).ToString());
            return str;
        }

        public static string RandomString(int length, char[] chars)
        {
            if (chars == null || chars.Length == 0)
                chars = "1234567890abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();

            RNGCryptoServiceProvider rand = new RNGCryptoServiceProvider();
            var s = new StringBuilder(length);
            for (int i = 0; i < length; i++)
            {
                byte[] intBytes = new byte[4];
                rand.GetBytes(intBytes);
                uint randomInt = BitConverter.ToUInt32(intBytes, 0);
                s.Append(chars[randomInt % chars.Length]);
            }
            return s.ToString();
        }

        public static T ConvertTo<T>( object input ) {
            try {
                var converter = TypeDescriptor.GetConverter( typeof( T ) );
                if (converter != null)
                {
                    if (typeof(T) == typeof(int) && input.ToString().IndexOf('.') > 0)
                        return (T)converter.ConvertFromString(input.ToString().Remove(input.ToString().IndexOf('.')));

                    return (T)converter.ConvertFromString(input.ToString());
                }
                return default( T );
            }
            catch( Exception ) {
                return default( T );
            }
        }
    }
}
