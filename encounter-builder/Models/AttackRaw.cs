using System.Xml.Serialization;

namespace encounter_builder.Models
{
    public class AttackRaw
    {
        [XmlElement("name")]
        public string Name;
        [XmlElement("attackBonus")]
        public int AttackBonus;
        [XmlElement("damage")]
        public string Damage;
    }
}