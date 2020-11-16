using System;
using System.Collections.Generic;
using System.Text;

namespace Syinpo.Core.Helpers {
    // EnumParser<YourEnumType>.Parse("EnumValue");
    public static class EnumParser<T> where T : struct {
        private static readonly Dictionary<string, T> _dictionary = new Dictionary<string, T>();

        static EnumParser() {
            if( !typeof( T ).IsEnum )
                throw new NotSupportedException( "Type " + typeof( T ).FullName + " is not an enum." );

            string[] names = Enum.GetNames( typeof( T ) );
            T[] values = (T[])Enum.GetValues( typeof( T ) );

            int count = names.Length;
            for( int i = 0; i < count; i++ )
                _dictionary.Add( names[ i ], values[ i ] );
        }

        public static bool TryParse( string name, out T value ) {
            return _dictionary.TryGetValue( name, out value );
        }

        public static bool TryParse( int vale, out T value ) {
            Enum.TryParse( vale.ToString(), out value );
            return Enum.IsDefined( typeof( T ), value );
        }
        public static bool TryParse( long vale, out T value ) {
            Enum.TryParse( vale.ToString(), out value );
            return Enum.IsDefined( typeof( T ), value );
        }

        public static T Parse( string name ) {
            return _dictionary[ name ];
        }
    }
}
