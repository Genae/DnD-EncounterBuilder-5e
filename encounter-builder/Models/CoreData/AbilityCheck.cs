using encounter_builder.Models.CoreData.Enums;

namespace encounter_builder.Models.CoreData
{
    public class AbilityCheck : ICheck
    {
        public string Ability { get; set; }
        public int Value { get; set; }

        public override bool Equals(object obj)
        {
            return obj is AbilityCheck check &&
                   Ability == check.Ability &&
                   Value == check.Value;
        }
    }
}