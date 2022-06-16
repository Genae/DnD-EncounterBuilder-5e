using System;
using System.Linq;
using LiteDB;
using Newtonsoft.Json;
using JsonSerializer = Newtonsoft.Json.JsonSerializer;

namespace compendium
{
    public class FlagConverter : JsonConverter
    {
        public override object ReadJson(Newtonsoft.Json.JsonReader reader, Type objectType, Object existingValue, JsonSerializer serializer)
        {
            //If you need to deserialize, fill in the code here
            return null;
        }

        public override void WriteJson(Newtonsoft.Json.JsonWriter writer, Object value, JsonSerializer serializer)
        {
            var flags = value.ToString()
                .Split(new[] { ", " }, StringSplitOptions.RemoveEmptyEntries)
                .Select(f => $"\"{f}\"");

            writer.WriteRawValue($"[{string.Join(", ", flags)}]");
        }

        public override bool CanConvert(Type objectType)
        {
            return true;
        }
    }

    public class ObjectIdConverter : JsonConverter
    {
        public override void WriteJson(Newtonsoft.Json.JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value.ToString());
        }

        public override object ReadJson(Newtonsoft.Json.JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return new ObjectId((string)existingValue);
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(ObjectId).IsAssignableFrom(objectType);
        }
    }
}