using System.Xml.Serialization;

namespace encounter_builder.Models
{
    public class ModifierRaw
    {
        [XmlElement("type")]
        public int StatId;
        [XmlElement("value")]
        public int Value;

        public ModifierStat Stat => (ModifierStat) StatId;
    }
}