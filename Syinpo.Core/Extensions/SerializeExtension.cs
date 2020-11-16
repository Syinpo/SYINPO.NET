using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using System.Text.Json;

namespace Syinpo.Core.Extensions
{
    public static class SerializeExtension
    {
        public static T XmlDeserialize<T>(this string toDeserialize)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
            using (StringReader textReader = new StringReader(toDeserialize))
            {
                return (T)xmlSerializer.Deserialize(textReader);
            }
        }

        public static string XmlSerialize<T>(this T toSerialize)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
            using (StringWriter textWriter = new StringWriter())
            {
                xmlSerializer.Serialize(textWriter, toSerialize);
                return textWriter.ToString();
            }
        }

        public static T JsonDeserialize<T>(this string toDeserialize)
        {
            return JsonSerializer.Deserialize<T>(toDeserialize,
                    new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    });
        }

        public static string JsonSerialize<T>(this T toSerialize)
        {
            return JsonSerializer.Serialize<T>(toSerialize,
                    new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    });
        }
    }
}
