using compendiumOld.Models.CoreData.Enums;

namespace compendiumOld.Models.CoreData
{
    public class Attack
    {
        public int AttackBonus { get; set; }
        public AttackType Type { get; set; }
        public string Target { get; set; }
        public int Reach { get; set; }
        public int ShortRange { get; set; }
        public int LongRange { get; set; }

        public override bool Equals(object obj)
        {
            return obj is Attack attack &&
                   AttackBonus == attack.AttackBonus &&
                   Type == attack.Type &&
                   Target == attack.Target &&
                   Reach == attack.Reach &&
                   ShortRange == attack.ShortRange &&
                   LongRange == attack.LongRange;
        }
    }
}