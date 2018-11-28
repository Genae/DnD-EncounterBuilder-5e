using System.Xml.Serialization;
using LiteDB;
using Newtonsoft.Json;

namespace compendium.Database
{
    public class KeyedDocument
    {
        [XmlIgnore]
        [JsonConverter(typeof(ObjectIdConverter))]
        public ObjectId Id { get; set; }
    }
}