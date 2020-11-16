using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Syinpo.Core.Reflection {
    public class AssemblyFinder : IAssemblyFinder {

        public List<Assembly> GetAllAssemblies() {
            var assemblies = new List<Assembly>();

            foreach( var assembly in AppDomain.CurrentDomain.GetAssemblies() ) {
                assemblies.Add( assembly );
            }

            assemblies.AddRange(
                AssemblyHelper.GetAllAssembliesInFolder( AppContext.BaseDirectory,
                    new System.IO.SearchOption { } ) );

            return assemblies.Distinct().ToList();
        }
    }
}