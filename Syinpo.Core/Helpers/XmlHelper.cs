using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Syinpo.Core.Helpers {
    public static class XmlHelper {
        public static string ToXml<T>( this T model ) {
            if( model == null )
                return string.Empty;

            var xmlSerializer = new XmlSerializer( typeof( T ) );

            using( var stringWriter = new StringWriter() ) {
                using( var xmlWriter = XmlWriter.Create( stringWriter, new XmlWriterSettings { Indent = true } ) ) {
                    xmlSerializer.Serialize( xmlWriter, model );
                    return stringWriter.ToString();
                }
            }
        }

        public static T ToModel<T>( this string xml ) {
            if( string.IsNullOrWhiteSpace( xml ) || !xml.IsValidXml() )
                return default( T );

            var xmlSerializer = new XmlSerializer( typeof( T ) );

            using( var stringReader = new StringReader( xml ) ) {
                using( var xmlTextReader = new XmlTextReader( stringReader ) ) {
                    var result = xmlSerializer.Deserialize( xmlTextReader );
                    return result != null ? (T)Convert.ChangeType( result, typeof( T ) ) : default( T );
                }
            }
        }

        public static bool IsValidXml( this string xml ) {
            try {
                var result = new XmlDocument();
                result.LoadXml( xml );
                return true;
            }
            catch( XmlException ex ) {
                Console.WriteLine( "XML Exception: {0}", ex );
                return false;
            }
        }
    }
}
