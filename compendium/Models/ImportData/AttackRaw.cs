using System.Xml.Serialization;

namespace Compendium.Models.ImportData
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