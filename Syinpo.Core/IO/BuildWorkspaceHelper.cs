using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Syinpo.Core.IO {
    public class BuildWorkspaceHelper {
        public static string GetCurrentSolutionPath() {
            string relealsePaht = "";
            var rootFolder = string.IsNullOrEmpty( relealsePaht ) ? Directory.GetCurrentDirectory() : relealsePaht;

            if( rootFolder.Contains( @"\Syinpo\" ) ) {
                rootFolder = rootFolder.Substring( 0,
                    rootFolder.LastIndexOf( @"\Syinpo\", StringComparison.Ordinal ) + @"\Syinpo\".Length );
            }
            else if( rootFolder.Contains( @"\Syinpo" ) ) {
                rootFolder = rootFolder.Substring( 0,
                    rootFolder.LastIndexOf( @"\Syinpo", StringComparison.Ordinal )  );
            }

            return rootFolder;
        }


        public static string GetRelativeWorkspacePath( string path ) {
            return Path.GetFullPath( Path.Combine( GetCurrentSolutionPath(), path ) );
        }
    }
}
