using System.Xml.Serialization;
using Compendium.Models.CoreData.Enums;

namespace Compendium.Models.ImportData
{
    public class SkillRaw
    {
        [XmlElement("id")]
        public int SkillId;
        [XmlElement("modifier")]
        public int Modifier;
        public Skill Skill => (Skill)(1 << SkillId);
    }
}