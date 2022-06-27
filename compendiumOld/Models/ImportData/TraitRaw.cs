using System.Xml.Serialization;

namespace compendiumOld.Models.ImportData
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