using compendium.Database;
using compendium.Models.CoreData.Enums;
using compendium.Renderer;
using System;
using System.Collections.Generic;
using System.Linq;

namespace compendium.Models.CoreData
{
    public class Monster : KeyedDocument
    {
        public string Name { get; set; }
        public Size Size { get; set; }
        public MonsterRace Race { get; set; }
        public AlignmentDistribution Alignment { get; set; }
        public ArmorInfo ArmorInfo { get; set; }
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

        public string Markup => (new MonsterRenderer().RenderMonster(this));

        public Monster()
        {
            Abilities = new Dictionary<Ability, AbilityScore>();
            foreach (var ability in Enum.GetValues(typeof(Ability)).Cast<Ability>())
                Abilities.Add(ability, new AbilityScore() { Value = 10 });
            Race = new();
            Senses = new();
            ChallengeRating = new();
            HitDie = new();
            Skillmodifiers = new();
            SavingThrows = new();
            Alignment = new();
            Speed = new();
            ArmorInfo = new();
        }
    }
}
