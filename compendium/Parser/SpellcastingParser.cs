using System;
using System.Collections.Generic;
using System.Linq;
using Compendium.Models.CoreData;
using Compendium.Models.CoreData.Enums;

namespace Compendium.Parser
{
    public class SpellcastingParser
    {
        public Spellcasting ParseSpellcasting(string spellcastingDescription, List<Spell> spells, ref List<string> errors)
        {
            spellcastingDescription = spellcastingDescription
                .Replace("spell caster", "spellcaster")
                .Replace("level-spellcaster", "level spellcaster")
                .Replace(": ", ":")
                .Replace(":", ": ")
                .Replace("th-level", "th level")
                .Replace("th level spellcaster", "th-level spellcaster")
                .Replace("th level slot", "th-level slot")
                .Replace("Save DC", "save DC")
                .Replace(" Level", " level")
                .Replace("st—", "st-");
            var SpellcastingLevel = TryFindLevel(spellcastingDescription, ref errors);
            var SpellListClass = TryFindSpellListClass(spellcastingDescription, ref errors);
            var SpellcastingAbility = TryFindSpellcastingAbility(spellcastingDescription, ref errors);
            var SpellslotsByLevel = GetSpellslotByLevel(SpellcastingLevel, SpellListClass);
            var Spellslots = CheckSpellslotsByDescription(SpellslotsByLevel, SpellListClass, spellcastingDescription);
            if (!Spellslots.SequenceEqual(SpellslotsByLevel))
            {
                var levelBySlots = FindSpellCastingLevelBySlots(Spellslots, SpellListClass);
                if (levelBySlots != -1)
                {
                    spellcastingDescription = FixDescription(SpellcastingLevel, levelBySlots, spellcastingDescription);
                    SpellcastingLevel = levelBySlots;
                }
            }
            var SpellDC = TryFindSpellsaveDC(spellcastingDescription, ref errors);
            var SpellcastingModifier = TryFindSpellcastingModifier(spellcastingDescription, SpellDC, ref errors);
            var Spells = TryFindSpells(Spellslots, spellcastingDescription, SpellListClass, spells, ref errors, out var _spellTableStart, out var _spellTableEnd);
            var (before, _, after) = RemoveSpellTable(_spellTableStart, _spellTableEnd, spellcastingDescription);
            return new Spellcasting
            {
                Name = "Spellcasting",
                Text = spellcastingDescription,
                SpellcastingLevel = SpellcastingLevel,
                SpellListClass = SpellListClass,
                SpellcastingAbility = SpellcastingAbility,
                Spellslots = Spellslots,
                SpellDC = SpellDC,
                SpellcastingModifier = SpellcastingModifier,
                Spells = Spells,
                TextBeforeTable = before,
                TextAfterTable = after
            };
        }


        private (string before, string old, string after) RemoveSpellTable(int spellTableStart, int spellTableEnd, string spellcastingDescription)
        {
            var realStart = spellcastingDescription.LastIndexOf(':', spellTableStart) + 1;
            var TextBeforeTable = spellcastingDescription.Substring(0, realStart);
            var OldTableText = spellcastingDescription.Substring(realStart, spellTableEnd - realStart);
            var TextAfterTable = spellcastingDescription.Substring(spellTableEnd, spellcastingDescription.Length - spellTableEnd);
            return (TextBeforeTable, OldTableText, TextAfterTable);
        }

        private PreparedSpell[][] TryFindSpells(int[] spellslots, string spellcastingDescription, string spellListClass, List<Spell> spells, ref List<string> errors, out int _spellTableStart, out int _spellTableEnd)
        {
            _spellTableStart = 0;
            _spellTableEnd = 0;
            var spellList = new PreparedSpell[10][];
            for (var i = 0; i <= 9; i++)
            {
                if (i == 0 || spellslots[i - 1] > 0)
                {
                    var spellsInLevel = new List<PreparedSpell>();
                    var startStr = i == 0
                        ? "Cantrips (at will): "
                        : thIfy(i) + " level (" + spellslots[i - 1] + " slot" + (spellslots[i - 1] > 1 ? "s): " : "): ");
                    if (spellListClass.Equals("warlock"))
                    {

                        startStr = i == 0
                            ? "Cantrips (at will): "
                            : "1st-" + thIfy(i) + " level (" + spellslots[i - 1] + " " + thIfy(i) + "-level slot" + (spellslots[i - 1] > 1 ? "s): " : "): ");
                    }
                    var startIndex = spellcastingDescription.IndexOf(startStr, StringComparison.Ordinal);
                    if (startIndex != -1)
                    {
                        if (_spellTableStart == 0 || _spellTableStart > startIndex)
                            _spellTableStart = startIndex;
                        startIndex += startStr.Length;
                        var endIndex = spellcastingDescription.IndexOf("\n", startIndex, StringComparison.Ordinal);
                        if (endIndex == -1)
                            endIndex = spellcastingDescription.Length;
                        if (_spellTableEnd < endIndex)
                            _spellTableEnd = endIndex;
                        var spellsStr = spellcastingDescription.Substring(startIndex, endIndex - startIndex).Trim().Split(',');
                        foreach (var spell in spellsStr)
                        {
                            var marked = spell.Contains('*');
                            var name = spell.Trim().Trim('*');
                            var spellObj = spells.FirstOrDefault(s => s.Name.ToLower().Equals(name.ToLower()));
                            if (spellObj == null)
                            {
                                errors.Add("could not find spell " + name + " in description " + spellcastingDescription);
                            }
                            spellsInLevel.Add(new PreparedSpell(name, spellObj.Id, marked));
                        }
                        spellList[i] = spellsInLevel.ToArray();
                        continue;
                    }
                    errors.Add("could not find spell of level " + i + " in description " + spellcastingDescription);
                }
            }
            return spellList;
        }

        private string TryFindSpellListClass(string spellcastingDescription, ref List<string> errors)
        {
            var startStrings = new[] { "prepared from the ", "knows the following ", "has the following ", "has following " };
            foreach (var startStr in startStrings)
            {
                var stopStrings = new[] { " spells prepared", " spell", " spell list" };
                foreach (var stopStr in stopStrings)
                {
                    var startIndex = spellcastingDescription.IndexOf(startStr, StringComparison.Ordinal);
                    if (startIndex != -1)
                    {
                        startIndex += startStr.Length;
                        var stopIndex = spellcastingDescription.IndexOf(stopStr, startIndex, StringComparison.Ordinal);
                        if (stopIndex != -1)
                        {
                            var found = spellcastingDescription.Substring(startIndex, stopIndex - startIndex).Trim();
                            return found.Replace("spells", "").Replace("prepared", "").Replace("from", "").Replace("the", "").Trim();
                        }
                    }
                }
            }
            errors.Add("could not find spellcaster Class in description: " + spellcastingDescription);
            return "";
        }

        private int TryFindSpellcastingModifier(string spellcastingDescription, int spellDC, ref List<string> errors)
        {
            var guess = spellDC - 8;
            if (spellcastingDescription.Contains(guess + " to hit"))
                return guess;
            errors.Add("Modifier is wrong in description: " + spellcastingDescription);
            for (var i = 10; i < 40; i++)
            {
                if (spellcastingDescription.Contains(i + " to hit"))
                    return i;
            }
            return -1;
        }

        private int TryFindSpellsaveDC(string spellcastingDescription, ref List<string> errors)
        {
            for (var i = 10; i < 40; i++)
            {
                if (spellcastingDescription.Contains("save DC " + i))
                    return i;
            }
            errors.Add("Unable to find DC in decription: " + spellcastingDescription);
            return -1;
        }

        private string FixDescription(int oldLevel, int newLevel, string text)
        {
            text = text.Replace(" " + thIfy(oldLevel) + "-level spellcaster", " " + thIfy(newLevel) + "-level spellcaster");
            return text;
        }

        private int FindSpellCastingLevelBySlots(int[] spellslots, string spellcastingClass)
        {
            for (var i = 1; i <= 20; i++)
            {
                if (spellslots.SequenceEqual(GetSpellslotByLevel(i, spellcastingClass)))
                {
                    return i;
                }
            }
            return -1;
        }

        private int[] CheckSpellslotsByDescription(int[] spellslots, string spellListClass, string spellcastingDescription)
        {
            var slots = new int[9];
            for (var i = 0; i < 9; i++)
            {
                if (spellslots[i] > 0 && (spellcastingDescription.Contains(thIfy(i + 1) + " level (" + spellslots[i] + " slot")
                                          || spellListClass.Equals("warlock") && spellcastingDescription.Contains("1st-" + thIfy(i + 1) + " level (" + spellslots[i] + " " + thIfy(i + 1) + "-level slot")))
                {
                    slots[i] = spellslots[i];
                }
                else
                {
                    for (var j = 1; j < 10; j++)
                    {
                        if (spellcastingDescription.Contains(thIfy(i + 1) + " level (" + j + " slot")
                            || spellListClass.Equals("warlock") && spellcastingDescription.Contains("1st-" + thIfy(i + 1) + " level (" + j + " " + thIfy(i + 1) + "-level slot"))
                        {
                            slots[i] = j;
                        }
                    }
                }
            }
            return slots;
        }

        public int[] GetSpellslotByLevel(int level, string spellcastingClass)
        {
            var mod = 1;
            if (SpellcastingClassModifier.ContainsKey(spellcastingClass))
                mod = SpellcastingClassModifier[spellcastingClass];
            if (spellcastingClass.Equals("warlock"))
            {
                return DefaultSpellslotsByLevelWarlock[level - 1];
            }
            return DefaultSpellslotsByLevelFull[(level + (mod > 1 && level > 2 ? 1 : 0)) / mod];
        }

        private static readonly Dictionary<string, int> SpellcastingClassModifier = new Dictionary<string, int>
        {
            {"bard", 1 },
            {"cleric", 1 },
            {"druid", 1 },
            {"paladin", 2 },
            {"ranger", 2 },
            {"sorcerer", 1 },
            {"wizard", 1 }
        };
        private static readonly int[][] DefaultSpellslotsByLevelFull =
        {
            new[] {0, 0, 0, 0, 0, 0, 0, 0, 0}, //0
            new[] {2, 0, 0, 0, 0, 0, 0, 0, 0}, //1
            new[] {3, 0, 0, 0, 0, 0, 0, 0, 0}, //2
            new[] {4, 2, 0, 0, 0, 0, 0, 0, 0}, //3
            new[] {4, 3, 0, 0, 0, 0, 0, 0, 0}, //4
            new[] {4, 3, 2, 0, 0, 0, 0, 0, 0}, //5
            new[] {4, 3, 3, 0, 0, 0, 0, 0, 0}, //6
            new[] {4, 3, 3, 1, 0, 0, 0, 0, 0}, //7
            new[] {4, 3, 3, 2, 0, 0, 0, 0, 0}, //8
            new[] {4, 3, 3, 3, 1, 0, 0, 0, 0}, //9
            new[] {4, 3, 3, 3, 2, 0, 0, 0, 0}, //10
            new[] {4, 3, 3, 3, 2, 1, 0, 0, 0}, //11
            new[] {4, 3, 3, 3, 2, 1, 0, 0, 0}, //12
            new[] {4, 3, 3, 3, 2, 1, 1, 0, 0}, //13
            new[] {4, 3, 3, 3, 2, 1, 1, 0, 0}, //14
            new[] {4, 3, 3, 3, 2, 1, 1, 1, 0}, //15
            new[] {4, 3, 3, 3, 2, 1, 1, 1, 0}, //16
            new[] {4, 3, 3, 3, 2, 1, 1, 1, 1}, //17
            new[] {4, 3, 3, 3, 3, 1, 1, 1, 1}, //18
            new[] {4, 3, 3, 3, 3, 2, 1, 1, 1}, //19
            new[] {4, 3, 3, 3, 3, 2, 2, 1, 1} //20
        };
        private static readonly int[][] DefaultSpellslotsByLevelWarlock =
        {
            new[] {1, 0, 0, 0, 0, 0, 0, 0, 0}, //1
            new[] {2, 0, 0, 0, 0, 0, 0, 0, 0}, //2
            new[] {0, 2, 0, 0, 0, 0, 0, 0, 0}, //3
            new[] {0, 2, 0, 0, 0, 0, 0, 0, 0}, //4
            new[] {0, 0, 2, 0, 0, 0, 0, 0, 0}, //5
            new[] {0, 0, 2, 0, 0, 0, 0, 0, 0}, //6
            new[] {0, 0, 0, 2, 0, 0, 0, 0, 0}, //7
            new[] {0, 0, 0, 2, 0, 0, 0, 0, 0}, //8
            new[] {0, 0, 0, 0, 2, 0, 0, 0, 0}, //9
            new[] {0, 0, 0, 0, 2, 0, 0, 0, 0}, //10
            new[] {0, 0, 0, 0, 3, 0, 0, 0, 0}, //11
            new[] {0, 0, 0, 0, 3, 0, 0, 0, 0}, //12
            new[] {0, 0, 0, 0, 3, 0, 0, 0, 0}, //13
            new[] {0, 0, 0, 0, 3, 0, 0, 0, 0}, //14
            new[] {0, 0, 0, 0, 3, 0, 0, 0, 0}, //15
            new[] {0, 0, 0, 0, 3, 0, 0, 0, 0}, //16
            new[] {0, 0, 0, 0, 4, 0, 0, 0, 0}, //17
            new[] {0, 0, 0, 0, 4, 0, 0, 0, 0}, //18
            new[] {0, 0, 0, 0, 4, 0, 0, 0, 0}, //19
            new[] {0, 0, 0, 0, 4, 0, 0, 0, 0} //20
        };

        private Ability TryFindSpellcastingAbility(string spellcastingDescription, ref List<string> errors)
        {
            var desc = spellcastingDescription.ToLower();
            foreach (Ability attr in Enum.GetValues(typeof(Ability)))
            {
                if (desc.Contains("ability is " + attr.ToString().ToLower()))
                {
                    return attr;
                }
                if (desc.Contains("that uses " + attr.ToString().ToLower() + " as "))
                {
                    return attr;
                }
            }
            errors.Add("Unable to find spellcasting ability in description: " + spellcastingDescription);
            return Ability.Strength;
        }

        private int TryFindLevel(string spellcastingDescription, ref List<string> errors)
        {
            for (var i = 1; i <= 20; i++)
            {
                if (spellcastingDescription.Contains(" " + thIfy(i) + "-level spellcaster"))
                    return i;
            }
            errors.Add("Unable to find spellcasting level in description: " + spellcastingDescription);
            return 1;
        }
        private string thIfy(int num)
        {
            if (num == 1)
                return "1st";
            if (num == 2)
                return "2nd";
            if (num == 3)
                return "3rd";
            return num + "th";
        }
    }
}