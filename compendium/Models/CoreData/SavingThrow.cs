using compendium.Models.CoreData.Enums;

namespace compendium.Models.CoreData
{
    public class SavingThrow : ICheck
    {
        public Ability Ability { get; set; }
        public int Value { get; set; }

        public override bool Equals(object obj)
        {
            return obj is SavingThrow @throw &&
                   Ability == @throw.Ability &&
                   Value == @throw.Value;
        }
    }
}