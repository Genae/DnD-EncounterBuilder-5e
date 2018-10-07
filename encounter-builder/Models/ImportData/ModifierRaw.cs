using System.Xml.Serialization;
using encounter_builder.Models.CoreData.Enums;

namespace encounter_builder.Models.ImportData
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