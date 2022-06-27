using System.Xml.Serialization;
using compendiumOld;
using LiteDB;
using Newtonsoft.Json;

namespace compendiumOld.Database
{
    public class KeyedDocument
    {
        [XmlIgnore]
        [JsonConverter(typeof(ObjectIdConverter))]
        public ObjectId Id { get; set; }
    }
}