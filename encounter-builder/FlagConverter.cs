using System;
using System.Linq;
using LiteDB;
using Newtonsoft.Json;
using JsonSerializer = Newtonsoft.Json.JsonSerializer;

namespace encounter_builder
{
    public class FlagConverter : JsonConverter
    {
        public override object ReadJson(JsonReader reader, Type objectType, Object existingValue, JsonSerializer serializer)
        {
            //If you need to deserialize, fill in the code here
            return null;
        }

        public override void WriteJson(JsonWriter writer, Object value, JsonSerializer serializer)
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

    public class IdConverter : JsonConverter
    {
        public override object ReadJson(JsonReader reader, Type objectType, Object existingValue, JsonSerializer serializer)
        {
            var val = reader.ReadAsString();
            return new ObjectId(val);
        }

        public override void WriteJson(JsonWriter writer, Object value, JsonSerializer serializer)
        {
            var id = (ObjectId) value;
            writer.WriteRawValue(id.ToString());
        }

        public override bool CanConvert(Type objectType)
        {
            return true;
        }
    }
}