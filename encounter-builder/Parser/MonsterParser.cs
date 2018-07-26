using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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

    public class ActionParser
    {
        public Action ParseAction(ActionRaw raw, List<string> errors)
        {
            var action = new Action
            {
                Name = raw.Name,
                Text = new Regex("[ ]{2,}", RegexOptions.None).Replace(raw.Text.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " "), " "),
            };
            GetAttackTypeFromText(action, errors);
            if (action.Attack != null)
            {
                GetHitBonusFromText(action, errors);
                GetReachFromText(action, errors, out var reachEnd);
                GetTargetFromText(action, errors, reachEnd);
            }
            var pos = 0;
            FindHitEffects(action, errors, ref pos);
            return action;
        }

        private void FindHitEffects(Action action, List<string> errors, ref int pos)
        {
            var HitDieRegex = new Regex(@"/[0-9]* \([a-zA-Z0-9+ ]*\)/gm");
            var hitDies = HitDieRegex.Matches(action.Text);
            var positions = hitDies.ToDictionary(d => d.Index, d => d.Value);
            var DCRegex = new Regex("/DC [0-9]*/gm", RegexOptions.None);
            var dcs = DCRegex.Matches(action.Text);
            positions = (Dictionary<int, string>) positions.Concat(dcs.ToDictionary(d => d.Index, d => d.Value));
        }

        private void GetAttackTypeFromText(Action action, List<string> errors)
        {
            var text = action.Text.ToLower();
            foreach (AttackType type in Enum.GetValues(typeof(AttackType)))
            {
                if (text.Contains(type.ToString().Replace("_", " ").ToLower()))
                {
                    action.Attack = new Attack
                    {
                        Type = type
                    };
                    return;
                }
            }
            errors.Add("Unable to find AttackType in " + action.Text);
        }

        private void GetHitBonusFromText(Action action, List<string> errors)
        {
            for (var i = 0; i < 50; i++)
            {
                if (action.Text.Contains($"+{i} to hit"))
                {
                    action.Attack.AttackBonus = i;
                    return;
                }
            }
            errors.Add("unable to find hit bonus in " + action.Text);
        }

        private void GetTargetFromText(Action action, List<string> errors, int reachEnd)
        {
            var end = action.Text.IndexOf("Hit:", StringComparison.Ordinal);
            if (end != -1)
            {
                action.Attack.Target = action.Text.Substring(reachEnd, end - reachEnd);
                return;
            }
            errors.Add("unable to find target in " + action.Text);
        }

        private void GetReachFromText(Action action, List<string> errors, out int reachEnd)
        {
            var start = action.Text.IndexOf("reach", StringComparison.Ordinal);
            int end;
            reachEnd = 0;
            if (start != -1)
            {
                start += "reach".Length;
                end = action.Text.IndexOf("ft.", start, StringComparison.Ordinal);
                if (end != -1)
                {
                    action.Attack.Reach = Convert.ToInt32(action.Text.Substring(start, end - start).Trim());
                    reachEnd = end + 3;
                }
            }
            start = action.Text.IndexOf("range", StringComparison.Ordinal);
            if (start != -1)
            {
                start += "range".Length;
                end = action.Text.IndexOf("ft.", start, StringComparison.Ordinal);
                if (end != -1)
                {
                    var range = action.Text.Substring(start, end - start).Trim().Split('/');
                    action.Attack.ShortRange = Convert.ToInt32(range[0]);
                    if(range.Length > 0)
                        action.Attack.LongRange = Convert.ToInt32(range[1]);
                    reachEnd = end + 3;
                }
            }
            if (action.Attack.Type == AttackType.Melee_Spell_Attack ||
                action.Attack.Type == AttackType.Melee_Weapon_Attack ||
                action.Attack.Type == AttackType.Melee_or_Ranged_Spell_Attack ||
                action.Attack.Type == AttackType.Melee_or_Ranged_Weapon_Attack)
            {
                if (action.Attack.Reach == 0)
                {
                    errors.Add("Unable to find Reach in " + action.Text);
                }
            }
            if (action.Attack.Type == AttackType.Ranged_Spell_Attack ||
                action.Attack.Type == AttackType.Ranged_Weapon_Attack ||
                action.Attack.Type == AttackType.Melee_or_Ranged_Spell_Attack ||
                action.Attack.Type == AttackType.Melee_or_Ranged_Weapon_Attack)
            {
                if (action.Attack.ShortRange == 0)
                {
                    errors.Add("Unable to find short range in " + action.Text);
                }
            }
            if (action.Attack.Type == AttackType.Ranged_Weapon_Attack ||
                action.Attack.Type == AttackType.Melee_or_Ranged_Weapon_Attack)
            {
                if (action.Attack.LongRange == 0)
                {
                    errors.Add("Unable to find long range in " + action.Text);
                }
            }
        }
    }
}
