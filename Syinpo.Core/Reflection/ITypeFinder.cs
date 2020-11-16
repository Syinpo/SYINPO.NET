using System;
using System.Collections.Generic;

namespace Syinpo.Core.Reflection
{
    public interface ITypeFinder
    {
        Type[] Find(Func<Type, bool> predicate);

        List<Type> Find<T>();

        Type[] FindAll();
    }
}