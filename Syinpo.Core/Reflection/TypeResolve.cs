using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Syinpo.Core.Reflection {
   public class TypeResolve : ITypeResolve
   {
        public virtual object Resolve( Type type ) {
            Exception innerException = null;
            foreach( var constructor in type.GetConstructors() ) {
                try {
                    var parameters = constructor.GetParameters().Select( parameter =>
                    {
                        var service = IoC.Resolve( parameter.ParameterType );
                        if( service == null )
                            throw new SysException( "未知注入" );
                        return service;
                    } );

                    return Activator.CreateInstance( type, parameters.ToArray() );
                }
                catch( Exception ex ) {
                    innerException = ex;
                }
            }

            throw new SysException( "TypeResolve 创建类错误", innerException );
        }
    }
}
