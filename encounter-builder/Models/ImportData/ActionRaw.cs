using System.Xml.Serialization;

namespace encounter_builder.Models.ImportData
{
    public class ActionRaw
    {
        [XmlElement("name")]
        public string Name;
        [XmlElement("text")]
        public string Text;
        [XmlElement("attack")]
        public AttackRaw Attack;
    }
}