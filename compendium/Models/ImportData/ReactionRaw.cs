using System.Xml.Serialization;

namespace compendium.Models.ImportData
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