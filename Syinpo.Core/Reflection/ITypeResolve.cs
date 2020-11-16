using System;

namespace Syinpo.Core.Reflection
{
    public interface ITypeResolve
    {
        object Resolve( Type type );
    }
}