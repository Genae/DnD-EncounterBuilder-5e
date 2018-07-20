using System.Collections.Generic;
using System.Xml.Serialization;

namespace encounter_builder.Models
{
    public class MonsterRawData
    {
        [XmlElement("name")]
        public string Name;
        [XmlElement("size")]
        public string Size;
        [XmlElement("type")]
        public string Type;
        [XmlElement("alignment")]
        public string Alignment;
        [XmlElement("ac")]
        public string AC;
        [XmlElement("hp")]
        public string HP;
        [XmlElement("speed")]
        public string Speed;
        [XmlElement("str")]
        public int Str;
        [XmlElement("dex")]
        public int Dex;
        [XmlElement("con")]
        public int Con;
        [XmlElement("int")]
        public int Int;
        [XmlElement("wis")]
        public int Wis;
        [XmlElement("cha")]
        public int Cha;
        [XmlElement("save")]
        public string Save;
        [XmlElement("skill")]
        public string Skill;
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
        public string CR;
        [XmlElement("spells")]
        public string Spells;
        [XmlElement("slots")]
        public string Slots;

        [XmlElement("trait")]
        public List<TraitRaw> Traits;
        [XmlElement("action")]
        public List<ActionRaw> Actions;
        [XmlElement("reaction")]
        public List<ReactionRaw> Reactions;
        [XmlElement("legendary")]
        public List<LegendaryActionRaw> LegendaryActions;
    }
    
    public class TraitRaw
    {
        [XmlElement("name")]
        public string Name;
        [XmlElement("text")]
        public string Text;
        [XmlElement("attack")]
        public string Attack;
    }

    public class ActionRaw
    {
        [XmlElement("name")]
        public string Name;
        [XmlElement("text")]
        public string Text;
        [XmlElement("attack")]
        public string Attack;
    }

    public class ReactionRaw
    {
        [XmlElement("name")]
        public string Name;
        [XmlElement("text")]
        public string Text;
        [XmlElement("attack")]
        public string Attack;
    }

    public class LegendaryActionRaw
    {
        [XmlElement("name")]
        public string Name;
        [XmlElement("text")]
        public string Text;
        [XmlElement("attack")]
        public string Attack;
    }
}
