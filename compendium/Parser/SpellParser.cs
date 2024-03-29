﻿using System.Text.RegularExpressions;
using Compendium.Models.CoreData;
using Compendium.Models.CoreData.Enums;
using Compendium.Models.ImportData;
using Compendium.Provider;

namespace Compendium.Parser
{
    public class SpellParser
    {
        private readonly DynamicEnumProvider _dep;

        public SpellParser(DynamicEnumProvider dep)
        {
            _dep = dep;
        }
        
        public Spell Parse(SpellRaw raw)
        {
            var spell = new Spell()
            {
                CastAsRitual = raw.RitualId == 1,
                ClassLists = raw.ClassLists,
                Duration = raw.Duration,
                Level = raw.Level,
                Materials = raw.Materials,
                Name = raw.Name,
                Range = raw.Range,
                School = raw.SchoolId.HasValue ? (SpellSchool)raw.SchoolId.Value : SpellSchool.None,
                SomaticComponent = raw.SomaticId == 1,
                AtHigherLevels = GetAtHigherLevels(ref raw.Text),
                Text = raw.Text,
                Time = raw.Time,
                CastingTime = GetCastingTime(raw.Time),
                VocalComponent = raw.VocalId == 1
            };
            spell.IsMultiTarget = spell.Text.Contains("targets", StringComparison.InvariantCultureIgnoreCase) ||
                                  spell.Text.Contains("creatures", StringComparison.InvariantCultureIgnoreCase);

            try
            {
                var effects = FindHitEffects(spell.Text, _dep);
                spell.Effects = effects;
                if(spell.AtHigherLevels != null)
                {
                    var effects2 = FindAtHigherLevelEffects(spell.AtHigherLevels, _dep);
                    spell.AtHigherLevelEffects = effects2;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(spell.Name);
            }
            return spell;
        }

        private List<HitEffect> FindAtHigherLevelEffects(string text, DynamicEnumProvider dep)
        {
            var hitlist = new List<HitEffect>();
            var HitDieRegex = new Regex(@"([0-9]+)d([0-9]+)");
            var hitDies = HitDieRegex.Matches(text);
            var positions = hitDies.ToDictionary(d => d.Index, d => d);
            foreach (var hitDie in positions)
            {
                var damageDie = new DieRoll(Convert.ToInt32(hitDie.Value.Groups[2].Value),
                    Convert.ToInt32(hitDie.Value.Groups[1].Value), 0);
                hitlist.Add(new HitEffect()
                {
                    DamageDie = damageDie
                });
            }
            return hitlist;
        }

        private string GetAtHigherLevels(ref string rawText)
        {
            if (rawText.Contains("at higher levels", StringComparison.InvariantCultureIgnoreCase))
            {
                if (rawText.Contains("~"))
                    throw new InvalidDataException();
                var split = rawText.Replace("at higher levels", "~", StringComparison.InvariantCultureIgnoreCase).Split("~");
                rawText = split[0].Trim();
                return split[1].Trim(':', ' ');
            }
            return null;
        }

        private CastingTime? GetCastingTime(string time)
        {
            if (time == null)
                return null;
            if (time.Contains("bonus", StringComparison.InvariantCultureIgnoreCase))
                return CastingTime.BonusAction;
            if (time.Contains("reaction", StringComparison.InvariantCultureIgnoreCase))
                return CastingTime.Reaction;
            if (time.Contains("attack action", StringComparison.InvariantCultureIgnoreCase))
                return CastingTime.AttackAction;
            if (time.Contains("action", StringComparison.InvariantCultureIgnoreCase))
                return CastingTime.Action;
            if (time.Contains("1 minute", StringComparison.InvariantCultureIgnoreCase))
                return CastingTime.Minute;
            if (time.Contains("10 minutes", StringComparison.InvariantCultureIgnoreCase))
                return CastingTime._10Minutes;
            if (time.Contains("1 hour", StringComparison.InvariantCultureIgnoreCase))
                return CastingTime.Hour;
            if (time.Contains("8 hours", StringComparison.InvariantCultureIgnoreCase))
                return CastingTime._8Hours;
            if (time.Contains("12 hours", StringComparison.InvariantCultureIgnoreCase))
                return CastingTime._12Hours;
            if (time.Contains("24 hours", StringComparison.InvariantCultureIgnoreCase))
                return CastingTime._24Hours;
            if (time.Contains("varies", StringComparison.InvariantCultureIgnoreCase))
                return CastingTime.Varies;
            return null;
        }

        public static List<HitEffect> FindHitEffects(string text, DynamicEnumProvider dep)
        {
            var hitlist = new List<HitEffect>();
            var HitDieRegex = new Regex(@"([0-9]+)d([0-9]+) ([a-z]*) damage");
            var hitDies = HitDieRegex.Matches(text);
            var positions = hitDies.ToDictionary(d => d.Index, d => d);
            var DCRegex = new Regex(@"(([A-Za-z]*) saving throw)|(\(escape DC ([0-9]*)\))|(DC ([0-9]*) [A-Za-z]* \(([A-Za-z]*)\)( or [A-Za-z]* \(([A-Za-z]*)\)*)* check)", RegexOptions.None);
            var dcs = DCRegex.Matches(text);
            var dcPositions = dcs.ToDictionary(d => d.Index, d => d);
            var conditionRegex = new Regex(string.Join("|", Enum.GetNames(typeof(Condition)).Select(c => "(" + c.ToLower() + ")").ToArray()));
            var conditions = conditionRegex.Matches(text);
            var effects = new Dictionary<int, HitEffect>();
            foreach (var hitDie in positions)
            {
                var hitEffect = FindEffectForPosition(hitDie.Key, dep, effects, dcPositions);
                var damageDie = new DieRoll(Convert.ToInt32(hitDie.Value.Groups[2].Value), Convert.ToInt32(hitDie.Value.Groups[1].Value), 0);
                DamageType? damageType = null;
                if (!Enum.TryParse<DamageType>(hitDie.Value.Groups[3].Value, out var dmgt))
                {
                    damageType = null;
                };
                if (hitEffect.DamageDie != null)
                {
                    hitEffect = new HitEffect(hitEffect)
                    {
                        DamageDie = damageDie,
                        DamageType = damageType
                    };
                    hitlist.Add(hitEffect);
                }
                else
                {
                    hitEffect.DamageDie = damageDie;
                    hitEffect.DamageType = damageType;
                }
            }
            foreach (var cond in conditions.ToArray())
            {
                var hitEffect = FindEffectForPosition(cond.Index, dep, effects, dcPositions);
                hitEffect.Condition.Add(Enum.Parse<Condition>(cond.Value, true));
            }

            hitlist.AddRange(effects.Values);
            return hitlist;
        }

        private static HitEffect FindEffectForPosition(int position, DynamicEnumProvider dep, Dictionary<int, HitEffect> effects, Dictionary<int, Match> dcPositions)
        {
            HitEffect effect;
            var dc = dcPositions.LastOrDefault(d => d.Key < position || d.Key - 20 < position && d.Value.Value.Contains("escape"));
            if (!dc.Equals(default(KeyValuePair<int, Match>)))
            {
                if (!effects.ContainsKey(dc.Key))
                {
                    effects[dc.Key] = new HitEffect();
                    if (dc.Value.Value.Contains("escape"))
                    {
                        effects[dc.Key].DC = new SkillCheck
                        {
                            Skill = Skill.Acrobatics | Skill.Athletics,
                            Value = Convert.ToInt32(dc.Value.Groups[5].Value)
                        };
                    }
                    else if (dc.Value.Value.Contains("check"))
                    {
                        effects[dc.Key].DC = new SkillCheck
                        {
                            Value = Convert.ToInt32(dc.Value.Groups[7].Value)
                        };
                        var reg = new Regex(@"\([A-Za-z]*\)");
                        foreach (var s in reg.Matches(dc.Value.Value).Select(s => Enum.Parse<Skill>(s.Value.Trim('(', ')'), true)))
                        {
                            ((SkillCheck)effects[dc.Key].DC).Skill |= s;
                        }
                    }
                    else if (dc.Value.Value.Contains("saving throw"))
                    {
                        var str = dc.Value.Value.Replace("saving throw", "").Replace("DC " + dc.Value.Groups[2].Value, "").Trim();
                        effects[dc.Key].DC = new SavingThrow()
                        {
                            Ability = dep.GetEnumValues("Ability").Parse(str)
                        };
                    }
                }
                effect = effects[dc.Key];
            }
            else
            {
                if (!effects.ContainsKey(0))
                {
                    effects[0] = new HitEffect();
                }
                effect = effects[0];
            }
            return effect;
        }
    }
}