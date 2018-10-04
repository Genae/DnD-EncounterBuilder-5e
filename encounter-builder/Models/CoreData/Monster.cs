using encounter_builder.Database;
using System.Collections.Generic;
using System.Linq;
using StackExchange.Redis;

namespace encounter_builder.Models.CoreData
{
    public class Monster : KeyedDocument
    {
        public string Name { get; set; }
        public Size Size { get; set; }
        public MonsterRace Type { get; set; }
        public AlignmentDistribution Alignment { get; set; }
        public string Armor { get; set; }
        public int Armorclass { get; set; }
        public int MaximumHitpoints { get; set; }
        public Speed Speed { get; set; }
        public DamageType[] Resist { get; set; }
        public DamageType[] Vulnerable { get; set; }
        public DamageType[] Immune { get; set; }
        public Condition[] ConditionImmune { get; set; }
        public Senses Senses { get; set; }
        public string Languages { get; set; }
        public Dictionary<Ability, AbilityScore> Abilities { get; set; }
        public DieRoll HitDie { get; set; }
        public ChallengeRating ChallengeRating { get; set; }
        public Spellcasting Spellcasting { get; set; }

        public Dictionary<Ability, int> SavingThrows { get; set; }
        public Dictionary<Skill, int> Skillmodifiers { get; set; }
        public List<Trait> Traits { get; set; }
        public List<Action> Actions { get; set; }
        public List<Reaction> Reactions { get; set; }
        public List<LegendaryAction> LegendaryActions { get; set; }
    }

    public class Action
    {
        public string Name { get; set; }
        public string Text { get; set; }
        public Attack Attack { get; set; }
        public List<HitEffect> HitEffects { get; set; } = new List<HitEffect>();
    }

    public class HitEffect
    {
        public DamageType? DamageType { get; set; }
        public DieRoll DamageDie { get; set; }
        public ICheck DC { get; set; }
        public List<Condition> Condition { get; set; } = new List<Condition>();
        
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
                   (Condition.SequenceEqual(effect.Condition));
        }
    }

    public interface ICheck
    {
        int Value { get; set; }
    }

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

    public enum AttackType
    {
        Melee_Weapon_Attack,
        Ranged_Weapon_Attack,
        Melee_or_Ranged_Weapon_Attack,
        Melee_Spell_Attack,
        Ranged_Spell_Attack,
        Melee_or_Ranged_Spell_Attack
    }
    
    public class Trait
    {
        public string Name { get; set; }
        public string Text { get; set; }
    }

    public class Reaction
    {
        public Action Action { get; set; } //TODO
    }

    public class LegendaryAction
    {
        public Action Action { get; set; } //TODO
    }

    public class Senses
    {
        public int PassivePerception { get; set; }
        public bool BlindOutsideRange { get; set; }
        public Dictionary<Sense, int> SenseRanges { get; set; } = new Dictionary<Sense, int>();
        public string Description { get; set; }
    }

    public enum Sense
    {
        Blindsight,
        Darkvision,
        Tremorsense,
        Truesight
    }

    public class MonsterRace
    {
        public MonsterType Type;
        public string Tags;
    }

    public enum MonsterType
    {
        Aberration,
        Beast,
        Celestial,
        Construct,
        Dragon,
        Elemental,
        Fey,
        Fiend,
        Giant,
        Humanoid,
        Monstrosity,
        Ooze,
        Plant,
        Undead
    }                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                             

    public class AlignmentDistribution
    {
        public List<AlignmentChance> AlignmentChances { get; set; }
        public string Description { get; set; }

        public AlignmentDistribution()
        {
            AlignmentChances = new List<AlignmentChance>();
        }

        public AlignmentDistribution(Alignment alignment, string description)
        {
            AlignmentChances = new List<AlignmentChance>
            {
                new AlignmentChance(alignment, 1)
            };
            Description = description;
        }

        public static AlignmentDistribution Any(string description)
        {
            return new AlignmentDistribution
            {
                AlignmentChances = new List<AlignmentChance>
                {
                    new AlignmentChance(Morality.Good, Order.Lawful, 1/9f),
                    new AlignmentChance(Morality.Good, Order.Neutral, 1/9f),
                    new AlignmentChance(Morality.Good, Order.Chaotic, 1/9f),
                    new AlignmentChance(Morality.Neutral, Order.Lawful, 1/9f),
                    new AlignmentChance(Morality.Neutral, Order.Neutral, 1/9f),
                    new AlignmentChance(Morality.Neutral, Order.Chaotic, 1/9f),
                    new AlignmentChance(Morality.Evil, Order.Lawful, 1/9f),
                    new AlignmentChance(Morality.Evil, Order.Neutral, 1/9f),
                    new AlignmentChance(Morality.Evil, Order.Chaotic, 1/9f),
                },
                Description = description
            };
        }

        public static AlignmentDistribution Unaligned(string description)
        {
            return new AlignmentDistribution
            {
                AlignmentChances = new List<AlignmentChance>(),
                Description = description
            };
        }

        public static AlignmentDistribution Any(string description, Morality morality, float multiplier = 1f)
        {
            return new AlignmentDistribution
            {
                AlignmentChances = new List<AlignmentChance>()
                {
                    new AlignmentChance(morality, Order.Lawful, 1/3f * multiplier),
                    new AlignmentChance(morality, Order.Neutral, 1/3f * multiplier),
                    new AlignmentChance(morality, Order.Chaotic, 1/3f * multiplier)
                },
                Description = description
            };
        }
    }

    public class AlignmentChance
    {
        public Alignment Alignment { get; set; }
        public float Chance { get; set; }

        public AlignmentChance() { }

        public AlignmentChance(Alignment alignment, float chance)
        {
            Alignment = alignment;
            Chance = chance;
        }

        public AlignmentChance(Morality morality, Order order, float chance)
        {
            Alignment = new Alignment(morality, order);
            Chance = chance;
        }
    }

    public class Alignment
    {
        public Morality Morality { get; set; } 
        public Order Order { get; set; } 

        public Alignment() { }

        public Alignment(Morality morality, Order order)
        {
            Morality = morality;
            Order = order;
        }
    }

    public enum Morality
    {
        Good,
        Neutral,
        Evil
    }

    public enum Order
    {
        Lawful,
        Neutral,
        Chaotic
    }

    public enum Condition
    {
        Blinded,
        Charmed,
        Deafened,
        Fatigued,
        Frightened,
        Grappled,
        Incapacitated,
        Invisible,
        Paralyzed,
        Petrified,
        Poisoned,
        Prone,
        Restrained,
        Stunned,
        Unconscious,
        Exhaustion
    }

    public class Speed
    {
        public Dictionary<MovementType, int> Speeds { get; set; } = new Dictionary<MovementType, int>();
        public string AdditionalInformation { get; set; }
    }

    public enum MovementType
    {
        Normal,
        Fly,
        Hover,
        Swim,
        Climb,
        Burrow
    }
}
