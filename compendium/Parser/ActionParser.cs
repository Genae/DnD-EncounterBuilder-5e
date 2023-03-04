using Compendium.Models.CoreData;
using Compendium.Models.CoreData.Enums;
using Compendium.Models.ImportData;
using Compendium.Provider;
using System.Text.RegularExpressions;
using Action = Compendium.Models.CoreData.Action;

namespace Compendium.Parser
{
    public class ActionParser
    {
        public Multiattack ParseMultiattack(ActionRaw raw, List<string> errors, List<Action> actions, out string shortName)
        {
            var action = new Multiattack
            {
                Name = raw.Name,
                Text = new Regex("[ ]{2,}", RegexOptions.None).Replace(raw.Text.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " "), " "),
                Actions = new Dictionary<string, int>()
            };


            var textSplit = action.Text.ToLower().Split(new[] { ":", ",", "and", "or", "." }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var t in textSplit)
            {
                FindActionInText(t, actions, action);
            }

            shortName = FindShortNameInText(action.Text);

            return action;
        }

        private string FindShortNameInText(string text)
        {
            var regex1 = new Regex(@"^The ([a-zA-Z ]*) (can use|uses)");
            var match1 = regex1.Match(text);
            var regex2 = new Regex(@"^The ([a-zA-Z ]*) (can make|makes)");
            var match2 = regex2.Match(text);
            if (match1.Success && !match2.Success)
                return match1.Groups[1].Value;
            if (!match1.Success && match2.Success)
                return match2.Groups[1].Value;
            if (match1.Groups[1].Value.Length > match2.Groups[1].Value.Length)
                return match2.Groups[1].Value;
            return match1.Groups[1].Value;
        }

        public void FindActionInText(string t, List<Action> actions, Multiattack action)
        {
            foreach (var a in actions)
            {
                if (t.Contains(a.Name, StringComparison.CurrentCultureIgnoreCase))
                {
                    var count = FindCountInText(t);
                    action.Actions[a.Name] = count;
                    return;
                }
            }
        }

        public int FindCountInText(string text)
        {
            if (text.Contains("one"))
                return 1;
            if (text.Contains("two"))
                return 2;
            if (text.Contains("three"))
                return 3;
            if (text.Contains("four"))
                return 4;
            if (text.Contains("five"))
                return 5;
            if (text.Contains("once"))
                return 1;
            if (text.Contains("twice"))
                return 2;
            return 1;
        }

        public Action ParseAction(ActionRaw raw, List<string> errors, DynamicEnumProvider dep)
        {
            var limitedUsage = GetLimitedUsageFromName(ref raw.Name);
            var action = new Action
            {
                Name = raw.Name,
                Text = new Regex("[ ]{2,}", RegexOptions.None).Replace(raw.Text.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " "), " "),
                LimitedUsage = limitedUsage
            };
            GetAttackTypeFromText(action, errors);
            if (action.Attack != null)
            {
                GetHitBonusFromText(action, errors);
                GetReachFromText(action, errors, out var reachEnd);
                GetTargetFromText(action, errors, reachEnd);
            }
            var pos = 0;
            FindHitEffects(action, errors, ref pos, dep);
            return action;
        }

        public static LimitedUsage? GetLimitedUsageFromName(ref string rawName)
        {
            if (rawName == null)
                return null;
            foreach (LimitedUsage usage in Enum.GetValues(typeof(LimitedUsage)))
            {
                if (rawName.ToLower().Contains(usage.ToDescriptionString().ToLower()))
                {
                    rawName = rawName.Replace(usage.ToDescriptionString(), "", StringComparison.CurrentCultureIgnoreCase).Replace("()", "").Trim();
                    return usage;
                }
            }
            return null;
        }

        private void FindHitEffects(Action action, List<string> errors, ref int pos, DynamicEnumProvider dep)
        {
            var HitDieRegex = new Regex(@"([0-9]*) \(([0-9]*)d([0-9]*)( [\+|\-] [0-9]*)*\) ([a-z]*) damage");
            var hitDies = HitDieRegex.Matches(action.Text);
            var positions = hitDies.ToDictionary(d => d.Index, d => d);
            var DCRegex = new Regex(@"(DC ([0-9]*) ([A-Za-z]*) saving throw)|(\(escape DC ([0-9]*)\))|(DC ([0-9]*) [A-Za-z]* \(([A-Za-z]*)\)( or [A-Za-z]* \(([A-Za-z]*)\)*)* check)", RegexOptions.None);
            var dcs = DCRegex.Matches(action.Text);
            var dcPositions = dcs.ToDictionary(d => d.Index, d => d);
            var conditionRegex = new Regex(string.Join("|", Enum.GetNames(typeof(Condition)).Select(c => "(" + c.ToLower() + ")").ToArray()));
            var conditions = conditionRegex.Matches(action.Text);
            var effects = new Dictionary<int, HitEffect>();
            foreach (var hitDie in positions)
            {
                var hitEffect = FindEffectForPosition(hitDie.Key, dep, effects, dcPositions);
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
                var hitEffect = FindEffectForPosition(cond.Index, dep, effects, dcPositions);
                hitEffect.Condition.Add(Enum.Parse<Condition>(cond.Value, true));
            }

            action.HitEffects.AddRange(effects.Values);
        }

        private HitEffect FindEffectForPosition(int position, DynamicEnumProvider dep, Dictionary<int, HitEffect> effects, Dictionary<int, Match> dcPositions)
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
                            Value = Convert.ToInt32(dc.Value.Groups[2].Value),
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
                    if (range.Length > 1)
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