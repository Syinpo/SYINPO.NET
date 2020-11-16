using System.IO;
using MessagePack;
using MessagePack.Resolvers;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Syinpo.Core.Helpers {
    public static class JsonHelper {
        public static string ToJson(this object obj ) {
            if( obj == null ) {
                return "{}";
            }
            else {
                var settings = new JsonSerializerSettings()
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                };

                return JsonConvert.SerializeObject( obj, Formatting.Indented, settings );
            }
        }

        public static string ToJson2( object obj ) {
            if( obj == null ) {
                return "{}";
            }
            else {
                var settings = new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                };

                return JsonConvert.SerializeObject( obj, Formatting.Indented, settings );
            }
        }

        public static string ToCompactJson2( object obj ) {
            if( obj == null ) {
                return "{}";
            }
            else {
                var settings = new JsonSerializerSettings() {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                };

                return JsonConvert.SerializeObject( obj, Formatting.None, settings );
            }
        }

        public static T ToObject<T>( string str ) {
            if( str == null )
                return default( T );
            return JsonConvert.DeserializeObject<T>( str );
        }

        public static void ToPack( string fullPath, object data ) {
            var root = new FileInfo( fullPath ).DirectoryName;
            if( !Directory.Exists( root ) ) {
                Directory.CreateDirectory( root );
            }

            File.WriteAllText(fullPath, ToJson(data));
        }

        public static T ToData<T>( string fullPath ) {
            var str = File.ReadAllText( fullPath );
            if( string.IsNullOrEmpty( str ) )
                return default( T);

            return ToObject<T>( str);
        }

    }
}
