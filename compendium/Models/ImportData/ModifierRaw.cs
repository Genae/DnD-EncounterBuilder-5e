using System.Xml.Serialization;
using compendium.Models.CoreData.Enums;

namespace compendium.Models.ImportData
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