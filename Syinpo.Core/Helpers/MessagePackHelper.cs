using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using MessagePack;
using MessagePack.Resolvers;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Protocol;

namespace Syinpo.Core.Helpers {
    public class MessagePackHelper {
        public static void ToPack( string fullPath, object data ) {
            var root = new FileInfo( fullPath ).DirectoryName;
            if( !Directory.Exists( root ) ) {
                Directory.CreateDirectory( root );
            }

            using( var file = File.Create( fullPath ) ) {
                var resolver = MessagePack.Resolvers.CompositeResolver.Create(
                    NativeDateTimeResolver.Instance,
                    StandardResolver.Instance
                );
                var options = MessagePackSerializerOptions.Standard.WithResolver( resolver );

                MessagePackSerializer.Serialize( file, data, options );

            }
        }

        public static T ToObject<T>( string fullPath ) {
            using( var file = File.OpenRead( fullPath ) ) {
                var resolver = MessagePack.Resolvers.CompositeResolver.Create(
                    NativeDateTimeResolver.Instance,
                    StandardResolver.Instance
                );
                var options = MessagePackSerializerOptions.Standard.WithResolver( resolver );

                return MessagePackSerializer.Deserialize<T>( file, options );
            }
        }
    }
}
