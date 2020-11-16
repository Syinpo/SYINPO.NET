using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Xml;
using Newtonsoft.Json;

namespace Syinpo.Core.EasyLicense.License {
    /// <summary>
    ///     LicenseGenerator generates and signs license.
    /// </summary>
    public class LicenseGenerator {
        private readonly string privateKey;

        /// <summary>
        ///     Creates a new instance of <seealso cref="LicenseGenerator" /> .
        /// </summary>
        /// <param name="privateKey">private key of the product</param>
        public LicenseGenerator( string privateKey ) {
            this.privateKey = privateKey;
        }

        /// <summary>
        ///     Generates a new license
        /// </summary>
        /// <param name="name">name of the license holder</param>
        /// <param name="id">Id of the license holder</param>
        /// <param name="expirationDate">expiry date</param>
        /// <param name="licenseType">type of the license</param>
        /// <returns></returns>
        public string Generate( string name, Guid id, DateTime expirationDate, LicenseType licenseType ) {
            return Generate( name, id, expirationDate, new Dictionary<string, string>(), licenseType );
        }

        /// <summary>
        ///     Generates a new license
        /// </summary>
        /// <param name="name">name of the license holder</param>
        /// <param name="id">Id of the license holder</param>
        /// <param name="expirationDate">expiry date</param>
        /// <param name="licenseType">type of the license</param>
        /// <param name="attributes">extra information stored as key/valye in the license file</param>
        /// <returns></returns>
        public string Generate( string name, Guid id, DateTime expirationDate, IDictionary<string, string> attributes,
            LicenseType licenseType ) {
            using( var rsa = new RSACryptoServiceProvider() ) {
                rsa.FromXmlString2( privateKey );
                var doc = CreateDocument( id, name, expirationDate, attributes, licenseType );

                var signature = GetXmlDigitalSignature( doc, rsa );
                doc.FirstChild.AppendChild( doc.ImportNode( signature, true ) );

                var ms = new MemoryStream();
                var writer = XmlWriter.Create( ms, new XmlWriterSettings {
                    Indent = true,
                    Encoding = Encoding.UTF8
                } );
                doc.Save( writer );
                ms.Position = 0;
                return new StreamReader( ms ).ReadToEnd();
            }
        }

        /// <summary>
        ///     Generates a new floating license.
        /// </summary>
        /// <param name="name">Name of the license holder</param>
        /// <param name="publicKey">public key of the license server</param>
        /// <returns>license content</returns>
        public string GenerateFloatingLicense( string name, string publicKey ) {
            using( var rsa = new RSACryptoServiceProvider() ) {
                rsa.FromXmlString2( privateKey );
                var doc = new XmlDocument();
                var license = doc.CreateElement( "floating-license" );
                doc.AppendChild( license );

                var publicKeyEl = doc.CreateElement( "license-server-public-key" );
                license.AppendChild( publicKeyEl );
                publicKeyEl.InnerText = publicKey;

                var nameEl = doc.CreateElement( "name" );
                license.AppendChild( nameEl );
                nameEl.InnerText = name;

                var signature = GetXmlDigitalSignature( doc, rsa );
                doc.FirstChild.AppendChild( doc.ImportNode( signature, true ) );

                var ms = new MemoryStream();
                var writer = XmlWriter.Create( ms, new XmlWriterSettings {
                    Indent = true,
                    Encoding = Encoding.UTF8
                } );
                doc.Save( writer );
                ms.Position = 0;
                return new StreamReader( ms ).ReadToEnd();
            }
        }

        private static XmlDocument CreateDocument( Guid id, string name, DateTime expirationDate,
            IDictionary<string, string> attributes, LicenseType licenseType ) {
            var doc = new XmlDocument();
            var license = doc.CreateElement( "license" );
            doc.AppendChild( license );
            var idAttr = doc.CreateAttribute( "id" );
            license.Attributes.Append( idAttr );
            idAttr.Value = id.ToString();

            var expirDateAttr = doc.CreateAttribute( "expiration" );
            license.Attributes.Append( expirDateAttr );
            expirDateAttr.Value = expirationDate.ToString( "yyyy-MM-ddTHH:mm:ss.fffffff", CultureInfo.InvariantCulture );

            var licenseAttr = doc.CreateAttribute( "type" );
            license.Attributes.Append( licenseAttr );
            licenseAttr.Value = licenseType.ToString();

            var nameEl = doc.CreateElement( "name" );
            license.AppendChild( nameEl );
            nameEl.InnerText = name;

            foreach( var attribute in attributes ) {
                var attrib = doc.CreateAttribute( attribute.Key );
                attrib.Value = attribute.Value;
                license.Attributes.Append( attrib );
            }

            return doc;
        }

        private static XmlElement GetXmlDigitalSignature( XmlDocument x, AsymmetricAlgorithm key ) {
            var signedXml = new SignedXml( x ) { SigningKey = key };
            var reference = new Reference { Uri = "" };
            reference.AddTransform( new XmlDsigEnvelopedSignatureTransform() );
            signedXml.AddReference( reference );
            signedXml.ComputeSignature();
            return signedXml.GetXml();
        }

        public static void GenerateLicenseKey( out string privateKey, out string publicKey ) {
            using( var rsa = new RSACryptoServiceProvider() ) {
                privateKey = rsa.ToXmlString2( true );
                publicKey = rsa.ToXmlString2( false );
            }
        }

    }



    internal static class RSAKeyExtensions {

        #region XML

        public static void FromXmlString2( this RSA rsa, string xmlString ) {
            RSAParameters parameters = new RSAParameters();

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml( xmlString );

            if( xmlDoc.DocumentElement.Name.Equals( "RSAKeyValue" ) ) {
                foreach( XmlNode node in xmlDoc.DocumentElement.ChildNodes ) {
                    switch( node.Name ) {
                        case "Modulus":
                            parameters.Modulus = ( string.IsNullOrEmpty( node.InnerText ) ? null : Convert.FromBase64String( node.InnerText ) );
                            break;
                        case "Exponent":
                            parameters.Exponent = ( string.IsNullOrEmpty( node.InnerText ) ? null : Convert.FromBase64String( node.InnerText ) );
                            break;
                        case "P":
                            parameters.P = ( string.IsNullOrEmpty( node.InnerText ) ? null : Convert.FromBase64String( node.InnerText ) );
                            break;
                        case "Q":
                            parameters.Q = ( string.IsNullOrEmpty( node.InnerText ) ? null : Convert.FromBase64String( node.InnerText ) );
                            break;
                        case "DP":
                            parameters.DP = ( string.IsNullOrEmpty( node.InnerText ) ? null : Convert.FromBase64String( node.InnerText ) );
                            break;
                        case "DQ":
                            parameters.DQ = ( string.IsNullOrEmpty( node.InnerText ) ? null : Convert.FromBase64String( node.InnerText ) );
                            break;
                        case "InverseQ":
                            parameters.InverseQ = ( string.IsNullOrEmpty( node.InnerText ) ? null : Convert.FromBase64String( node.InnerText ) );
                            break;
                        case "D":
                            parameters.D = ( string.IsNullOrEmpty( node.InnerText ) ? null : Convert.FromBase64String( node.InnerText ) );
                            break;
                    }
                }
            }
            else {
                throw new System.Exception( "Invalid XML RSA key." );
            }

            rsa.ImportParameters( parameters );
        }

        public static string ToXmlString2( this RSA rsa, bool includePrivateParameters ) {
            RSAParameters parameters = rsa.ExportParameters( includePrivateParameters );

            return string.Format( "<RSAKeyValue><Modulus>{0}</Modulus><Exponent>{1}</Exponent><P>{2}</P><Q>{3}</Q><DP>{4}</DP><DQ>{5}</DQ><InverseQ>{6}</InverseQ><D>{7}</D></RSAKeyValue>",
                  parameters.Modulus != null ? Convert.ToBase64String( parameters.Modulus ) : null,
                  parameters.Exponent != null ? Convert.ToBase64String( parameters.Exponent ) : null,
                  parameters.P != null ? Convert.ToBase64String( parameters.P ) : null,
                  parameters.Q != null ? Convert.ToBase64String( parameters.Q ) : null,
                  parameters.DP != null ? Convert.ToBase64String( parameters.DP ) : null,
                  parameters.DQ != null ? Convert.ToBase64String( parameters.DQ ) : null,
                  parameters.InverseQ != null ? Convert.ToBase64String( parameters.InverseQ ) : null,
                  parameters.D != null ? Convert.ToBase64String( parameters.D ) : null );
        }

        #endregion
    }
}