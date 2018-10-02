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
                Actions = ParseActions(raw.Actions, errors),
                Alignment = ParseAlignment(raw, errors),
                Armor = raw.Armor,
                Armorclass = raw.Armorclass,
                ChallengeRating = new ChallengeRating(raw.CR),
                ConditionImmune = ParseConditions(raw.ConditionImmune, errors),
                Immune = ParseDamageTypes(raw.Immune),
                Languages = raw.Languages,
                LegendaryActions = ParseActions(raw.LegendaryActions, errors).Select(a => new LegendaryAction {Action = a}).ToList(),
                MaximumHitpoints = raw.MaximumHitpoints,
                Reactions = ParseActions(raw.Reactions, errors).Select(a => new Reaction { Action = a }).ToList(),
                Resist = ParseDamageTypes(raw.Resist),
                SavingThrows = raw.SavingThrows.ToDictionary(s => s.Ability, s => s.Modifier),
                Senses = ParseSenses(raw.Senses, errors),
                Size = (Size)(raw.SizeId ?? 2),
                Skillmodifiers = raw.Skills.ToDictionary(s => s.Skill, s => s.Modifier),
                Speed = ParseSpeed(raw.Speed, errors),
                Spellcasting = CheckForSpellcasting(spells, raw, ref errors),
                Traits = raw.Traits.Select(t => new Trait(){Name = t.Name, Text = t.Text}).ToList(),
                Type = ParseMonsterType(raw.Type),
                Vulnerable = ParseDamageTypes(raw.Vulnerable)
            };
            monster.HitDie = GetHealthDies(monster.MaximumHitpoints, monster.Abilities[Ability.Constitution], raw.SizeId);
            return monster;
        }

        private MonsterRace ParseMonsterType(string rawType)
        {
            var race = new MonsterRace();
            foreach (MonsterType monsterRace in Enum.GetValues(typeof(MonsterType)))
            {
                if (rawType.Contains(monsterRace.ToString()))
                    race.Type = monsterRace;
            }
            var regex = new Regex(@"\(([A-Za-z]*)\)");
            race.Tags = regex.Match(rawType).Value;
            return race;
        }

        private Speed ParseSpeed(string rawSpeed, List<string> errors)
        {
            var speed = new Speed {AdditionalInformation = rawSpeed};
            var regex = new Regex("^([0-9]+)");
            if(regex.IsMatch(rawSpeed))
                speed.Speeds[MovementType.Normal] = Convert.ToInt32(regex.Match(rawSpeed.ToLower()).Groups[1].Value);

            regex = new Regex("(burrow )([0-9]*)");
            if (regex.IsMatch(rawSpeed))
                speed.Speeds[MovementType.Burrow] = Convert.ToInt32(regex.Match(rawSpeed.ToLower()).Groups[2].Value);

            regex = new Regex("(climb )([0-9]*)");
            if (regex.IsMatch(rawSpeed))
                speed.Speeds[MovementType.Climb] = Convert.ToInt32(regex.Match(rawSpeed.ToLower()).Groups[2].Value);

            regex = new Regex("(fly )([0-9]*)");
            if (regex.IsMatch(rawSpeed))
                speed.Speeds[rawSpeed.Contains("hover") ? MovementType.Hover : MovementType.Fly] = Convert.ToInt32(regex.Match(rawSpeed.ToLower()).Groups[2].Value);

            regex = new Regex("(swim )([0-9]*)");
            if (regex.IsMatch(rawSpeed))
                speed.Speeds[MovementType.Swim] = Convert.ToInt32(regex.Match(rawSpeed.ToLower()).Groups[2].Value);

            return speed;
        }

        private Senses ParseSenses(string rawSenses, List<string> errors)
        {
            if (rawSenses == null)
                return null;
            var senses = new Senses();
            var regex = new Regex("(passive perception )([0-9]*)");
            if(regex.IsMatch(rawSenses.ToLower()))
                senses.PassivePerception = Convert.ToInt32(regex.Match(rawSenses.ToLower()).Groups[2].Value);

            regex = new Regex("(blindsight )([0-9]*)");
            if (regex.IsMatch(rawSenses.ToLower()))
                senses.SenseRanges[Sense.Blindsight] = Convert.ToInt32(regex.Match(rawSenses.ToLower()).Groups[2].Value);

            regex = new Regex("(truesight )([0-9]*)");
            if (regex.IsMatch(rawSenses.ToLower()))
                senses.SenseRanges[Sense.Truesight] = Convert.ToInt32(regex.Match(rawSenses.ToLower()).Groups[2].Value);

            regex = new Regex("(tremorsense )([0-9]*)");
            if (regex.IsMatch(rawSenses.ToLower()))
                senses.SenseRanges[Sense.Tremorsense] = Convert.ToInt32(regex.Match(rawSenses.ToLower()).Groups[2].Value);

            regex = new Regex("(darkvision )([0-9]*)");
            if (regex.IsMatch(rawSenses.ToLower()))
                senses.SenseRanges[Sense.Darkvision] = Convert.ToInt32(regex.Match(rawSenses.ToLower()).Groups[2].Value);

            if (rawSenses.ToLower().Contains("blind beyond this radius"))
                senses.BlindOutsideRange = true;

            return senses;
        }

        private DamageType[] ParseDamageTypes(string rawImmune)
        {
            if (rawImmune == null)
                return null;
            var damageTypes = new List<DamageType>();
            if (rawImmune.ToLower().Contains("from nonmagical attacks"))
            {
                damageTypes.AddRange(new []{DamageType.Piercing, DamageType.Slashing, DamageType.Bludgeoning}.Where(dt => rawImmune.Contains(dt.ToString())));
            }
            else
            {
                damageTypes.AddRange(new[] { DamageType.PiercingMagic, DamageType.SlashingMagic, DamageType.BludgeoningMagic }.Where(dt => rawImmune.Contains(dt.ToString().Replace("Magic", ""))));
            }
            damageTypes.AddRange(new []
            {
                DamageType.Acid,
                DamageType.Cold,
                DamageType.Fire,
                DamageType.Force,
                DamageType.Lightning,
                DamageType.Necrotic,
                DamageType.Poison,
                DamageType.Psychic,
                DamageType.Radiant,
                DamageType.Thunder,
            }.Where(dt => rawImmune.Contains(dt.ToString())));
            return damageTypes.ToArray();
        }

        private Condition[] ParseConditions(string rawConditionImmune, List<string> errors)
        {
            try
            {
                return rawConditionImmune?.Split(',').Select(s => Enum.Parse<Condition>(s.Trim(), true)).ToArray();
            }
            catch (Exception ex)
            {
                errors.Add(ex.ToString());
                return null;
            }
        }

        private AlignmentDistribution ParseAlignment(MonsterRaw raw, List<string> errors)
        {
            foreach (Morality morality in Enum.GetValues(typeof(Morality)))
            {
                foreach (Order order in Enum.GetValues(typeof(Order)))
                {
                    if(raw.Alignment.Contains($"{order} {morality}"))
                        return new AlignmentDistribution(new Alignment(morality, order));
                }
                if (raw.Alignment.Contains("any " + morality + "alignment"))
                {
                    return AlignmentDistribution.Any(morality);
                }
            }
            foreach (Morality morality in Enum.GetValues(typeof(Morality)))
            {
                if (raw.Alignment.Contains("any " + morality + "alignment"))
                {
                    return AlignmentDistribution.Any(morality);
                }
            }
            if (raw.Alignment.Contains("any"))
            {
                return AlignmentDistribution.Any();
            }
            if (raw.Alignment.Contains("unaligned"))
            {
                return AlignmentDistribution.Unaligned();
            }
            errors.Add("Could not find Alignment");
            return null;
        }

        private List<Action> ParseActions(List<ActionRaw> raw, List<string> errors)
        {
            return raw.Select(r => _actionParser.ParseAction(r, errors)).ToList();
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
