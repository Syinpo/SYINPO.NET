using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Syinpo.Core.Reflection {
    public class TypeFinder : ITypeFinder {

        private readonly IAssemblyFinder _assemblyFinder;
        private readonly object _syncObj = new object();
        private Type[] _types;

        public TypeFinder( IAssemblyFinder assemblyFinder ) {
            _assemblyFinder = assemblyFinder;
        }

        public Type[] Find( Func<Type, bool> predicate ) {
            return GetAllTypes().Where( predicate ).ToArray();
        }

        public List<Type> Find<T>() {
            var result = new List<Type>();

            var typeFrom = typeof( T );
            foreach( var t in GetAllTypes() ) {
                if( !typeFrom.IsAssignableFrom( t ) && ( !typeFrom.IsGenericTypeDefinition ) )
                    continue;

                if( t.IsClass && !t.IsAbstract ) {
                    result.Add( t );
                }
            }

            return result;
        }


        public Type[] FindAll() {
            return GetAllTypes().ToArray();
        }

        private Type[] GetAllTypes() {
            if( _types == null ) {
                lock( _syncObj ) {
                    if( _types == null ) {
                        _types = CreateTypeList().ToArray();
                    }
                }
            }

            return _types;
        }

        private List<Type> CreateTypeList() {
            var allTypes = new List<Type>();

            var assemblies = _assemblyFinder.GetAllAssemblies().Distinct();

            foreach( var assembly in assemblies ) {
                try {
                    Type[] typesInThisAssembly;

                    try {
                        typesInThisAssembly = assembly.GetTypes();
                    }
                    catch( ReflectionTypeLoadException ex ) {
                        typesInThisAssembly = ex.Types;
                    }

                    if( typesInThisAssembly == null || typesInThisAssembly.Length == 0 ) {
                        continue;
                    }

                    allTypes.AddRange( typesInThisAssembly.Where( type => type != null ) );
                }
                catch( Exception ex ) {
                    //Logger.Warn(ex.ToString(), ex);
                }
            }

            return allTypes;
        }
    }
}