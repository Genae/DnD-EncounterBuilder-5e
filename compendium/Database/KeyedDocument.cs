using LiteDB;
using Newtonsoft.Json;
using System.Xml.Serialization;

namespace Compendium.Database
{
    public class KeyedDocument
    {
        [XmlIgnore]
        [JsonConverter(typeof(ObjectIdConverter))]
        public ObjectId Id { get; set; }
    }

    public class ProjectKeyedDocument : KeyedDocument
    {
        public List<ObjectId>? ProjectTags { get; set; } = new();
    }
}