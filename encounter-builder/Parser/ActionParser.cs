using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using encounter_builder.Models;
using encounter_builder.Models.CoreData;
using encounter_builder.Models.ImportData;
using Microsoft.EntityFrameworkCore.Internal;
using Action = encounter_builder.Models.CoreData.Action;

namespace encounter_builder.Parser
{
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
            var HitDieRegex = new Regex(@"([0-9]*) \(([0-9]*)d([0-9]*)( [\+|\-] [0-9]*)*\) ([a-z]*) damage");
            var hitDies = HitDieRegex.Matches(action.Text);
            var positions = hitDies.ToDictionary(d => d.Index, d => d);
            var DCRegex = new Regex(@"(DC ([0-9]*) ([A-Za-z]*) saving throw)|(\(escape DC ([0-9]*)\))|(DC ([0-9]*) [A-Za-z]* \(([A-Za-z]*)\)( or [A-Za-z]* \(([A-Za-z]*)\)*)* check)", RegexOptions.None);
            var dcs = DCRegex.Matches(action.Text);
            var dcPositions = dcs.ToDictionary(d => d.Index, d => d);
            var conditionRegex = new Regex(Enum.GetNames(typeof(Condition)).Select(c => "(" + c.ToLower() + ")").ToArray().Join("|"));
            var conditions = conditionRegex.Matches(action.Text);
            var effects = new Dictionary<int, HitEffect>();
            foreach (var hitDie in positions)
            {
                var hitEffect = FindEffectForPosition(hitDie.Key, effects, dcPositions);
                var damageDie = new DieRoll(Convert.ToInt32(hitDie.Value.Groups[3].Value), Convert.ToInt32(hitDie.Value.Groups[2].Value), hitDie.Value.Groups[4].Value.Length > 0 ? Convert.ToInt32(hitDie.Value.Groups[4].Value.Replace(" ", "")) : 0);
                var damageType = Enum.Parse<DamageType>(hitDie.Value.Groups[5].Value, true);
                if (hitEffect.DamageDie != null)
                {
                    hitEffect = new HitEffect(hitEffect)
                    {
                        DamageDie = damageDie,
                        DamageType = damageType
                    };
                    action.HitEffects.Add(hitEffect);
                }
                else
                {
                    hitEffect.DamageDie = damageDie;
                    hitEffect.DamageType = damageType;
                }
            }
            foreach (var cond in conditions.ToArray())
            {
                var hitEffect = FindEffectForPosition(cond.Index, effects, dcPositions);
                hitEffect.Condition.Add(Enum.Parse<Condition>(cond.Value, true));
            }

            action.HitEffects.AddRange(effects.Values);
        }

        private HitEffect FindEffectForPosition(int position, Dictionary<int, HitEffect> effects, Dictionary<int, Match> dcPositions)
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
                            ((SkillCheck) effects[dc.Key].DC).Skill |= s;
                        }
                    }
                    else if (dc.Value.Value.Contains("saving throw"))
                    {
                        var str = dc.Value.Value.Replace("saving throw", "").Replace("DC " + dc.Value.Groups[2].Value, "").Trim();
                        effects[dc.Key].DC = new SavingThrow()
                        {
                            Value = Convert.ToInt32(dc.Value.Groups[2].Value),
                            Ability = Enum.Parse<Ability>(str, true)
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

        private void GetAttackTypeFromText(Action action, List<string> errors)
        {
            var text = action.Text.ToLower();
            var t = Enum.GetValues(typeof(AttackType));
            AttackType[] types = new AttackType[t.Length];
            t.CopyTo(types, 0);
            t = types.Reverse().ToArray();
            foreach (AttackType type in t)
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
            var hitRegex = new Regex(@"\+( )*([0-9]*) to hit");
            var hit = hitRegex.Match(action.Text);
            if (hit.Success)
            {
                action.Attack.AttackBonus = Convert.ToInt32(hit.Groups[2].Value);
                return;
            }
            errors.Add("unable to find hit bonus in " + action.Text);
        }

        private void GetTargetFromText(Action action, List<string> errors, int reachEnd)
        {
            var end = action.Text.IndexOf("Hit:", StringComparison.Ordinal);
            if (end != -1)
            {
                action.Attack.Target = action.Text.Substring(reachEnd, end - reachEnd).Trim(',').Trim(' ').Trim('.');
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
                    if(range.Length > 1)
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