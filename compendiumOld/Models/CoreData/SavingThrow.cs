using compendiumOld.Models.CoreData.Enums;

namespace compendiumOld.Models.CoreData
{
    public class SavingThrow : ICheck
    {
        public string Ability { get; set; }
        public int Value { get; set; }

        public override bool Equals(object obj)
        {
            return obj is SavingThrow @throw &&
                   Ability == @throw.Ability &&
                   Value == @throw.Value;
        }
    }
}