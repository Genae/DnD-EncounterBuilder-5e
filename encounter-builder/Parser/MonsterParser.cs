using System;
using System.Collections.Generic;
using System.Linq;
using encounter_builder.Models;
using encounter_builder.Models.CoreData;
using encounter_builder.Models.ImportData;
using Action = encounter_builder.Models.CoreData.Action;

namespace encounter_builder.Parser
{
    public class MonsterParser
    {
        private readonly SpellcastingParser _spellParser;
        private readonly ActionParser _actionParser;

        public MonsterParser(SpellcastingParser spellParser, ActionParser actionParser)
        {
            _spellParser = spellParser;
            _actionParser = actionParser;
        }
        public Monster Parse(MonsterRaw raw, List<SpellRaw> spells)
        {
            var errors = new List<string>();
            var monster = new Monster
            {
                Name = raw.Name,
                Abilities = ParseAbilities(raw, ref errors),
                Actions = ParseActions(raw, errors),
                ChallengeRating = new ChallengeRating(raw.CR),
                Spellcasting = CheckForSpellcasting(spells, raw, ref errors)
            };
            monster.HitDie = GetHealthDies(monster.MaximumHitpoints, monster.Abilities[Ability.Constitution], raw.SizeId);
            return monster;
        }

        private List<Action> ParseActions(MonsterRaw raw, List<string> errors)
        {
            return raw.Actions.Select(r => _actionParser.ParseAction(r, errors)).ToList();
        }
        
        private Dictionary<Ability, AbilityScore> ParseAbilities(MonsterRaw raw, ref List<string> errors)
        {
            return AbilityScore.GetFromString(raw.AbilityString, ref errors);
        }

        private DieRoll GetHealthDies(int maximumHitpoints, AbilityScore abilityScore, int? sizeId)
        {
            var size = sizeId ?? 2;
            var dieSize = size <= 4 ? size * 2 + 4 : 20;
            var level = (int)Math.Round(maximumHitpoints / (dieSize / 2f + 0.5f + abilityScore.Modifier));
            return new DieRoll(dieSize, level, level * abilityScore.Modifier);
        }

        public Spellcasting CheckForSpellcasting(List<SpellRaw> spells, MonsterRaw raw, ref List<string> errors)
        {
            for (var i = 0; i < raw.Traits.Count; i++)
            {
                var trait = raw.Traits[i];
                if (trait.Name.Equals("Spellcasting"))
                {
                    var spellcasting = _spellParser.ParseSpellcasting(trait.Text, spells, ref errors);
                    raw.Traits.RemoveAt(i);
                    return spellcasting;
                }
            }
            return null;
        }
    }
}
