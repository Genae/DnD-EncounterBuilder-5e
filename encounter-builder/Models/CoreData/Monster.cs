using System;
using System.Collections.Generic;
using encounter_builder.Database;
using LiteDB;

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
        public List<HitEffect> HitEffects { get; set; }
    }

    public class HitEffect
    {
        public DamageType DamageType { get; set; }
        public DieRoll DamageDie { get; set; }
        public AbilityCheck DC { get; set; }
        public Condition Condition { get; set; }
    }

    public class AbilityCheck
    {
        public Ability Ability { get; set; }
        public int Value { get; set; }
        public bool IsSavingThrow { get; set; }
    }

    public class Attack
    {
        public int AttackBonus { get; set; }
        public AttackType Type { get; set; }
        public string Target { get; set; }
        public int Reach { get; set; }
        public int ShortRange { get; set; }
        public int LongRange { get; set; }
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
        
    }

    public class Reaction
    {
        
    }

    public class LegendaryAction
    {
        
    }

    public class Senses
    {
        public int PassivePerception { get; set; }
        public bool BlindOutsideRange { get; set; }
        public Dictionary<Sense, int> SenseRanges;
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
        Monstrositie,
        Ooze,
        Plant,
        Undead
    }                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                             

    public class AlignmentDistribution
    {
        [BsonIgnore]
        public static AlignmentDistribution Any;
        [BsonIgnore]
        public static AlignmentDistribution Unaligned;
        public Dictionary<Alignment, float> AlignmentChances { get; set; }
    }

    public class Alignment
    {
        public Morality Morality { get; set; }
        public Order Order { get; set; }
        public float Chance { get; set; }
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
        public Dictionary<MovementType, int> Speeds { get; set; }
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
