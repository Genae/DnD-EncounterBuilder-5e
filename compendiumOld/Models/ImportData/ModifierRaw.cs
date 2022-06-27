using System.Xml.Serialization;
using compendiumOld.Models.CoreData.Enums;

namespace compendiumOld.Models.ImportData
{
    public class ModifierRaw
    {
        [XmlElement("type")]
        public int StatId;
        [XmlElement("value")]
        public int Value;

        public ModifierStat Stat => (ModifierStat)StatId;
    }
}