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

    // ReSharper disable InconsistentNaming
    // Resharper enable InconsitentNaming
}
