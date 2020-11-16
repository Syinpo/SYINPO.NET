using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace Syinpo.ApiDoc.Generation.Utitls
{
    public static class JsonHelper
    {
        public static string ToJson(object obj)
        {
            if( obj == null )
            {
                return "{}";
            }
            else
            {
                var settings = new JsonSerializerSettings()
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                };

                return JsonConvert.SerializeObject(obj, Formatting.Indented, settings);
            }
        }
        public static string ToJson2(object obj)
        {
            if( obj == null )
            {
                return "{}";
            }
            else
            {
                var settings = new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                };

                return JsonConvert.SerializeObject(obj, Formatting.Indented, settings);
            }
        }

        public static T ToObject<T>(string str)
        {
            if( str == null )
                return default(T);
            if( str == "{}" )
                return default(T);
            return JsonConvert.DeserializeObject<T>(str);
        }


        public static bool IsValidJson( string stringValue)
        {
            if( string.IsNullOrWhiteSpace(stringValue) )
            {
                return false;
            }

            var value = stringValue.Trim();

            if( ( value.StartsWith("{") && value.EndsWith("}") ) || //For object
                ( value.StartsWith("[") && value.EndsWith("]") ) ) //For array
            {
                try
                {
                    var obj = JToken.Parse(value);
                    return true;
                }
                catch( JsonReaderException )
                {
                    return false;
                }
            }

            return false;
        }

    }
}
