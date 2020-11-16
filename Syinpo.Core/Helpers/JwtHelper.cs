using System;
using System.Collections.Generic;
using JWT;
using JWT.Algorithms;
using JWT.Builder;
using JWT.Serializers;
using Newtonsoft.Json;

namespace Syinpo.Core.Helpers
{
    public class JwtHelper
    {
        public T Decode<T>(string token, string secret) {
            var serializer = new JsonNetSerializer();
            var base64 = new JwtBase64UrlEncoder();
            var provier = new UtcDateTimeProvider();
            var validator = new JwtValidator( serializer, provier );

            var payload = new JwtBuilder()
                .WithSerializer( serializer )
                .WithValidator( validator )
                .WithUrlEncoder( base64 )
                .WithSecret(secret)
                .MustVerifySignature()
                .Decode<T>(token);

            return payload;
        }

        public string Encode<T>(T data, string secret, out DateTime exp)
        {
            var json = JsonConvert.SerializeObject(data, Formatting.Indented);
            var payload = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);

            var exp2 = DateTimeOffset.UtcNow.AddDays(1);
            exp = exp2.UtcDateTime;
            if( !payload.ContainsKey("exp") )
                payload.Add("exp", exp2.ToUnixTimeSeconds());

            IJwtAlgorithm algorithm = new HMACSHA256Algorithm();
            IJsonSerializer serializer = new JsonNetSerializer();
            IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
            IJwtEncoder encoder = new JwtEncoder(algorithm, serializer, urlEncoder);

            var token = encoder.Encode(payload, secret);

            return token;
        }
    }
}
