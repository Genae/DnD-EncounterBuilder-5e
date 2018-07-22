using System.Xml.Serialization;

namespace encounter_builder.Models
{
    public class ReactionRaw
    {
        [XmlElement("name")]
        public string Name;
        [XmlElement("text")]
        public string Text;
        [XmlElement("attack")]
        public AttackRaw Attack;
    }
}