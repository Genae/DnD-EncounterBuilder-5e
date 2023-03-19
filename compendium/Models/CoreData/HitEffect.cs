using Compendium.Models.CoreData.Enums;

namespace Compendium.Models.CoreData
{
    public class HitEffect
    {
        public DamageType? DamageType { get; set; }
        public DieRoll? DamageDie { get; set; }
        public ICheck? DC { get; set; }
        public List<Condition>? Condition { get; set; } = new List<Condition>();

        public HitEffect() { }
        public HitEffect(HitEffect hitEffect)
        {
            DamageType = hitEffect.DamageType;
            DamageDie = hitEffect.DamageDie;
            DC = hitEffect.DC;
            Condition = hitEffect.Condition;
        }

        public override bool Equals(object obj)
        {
            return obj is HitEffect effect &&
                   DamageType == effect.DamageType &&
                   EqualityComparer<DieRoll>.Default.Equals(DamageDie, effect.DamageDie) &&
                   EqualityComparer<ICheck>.Default.Equals(DC, effect.DC) &&
                   ((!(Condition?.Any()??false) && !(effect.Condition?.Any()??false)) || Condition.SequenceEqual(effect.Condition));
        }
    }
}