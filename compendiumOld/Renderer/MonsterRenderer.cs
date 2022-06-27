using compendiumOld.Models.CoreData;
using compendiumOld.Models.CoreData.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace compendiumOld.Renderer
{
    public class MonsterRenderer
    {
        public string RenderMonster(Monster monster)
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.AppendLine("___");
            stringBuilder.AppendLine($"> ## {monster.Name}");
            stringBuilder.AppendLine($">*{monster.Size} {RenderRace(monster.Race)}, {RenderAlignment(monster.Alignment)}*");
            stringBuilder.AppendLine("> ___");
            stringBuilder.AppendLine($"> - **Armor Class** {monster.Armorclass + (string.IsNullOrWhiteSpace(monster.Armor) ? "" : " " + monster.Armor)}");
            stringBuilder.AppendLine($"> - **Hit Points** {RenderDie(monster.HitDie)}");
            stringBuilder.AppendLine($"> - **Speed** {RenderSpeed(monster.Speed)}");
            stringBuilder.AppendLine($">___");
            stringBuilder.AppendLine($">|STR|DEX|CON|INT|WIS|CHA|");
            stringBuilder.AppendLine($">|:---:|:---:|:---:|:---:|:---:|:---:|");
            stringBuilder.AppendLine($">|{monster.Abilities[Ability.Strength]}|{monster.Abilities[Ability.Dexterity]}|{monster.Abilities[Ability.Constitution]}|" +
                                       $"{monster.Abilities[Ability.Intelligence]}|{monster.Abilities[Ability.Wisdom]}|{monster.Abilities[Ability.Charisma]}|");
            stringBuilder.AppendLine($">___");
            if (monster.SavingThrows?.Any() ?? false)
                stringBuilder.AppendLine($"> - **Saving Throws** {RenderSavingThrows(monster.SavingThrows)}");
            if (monster.Skillmodifiers?.Any() ?? false)
                stringBuilder.AppendLine($"> - **Skills** {RenderSkills(monster.Skillmodifiers)}");
            if (monster.Vulnerable?.Any() ?? false)
                stringBuilder.AppendLine($"> - **Damage Vulnerabilities** {string.Join(", ", monster.Vulnerable)}");
            if (monster.Resist?.Any() ?? false)
                stringBuilder.AppendLine($"> - **Damage Resistances** {string.Join(", ", monster.Resist)}");
            if (monster.Immune?.Any() ?? false)
                stringBuilder.AppendLine($"> - **Damage Immunities** {string.Join(", ", monster.Immune)}");
            if (monster.ConditionImmune?.Any() ?? false)
                stringBuilder.AppendLine($"> - **Condition Immunities** {string.Join(", ", monster.ConditionImmune)}");
            if (monster.Senses != null)
                stringBuilder.AppendLine($"> - **Senses** {RenderSenses(monster.Senses)}");
            stringBuilder.AppendLine($"> - **Languages** {monster.Languages ?? "-"}");
            stringBuilder.AppendLine($"> - **Challenge** {monster.ChallengeRating}");
            stringBuilder.AppendLine($">___");
            if (monster.Traits != null)
                foreach (var trait in monster.Traits)
                {
                    stringBuilder.AppendLine($"> ***{trait.Name}.*** {trait.Text}");
                    stringBuilder.AppendLine($">");
                }
            if (monster.Spellcasting != null || (monster.Actions?.Any() ?? false))
                stringBuilder.AppendLine($"> ### Actions");
            if (monster.Spellcasting != null)
                RenderSpellcasting(monster.Spellcasting, stringBuilder);
            if (monster.Actions != null)
                foreach (var action in monster.Actions)
                    RenderAction(action, stringBuilder);
            if (monster.LegendaryActions?.Any() ?? false)
            {
                stringBuilder.AppendLine($"> ### Legendary Actions");
                foreach (var la in monster.LegendaryActions)
                    RenderAction(la.Action, stringBuilder);
            }
            if (monster.Reactions?.Any() ?? false)
            {
                stringBuilder.AppendLine($"> ### Reactions");
                foreach (var ra in monster.Reactions)
                    RenderAction(ra.Action, stringBuilder);
            }

            return stringBuilder.ToString();
        }

        private void RenderAction(Models.CoreData.Action action, StringBuilder stringBuilder)
        {
            stringBuilder.AppendLine($"> ***{action.Name}.*** {action.Text}");
            stringBuilder.AppendLine($">");
        }

        private void RenderSpellcasting(Spellcasting spellcasting, StringBuilder stringBuilder)
        {
            stringBuilder.AppendLine($"> ***Spellcasting***. {spellcasting.TextBeforeTable}");
            for (var i = 0; i < spellcasting.Spells.GetLength(0); i++)
            {
                if (spellcasting.Spells[i] != null)
                {
                    stringBuilder.AppendLine($"> {SpellcastingHeader(spellcasting, i)}: {SpellList(spellcasting, i)}");
                    stringBuilder.AppendLine($">");
                }
            }
            if (!string.IsNullOrWhiteSpace(spellcasting.TextAfterTable))
                stringBuilder.AppendLine($"> {spellcasting.TextAfterTable}");
            stringBuilder.AppendLine($">");
        }

        private string SpellcastingHeader(Spellcasting spellcasting, int line)
        {
            if (line == 0)
                return "Cantrips (at will)";
            if (!spellcasting.SpellListClass.Equals("warlock", StringComparison.InvariantCultureIgnoreCase))
                return $"{ThIfy(line)} level ({spellcasting.Spellslots[line - 1]} slot{(spellcasting.Spellslots[line - 1] > 1 ? "s)" : ")")}";
            return $"1st-{ThIfy(line)} level ({spellcasting.Spellslots[line - 1]} {ThIfy(line)}-level slots)";
        }

        private string SpellList(Spellcasting spellcasting, int line)
        {
            return string.Join(", ", spellcasting.Spells[line].Select(s => $"*{s.Name}*{(s.Marked ? @"\*" : "")}").ToArray()).ToLower();
        }

        private string ThIfy(int number)
        {
            if (number == 1)
                return "1st";
            if (number == 2)
                return "2nd";
            if (number == 3)
                return "3rd";
            return number + "th";
        }

        private string RenderSenses(Senses senses)
        {
            var str = "";
            if (senses.SenseRanges?.Any() ?? false)
            {
                str = string.Join(", ", senses.SenseRanges.Select(s => $"{s.Key} {s.Value} ft.")) + (senses.BlindOutsideRange ? " (blind beyond this radius), " : ", ");
            }
            str += $"Passive Perception {senses.PassivePerception}";
            return str;
        }

        private string RenderSkills(Dictionary<Skill, int> skillmodifiers)
        {
            return string.Join(", ", skillmodifiers.Select(s => $"{s.Key} {(s.Value > 0 ? "+" : "")}{s.Value}"));
        }

        private string RenderSavingThrows(Dictionary<Ability, int> savingThrows)
        {
            return string.Join(", ", savingThrows.Select(s => $"{ShortAbility(s.Key)} {(s.Value > 0 ? "+" : "")}{s.Value}"));
        }

        private string ShortAbility(Ability key)
        {
            switch (key)
            {
                case Ability.Intelligence: return "Int";
                case Ability.Wisdom: return "Wis";
                case Ability.Strength: return "Str";
                case Ability.Dexterity: return "Dex";
                case Ability.Constitution: return "Con";
                case Ability.Charisma: return "Cha";
            }
            return "";
        }

        private string RenderSpeed(Speed speed)
        {
            if (!string.IsNullOrWhiteSpace(speed.AdditionalInformation))
                return speed.AdditionalInformation;
            var sb = new List<string>();
            if (speed.Speeds.ContainsKey(MovementType.Normal))
            {
                sb.Add($"{speed.Speeds[MovementType.Normal]} ft.");
            }
            if (speed.Speeds.ContainsKey(MovementType.Fly))
            {
                sb.Add($"fly {speed.Speeds[MovementType.Fly]} ft.");
            }
            if (speed.Speeds.ContainsKey(MovementType.Hover))
            {
                sb.Add($"fly {speed.Speeds[MovementType.Hover]} ft. (hover)");
            }
            if (speed.Speeds.ContainsKey(MovementType.Swim))
            {
                sb.Add($"swim {speed.Speeds[MovementType.Swim]} ft.");
            }
            if (speed.Speeds.ContainsKey(MovementType.Climb))
            {
                sb.Add($"climb {speed.Speeds[MovementType.Climb]} ft.");
            }
            if (speed.Speeds.ContainsKey(MovementType.Burrow))
            {
                sb.Add($"burrow {speed.Speeds[MovementType.Burrow]} ft.");
            }
            return string.Join(", ", sb.ToArray());
        }

        private string RenderDie(DieRoll hitDie)
        {
            return $"{hitDie.ExpectedRoll} ({hitDie})";
        }

        private string RenderAlignment(AlignmentDistribution alignment)
        {
            return alignment.Description;
        }

        private string RenderRace(MonsterRace race)
        {
            if (!string.IsNullOrWhiteSpace(race.Tags))
            {
                return $"{race.MonsterType} ({race.Tags})";
            }
            return race.MonsterType.ToString();
        }
    }
}
