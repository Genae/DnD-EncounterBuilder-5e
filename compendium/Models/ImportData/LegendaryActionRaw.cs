using System.Xml.Serialization;

namespace Compendium.Models.ImportData
{
    public class LegendaryActionRaw
    {
        [XmlElement("name")]
        public string Name;
        [XmlElement("text")]
        public string Text;
        [XmlElement("attack")]
        public AttackRaw Attack;
    }
}