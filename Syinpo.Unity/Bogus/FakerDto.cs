using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using AutoBogus;
using Bogus;
using Syinpo.Core;
using Syinpo.Core.Helpers;

namespace Syinpo.Unity.Bogus {
    public class FakerDto : IFaker {

        public object Generate( Type type ) {
            return AutoFaker.Generate( type );
        }

        public void GenerateJsonFiles( string rootPath ) {
            var model = AppDomain.CurrentDomain.Load( "Syinpo.Model" );
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            var types = model.GetTypes().Where( w => w.Namespace.Contains( "Syinpo.Model.Dto" ) || w.Namespace.Contains( "Syinpo.Model.Core" ) || w.Namespace.Contains( "Syinpo.Model.ViewResult" ) || w.Namespace.Contains( "Syinpo.Model.Request" ) ).ToList();

            int i = 0;
            foreach( var type in types ) {
                try {
                    if( type.FullName == "Syinpo.Model.ViewResult.Safety.DeviceEventProcessResult" || type.FullName == "Syinpo.Model.Core.WeChat.Processor.WeChatMsgParameters" || type.FullName == "Syinpo.Model.Core.WeChat.Processor.WeChatMsgResult") {
                        continue;
                    }

                    Console.WriteLine("{0}: {1}", i, type.FullName);

                    var filename = type.FullName + ".json";
                    var path = Path.Combine( rootPath, filename );

                    var text = JsonHelper.ToJson( AutoFaker.Generate( type ) );

                    File.WriteAllText( path, text );

                }
                catch( Exception ex ) {
                    Console.WriteLine( ex.Message );
                }
                finally {
                    i++;
                }
            }
        }
    }
}
