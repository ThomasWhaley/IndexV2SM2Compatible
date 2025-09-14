using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Xml.Linq;

namespace SaberTools.CrashReporter
{
    public class SslDmp
    {
        public string protocol { get; set; }
        public string spdb_guid { get; set; }
        public List<SslCallStackItem> callstack { get; set; }

        [JsonConverter(typeof(SslObjectsConverter))]
        public Dictionary<string, Dictionary<string, SslValueAbstract>> objects;

    }
    public class SslCallStackItem
    {
        [JsonProperty(PropertyName = "class")]
        public string _class { get; set; }
        public string func { get; set; }
        public Dictionary<string, string> uids { get; set; }
        public string funcType { get; set; }
        [JsonProperty(PropertyName = "this")]
        public SslValuePtr _this;
        public List<SslArg> args { get; set; }
    }
    public class SslArg
    {
        public int offset { get; set; }
        public string name { get; set; }
        [JsonConverter(typeof(SslValueConverter))]
        public SslValueAbstract value { get; set; }

    }

    public abstract class SslValueAbstract
    {

    }
    public class SslValueString : SslValueAbstract
    {
        public string value { get; set; }
    }

    public class SslValuePtr : SslValueAbstract
    {
        public string id { get; set; }
    }
    public class SslValueArray : SslValueAbstract
    {
        public List<SslValueAbstract> array { get; set; }
    }
    public class SslValueDictionary: SslValueAbstract
    {
        public Dictionary<string, SslValueAbstract> dict { get; set; }
    }
    public class SslValueConverter : JsonConverter<SslValueAbstract>
    {
        public override SslValueAbstract? ReadJson(JsonReader reader, Type objectType, SslValueAbstract? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var token = JToken.Load(reader);
            if (token.Type == JTokenType.String)
            {
                var result = new SslValueString();
                result.value = token.ToString();
                return result;
            }
            else if (token.Type == JTokenType.Object)
            {
                var obj = (JObject)token;
                if (obj.ContainsKey("id") && obj["id"].ToString().StartsWith("0x"))
                {
                    var result = new SslValuePtr();
                    result.id = obj["id"]?.ToString();
                    return result;
                } else
                {
                    var dict = new Dictionary<string, SslValueAbstract>();
                    foreach (var item in obj)
                    {
                        var value = item.Value.ToObject<SslValueAbstract>(new JsonSerializer
                        {
                            Converters = { new SslValueConverter() }
                        });
                        dict.Add(item.Key, value);
                    }
                    var result = new SslValueDictionary();
                    result.dict = dict;
                    return result;
                }

            } else if (token.Type == JTokenType.Array) 
            {
                var array = new List<SslValueAbstract>();
                var obj = (JArray)token;
                foreach (var element in obj)
                {
                    var value = element.ToObject<SslValueAbstract>(new JsonSerializer
                    {
                        Converters = { new SslValueConverter() }
                    });
                    array.Add(value);
                }
                var sslArray = new SslValueArray();
                sslArray.array = array;
                return sslArray;
            } else
            {
                var result = new SslValueString();
                result.value = token.ToString();
                return result;
            }

            return null;
        }

        public override void WriteJson(JsonWriter writer, SslValueAbstract? value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
    public class SslObjectsConverter : JsonConverter<Dictionary<string, Dictionary<string, SslValueAbstract>>>
    {
        public override Dictionary<string, Dictionary<string, SslValueAbstract>>? ReadJson(JsonReader reader, Type objectType, Dictionary<string, Dictionary<string, SslValueAbstract>>? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var objects = new Dictionary<string, Dictionary<string, SslValueAbstract>>();

            var obj = JObject.Load(reader);

            foreach (var outerProp in obj.Properties())
            {
                var sslObject = new Dictionary<string, SslValueAbstract>();

                foreach (var sslField in (JObject)outerProp.Value)
                {
                    var value = sslField.Value.ToObject<SslValueAbstract>(new JsonSerializer
                    {
                        Converters = { new SslValueConverter() }
                    });
                    sslObject[sslField.Key] = value;
                }

                objects[outerProp.Name] = sslObject;
            }

            return objects;
        }

        public override void WriteJson(JsonWriter writer, Dictionary<string, Dictionary<string, SslValueAbstract>>? value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
