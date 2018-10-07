using encounter_builder.Models.CoreData.Enums;

namespace encounter_builder.Models.CoreData
{
    public class SkillCheck : ICheck
    {
        public Skill Skill { get; set; }
        public int Value { get; set; }

        public override bool Equals(object obj)
        {
            return obj is SkillCheck check &&
                   Skill == check.Skill &&
                   Value == check.Value;
        }
    }
}