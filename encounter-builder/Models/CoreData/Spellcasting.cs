using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using encounter_builder.Database;
using encounter_builder.Models.ImportData;

namespace encounter_builder.Models.CoreData
{
    public class Spellcasting : TraitRaw
    {
        public int SpellcastingLevelByDescription;
        public int SpellcastingLevel;
        public Ability SpellcastingAbility;
        public int[] Spellslots;
        public int[] SpellslotsByLevel;
        public bool SpellSlotsCorrect => Spellslots.SequenceEqual(SpellslotsByLevel);
        public int SpellDC;
        public int SpellcastingModifier;
        public string SpellListClass;
        public ReadiedSpell[][] Spells;

        //spell Table
        private int _spellTableStart;
        private int _spellTableEnd;
        public string TextBeforeTable;
        public string TextAfterTable;
        public string OldTableText;

        public Spellcasting(string spellcastingDescription, List<SpellRaw> spells, Importer importer)
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
            Name = "Spellcasting";
            Text = spellcastingDescription;
            SpellcastingLevel = SpellcastingLevelByDescription = TryFindLevel(spellcastingDescription, importer);
            SpellListClass = TryFindSpellListClass(spellcastingDescription, importer);
            SpellcastingAbility = TryFindSpellcastingAbility(spellcastingDescription, importer);
            SpellslotsByLevel = GetSpellslotByLevel(SpellcastingLevel, SpellListClass);
            Spellslots = CheckSpellslotsByDescription(SpellslotsByLevel, SpellListClass, spellcastingDescription);
            if (!SpellSlotsCorrect)
            {
                var levelBySlots = FindSpellCastingLevelBySlots(Spellslots, SpellListClass);
                if (levelBySlots != -1)
                {
                    spellcastingDescription = FixDescription(SpellcastingLevel, levelBySlots);
                    SpellcastingLevel = levelBySlots;
                    SpellslotsByLevel = GetSpellslotByLevel(SpellcastingLevel, SpellListClass);
                }
            }
            SpellDC = TryFindSpellsaveDC(spellcastingDescription, importer);
            SpellcastingModifier = TryFindSpellcastingModifier(spellcastingDescription, SpellDC, importer);
            Spells = TryFindSpells(Spellslots, spellcastingDescription, SpellListClass, spells, importer);
            RemoveSpellTable(_spellTableStart, _spellTableEnd, spellcastingDescription);
        }

        private void RemoveSpellTable(int spellTableStart, int spellTableEnd, string spellcastingDescription)
        {
            var realStart = spellcastingDescription.LastIndexOf(':', spellTableStart) + 1;
            TextBeforeTable = spellcastingDescription.Substring(0, realStart);
            OldTableText = spellcastingDescription.Substring(realStart, spellTableEnd - realStart);
            TextAfterTable = spellcastingDescription.Substring(spellTableEnd, spellcastingDescription.Length - spellTableEnd);
        }

        private ReadiedSpell[][] TryFindSpells(int[] spellslots, string spellcastingDescription, string spellListClass, List<SpellRaw> spells, Importer importer)
        {
            var spellList = new ReadiedSpell[10][];
            for (var i = 0; i <= 9; i++)
            {
                if (i == 0 || spellslots[i - 1] > 0)
                {
                    var spellsInLevel = new List<ReadiedSpell>();
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
                            var index = spells.FindIndex(s => s.Name.ToLower().Equals(name.ToLower()));
                            if (index == -1)
                            {
                                importer.Errors.Add("could not find spell " + name + " in description " + spellcastingDescription);
                            }
                            spellsInLevel.Add(new ReadiedSpell(name, index, marked));
                        }
                        spellList[i] = spellsInLevel.ToArray();
                        continue;
                    }
                    importer.Errors.Add("could not find spell of level " + i + " in description " + spellcastingDescription);
                }
            }
            return spellList;
        }

        private string TryFindSpellListClass(string spellcastingDescription, Importer importer)
        {
            var startStrings = new[] {"prepared from the ", "knows the following ", "has the following ", "has following "};
            foreach (var startStr in startStrings)
            {
                var stopStrings = new [] {" spells prepared", " spell", " spell list"};
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
            importer.Errors.Add("could not find spellcaster Class in description: " + spellcastingDescription);
            return "";
        }

        private int TryFindSpellcastingModifier(string spellcastingDescription, int spellDC, Importer importer)
        {
            var guess = spellDC - 8;
            if (spellcastingDescription.Contains(guess + " to hit"))
                return guess;
            importer.Errors.Add("Modifier is wrong in description: " + spellcastingDescription);
            for (var i = 10; i < 40; i++)
            {
                if (spellcastingDescription.Contains(i + " to hit"))
                    return i;
            }
            return -1;
        }

        private int TryFindSpellsaveDC(string spellcastingDescription, Importer importer)
        {
            for (var i = 10; i < 40; i++)
            {
                if (spellcastingDescription.Contains("save DC " + i))
                    return i;
            }
            importer.Errors.Add("Unable to find DC in decription: " + spellcastingDescription);
            return -1;
        }

        private string FixDescription(int oldLevel, int newLevel)
        {
            Text = Text.Replace(" " + thIfy(oldLevel) + "-level spellcaster", " " + thIfy(newLevel) + "-level spellcaster");
            return Text;
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
            return DefaultSpellslotsByLevelFull[(level + (mod > 1 && level > 2 ? 1 : 0))/mod];
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

        private Ability TryFindSpellcastingAbility(string spellcastingDescription, Importer importer)
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
            importer.Errors.Add("Unable to find spellcasting ability in description: " + spellcastingDescription);
            return Ability.Strength;
        }

        private int TryFindLevel(string spellcastingDescription, Importer importer)
        {
            for (var i = 1; i <= 20; i++)
            {
                if (spellcastingDescription.Contains(" " + thIfy(i) + "-level spellcaster"))
                    return i;
            }
            importer.Errors.Add("Unable to find spellcasting level in description: " + spellcastingDescription);
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
    
    public class ReadiedSpell
    {
        public string Name;
        public int Index;
        public bool Marked;

        public ReadiedSpell(string name, int index, bool marked)
        {
            Name = name;
            Index = index;
            Marked = marked;
        }
    }
}
