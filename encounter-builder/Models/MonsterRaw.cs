using System.Collections.Generic;
using System.Xml.Serialization;

namespace encounter_builder.Models
{
    public class MonsterRawData
    {
        [XmlElement("name")]
        public string Name;
        [XmlElement("size")]
        public int SizeId;
        [XmlElement("type")]
        public string Type;
        [XmlElement("alignment")]
        public string Alignment;
        [XmlElement("ac")]
        public int Armorclass;
        [XmlElement("hpMax")]
        public int MaximumHitpoints;
        [XmlElement("speed")]
        public string Speed;
        [XmlElement("abilities")]
        public string Abilities;
        [XmlElement("resist")]
        public string Resist;
        [XmlElement("vulnerable")]
        public string Vulnerable;
        [XmlElement("immune")]
        public string Immune;
        [XmlElement("conditionImmune")]
        public string ConditionImmune;
        [XmlElement("senses")]
        public string Senses;
        [XmlElement("passive")]
        public int Passive;
        [XmlElement("language")]
        public string Language;
        [XmlElement("cr")]
        public int CR;

        [XmlElement("savingThrow")]
        public List<SavingThrowRaw> SavingThrows;
        [XmlElement("skill")]
        public List<SkillRaw> Skills;
        [XmlElement("trait")]
        public List<TraitRaw> Traits;
        [XmlElement("action")]
        public List<ActionRaw> Actions;
        [XmlElement("reaction")]
        public List<ReactionRaw> Reactions;
        [XmlElement("legendary")]
        public List<LegendaryActionRaw> LegendaryActions;
        public Size? Size => (Size?)SizeId;
    }

    public enum Size
    {
        Tiny,
        Small,
        Medium,
        Large,
        Huge,
        Gargantuan,
        Collosal
    }

    public class SkillRaw
    {
        [XmlElement("id")]
        public int SkillId;
        [XmlElement("modifier")]
        public int Modifier;
        public Skill Skill => (Skill)SkillId;
    }

    // ReSharper disable InconsistentNaming
    public enum Skill
    {
        Acrobatics,
        Animal_Handling,
        Arcana,
        Athletics,
        Deception,
        History,
        Insight,
        Intimidation,
        Investigation,
        Medicine,
        Nature,
        Perception,
        Performance,
        Persuasion,
        Religion,
        Sleight_of_Hand,
        Stealth,
        Survival
    }
    // Resharper enable InconsitentNaming

    public class SavingThrowRaw
    {
        [XmlElement("ability")]
        public int AbilityId;
        [XmlElement("modifier")]
        public int Modifier;
        public Ability Ability => (Ability)AbilityId;
    }

    public enum Ability
    {
        Strength,
        Dexterity,
        Constition,
        Intelligence,
        Wisdom,
        Charisma
    }

    public class AttackRaw
    {
        [XmlElement("name")]
        public string Name;
        [XmlElement("attackBonus")]
        public int AttackBonus;
        [XmlElement("damage")]
        public string Damage;
    }

    public class TraitRaw
    {
        [XmlElement("name")]
        public string Name;
        [XmlElement("text")]
        public string Text;
        [XmlElement("attack")]
        public AttackRaw Attack;
    }

    public class ActionRaw
    {
        [XmlElement("name")]
        public string Name;
        [XmlElement("text")]
        public string Text;
        [XmlElement("attack")]
        public AttackRaw Attack;
    }


    public class ReactionRaw
    {
        [XmlElement("name")]
        public string Name;
        [XmlElement("text")]
        public string Text;
        [XmlElement("attack")]
        public AttackRaw Attack;
    }

    public class LegendaryActionRaw
    {
        [XmlElement("name")]
        public string Name;
        [XmlElement("text")]
        public string Text;
        [XmlElement("attack")]
        public AttackRaw Attack;
    }
}
