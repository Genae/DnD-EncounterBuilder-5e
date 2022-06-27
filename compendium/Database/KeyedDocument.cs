using System.Xml.Serialization;
using Compendium;
using LiteDB;
using Newtonsoft.Json;

namespace Compendium.Database
{
    public class KeyedDocument
    {
        [XmlIgnore]
        [JsonConverter(typeof(ObjectIdConverter))]
        public ObjectId Id { get; set; }
    }
}