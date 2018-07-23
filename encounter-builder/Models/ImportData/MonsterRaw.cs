using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using encounter_builder.Database;
using encounter_builder.Models.CoreData;

namespace encounter_builder.Models.ImportData
{
    public class MonsterRaw
    {
        [XmlElement("name")]
        public string Name;
        [XmlElement("size")]
        public int? SizeId;
        [XmlElement("type")]
        public string Type;
        [XmlElement("alignment")]
        public string Alignment;
        [XmlElement("armor")]
        public string Armor;
        [XmlElement("ac")]
        public int Armorclass;
        [XmlElement("hpMax")]
        public int MaximumHitpoints;
        [XmlElement("speed")]
        public string Speed;
        [XmlElement("abilities")]
        public string AbilityString;
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
        [XmlElement("languages")]
        public string Languages;
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
        [XmlIgnore]
        public Size? Size => (Size?)(SizeId ?? 2);
        [XmlIgnore]
        public Dictionary<Ability, AbilityScore> Abilities => AbilityScore.GetFromString(AbilityString);
        [XmlIgnore]
        public DieRoll HitDie => GetHealthDies(MaximumHitpoints, Abilities[Ability.Constitution], SizeId);

        public ChallengeRating ChallengeRating => new ChallengeRating(CR);
        [XmlIgnore]
        public Spellcasting Spellcasting;

        private DieRoll GetHealthDies(int maximumHitpoints, AbilityScore abilityScore, int? sizeId)
        {
            var size = sizeId ?? 2;
            var dieSize = size <= 4 ? size * 2 + 4 : 20;
            var level = (int)Math.Round(maximumHitpoints / (dieSize / 2f + 0.5f + abilityScore.Modifier));
            return new DieRoll(dieSize, level, level * abilityScore.Modifier);
        }

        public bool CheckForSpellcasting(List<SpellRaw> spells, Importer importer)
        {
            for (var i = 0; i < Traits.Count; i++)
            {
                var trait = Traits[i];
                if (trait.Name.Equals("Spellcasting"))
                {
                    Traits[i] = Spellcasting = new Spellcasting(trait.Text, spells, importer);
                }
            }
            return Spellcasting != null;
        }
    }

    public class ChallengeRating
    {
        public int Value;
        public int Experience;
        public string Description => ToString();

        private static int[] ExperienceTable = new[]
        {
            10,
            25,
            50,
            100,
            200,
            450,
            700,
            1100,
            1800,
            2300,
            2900,
            3900,
            5000,
            5900,
            7200,
            8400,
            10000,
            11500,
            13000,
            15000,
            18000,
            20000,
            22000,
            25000,
            33000,
            41000,
            50000,
            62000,
            75000,
            90000,
            105000,
            120000,
            135000,
            155000
        };

        public ChallengeRating(int cr)
        {
            Value = cr;
            Experience = ExperienceTable[cr + 3];
        }

        public override string ToString()
        {
            var cr = Value + "";
            if (Value == 0)
                cr = "1/2";
            if (Value == -1)
                cr = "1/4";
            if (Value == -2)
                cr = "1/8";
            if (Value == -3)
                cr = "0";
            return $"{cr} ({Experience:N0} XP)";
        }
    }
}
