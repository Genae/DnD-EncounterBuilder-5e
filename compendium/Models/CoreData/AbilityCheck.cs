using Compendium.Models.CoreData.Enums;

namespace Compendium.Models.CoreData
{
    public class AbilityCheck : ICheck
    {
        public Ability Ability { get; set; }
        public int Value { get; set; }

        public override bool Equals(object obj)
        {
            return obj is AbilityCheck check &&
                   Ability == check.Ability &&
                   Value == check.Value;
        }
    }
}