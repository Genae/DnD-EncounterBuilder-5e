using System.Xml.Serialization;
using compendiumOld.Models.CoreData.Enums;

namespace compendiumOld.Models.ImportData
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