using System.Xml.Serialization;
using LiteDB;

namespace encounter_builder.Database
{
    public class KeyedDocument
    {
        [XmlIgnore]
        public ObjectId Id { get; set; }
    }
}