using System;
using System.Collections.Generic;
using System.Text;

namespace Syinpo.Core {
    public interface IFaker {
        object Generate( Type type );

        void GenerateJsonFiles( string rootPath );
    }
}
