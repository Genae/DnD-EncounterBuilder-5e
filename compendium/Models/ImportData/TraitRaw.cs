using System.Xml.Serialization;

namespace compendium.Models.ImportData
{
    public class TraitRaw
    {
        [XmlElement("name")]
        public string Name;
        [XmlElement("text")]
        public string Text;
        [XmlElement("attack")]
        public AttackRaw Attack;
    }
}