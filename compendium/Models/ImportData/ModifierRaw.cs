using System.Xml.Serialization;
using Compendium.Models.CoreData.Enums;

namespace Compendium.Models.ImportData
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