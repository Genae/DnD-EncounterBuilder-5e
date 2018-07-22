using System.Xml.Serialization;

namespace encounter_builder.Models.ImportData
{
    public class SkillRaw
    {
        [XmlElement("id")]
        public int SkillId;
        [XmlElement("modifier")]
        public int Modifier;
        public Skill Skill => (Skill)SkillId;
    }
}