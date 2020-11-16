﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace Syinpo.Core.Helpers {
    public static class CryptoHelper {
        private static RsaSecurityKey _key;

        public static RsaSecurityKey CreateRsaSecurityKey() {
            if( _key == null ) {
                string pathToKey = CommonHelper.MapPath( $"~/App_Data/syinpo-apikey.rsa" );

                if( !File.Exists( pathToKey ) ) {
                    // generate random parameters
                    var randomParameters = GetRandomParameters();

                    var rsaParams = new RSAParametersWithPrivate();
                    rsaParams.SetParameters( randomParameters );
                    string serializedParameters = JsonConvert.SerializeObject( rsaParams );

                    // create file and save the key
                    File.WriteAllText( pathToKey, serializedParameters );
                }

                // load the key
                if( !File.Exists( pathToKey ) )
                    throw new FileNotFoundException( "Check configuration - cannot find auth key file: " + pathToKey );

                var keyParams = JsonConvert.DeserializeObject<RSAParametersWithPrivate>( File.ReadAllText( pathToKey ) );

                // create signing key by the key above
                _key = new RsaSecurityKey( keyParams.ToRSAParameters() );
            }

            return _key;
        }

        public static RSAParameters GetRandomParameters() {
            using( var rsa = new RSACryptoServiceProvider( 2048 ) ) {
                try {
                    return rsa.ExportParameters( true );
                }
                finally {
                    rsa.PersistKeyInCsp = false;
                }
            }
        }

        private class RSAParametersWithPrivate {
            public byte[] D {
                get; set;
            }
            public byte[] DP {
                get; set;
            }
            public byte[] DQ {
                get; set;
            }
            public byte[] Exponent {
                get; set;
            }
            public byte[] InverseQ {
                get; set;
            }
            public byte[] Modulus {
                get; set;
            }
            public byte[] P {
                get; set;
            }
            public byte[] Q {
                get; set;
            }

            public void SetParameters( RSAParameters p ) {
                D = p.D;
                DP = p.DP;
                DQ = p.DQ;
                Exponent = p.Exponent;
                InverseQ = p.InverseQ;
                Modulus = p.Modulus;
                P = p.P;
                Q = p.Q;
            }
            public RSAParameters ToRSAParameters() {
                return new RSAParameters() {
                    D = this.D,
                    DP = this.DP,
                    DQ = this.DQ,
                    Exponent = this.Exponent,
                    InverseQ = this.InverseQ,
                    Modulus = this.Modulus,
                    P = this.P,
                    Q = this.Q

                };
            }
        }
    }
}
