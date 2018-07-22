using System.Xml.Serialization;

namespace encounter_builder.Models
{
    public class FeatRaw
    {
        [XmlElement("name")]
        public string Name;
        [XmlElement("text")]
        public string Text;
        [XmlElement("prereq")]
        public string Prereq;
    }
}
