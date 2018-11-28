using System.Xml.Serialization;
using compendium.Models.CoreData.Enums;

namespace compendium.Models.ImportData
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