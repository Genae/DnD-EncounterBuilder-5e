using System.Xml.Serialization;
using encounter_builder.Models.CoreData.Enums;

namespace encounter_builder.Models.ImportData
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