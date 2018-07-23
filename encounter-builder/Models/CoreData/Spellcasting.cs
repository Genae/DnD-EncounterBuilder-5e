using System;
using System.Collections.Generic;
using System.Linq;
using encounter_builder.Database;
using encounter_builder.Models.ImportData;

namespace encounter_builder.Models.CoreData
{
    public class Spellcasting : TraitRaw
    {
        public int SpellcastingLevel;
        public Ability SpellcastingAbility;
        public int[] Spellslots;
        public int[] SpellslotsByLevel;
        public bool SpellSlotsCorrect => Spellslots.SequenceEqual(SpellslotsByLevel);
        public int SpellDC;
        public int SpellcastingModifier;
        public string SellListClass;
        public ReadiedSpell[][] Spells;

        public Spellcasting(string spellcastingDescription, List<SpellRaw> spells, Importer importer)
        {
            spellcastingDescription = spellcastingDescription
                .Replace("spell caster", "spellcaster")
                .Replace("level-spellcaster", "level spellcaster")
                .Replace(": ", ":")
                .Replace(":", ": ")
                .Replace("th level", "th-level")
                .Replace("Save DC", "save DC");
            Name = "Spellcasting";
            Text = spellcastingDescription;
            SpellcastingLevel = TryFindLevel(spellcastingDescription, importer);
            SpellcastingAbility = TryFindSpellcastingAbility(spellcastingDescription, importer);
            SpellslotsByLevel = GetSpellslotByLevel(SpellcastingLevel);
            Spellslots = CheckSpellslotsByDescription(SpellslotsByLevel, spellcastingDescription);
            if (!SpellSlotsCorrect)
            {
                var levelBySlots = FindSpellCastingLevelBySlots(Spellslots);
                if (levelBySlots != -1)
                {
                    spellcastingDescription = FixDescription(SpellcastingLevel, levelBySlots);
                    SpellcastingLevel = levelBySlots;
                    SpellslotsByLevel = GetSpellslotByLevel(SpellcastingLevel);
                }
            }
            SpellDC = TryFindSpellsaveDC(spellcastingDescription, importer);
            SpellcastingModifier = TryFindSpellcastingModifier(spellcastingDescription, SpellDC, importer);
            SellListClass = TryFindSpellListClass(spellcastingDescription, importer);
            Spells = TryFindSpells(Spellslots, spellcastingDescription, spells, importer);
        }

        private ReadiedSpell[][] TryFindSpells(int[] spellslots, string spellcastingDescription, List<SpellRaw> spells, Importer importer)
        {
            var spellList = new ReadiedSpell[10][];
            for (var i = 0; i <= 9; i++)
            {
                if (i == 0 || spellslots[i - 1] > 0)
                {
                    var spellsInLevel = new List<ReadiedSpell>();
                    var startStr = i == 0
                        ? "Cantrips (at will): "
                        : thIfy(i) + " level (" + spellslots[i-1] + " slot" + (spellslots[i - 1] > 1 ? "s): " : "): ");
                    var startIndex = spellcastingDescription.IndexOf(startStr, StringComparison.Ordinal);
                    if (startIndex != -1)
                    {
                        startIndex += startStr.Length;
                        var endIndex = spellcastingDescription.IndexOf("\n", startIndex, StringComparison.Ordinal);
                        if (endIndex == -1)
                            endIndex = spellcastingDescription.Length;
                        var spellsStr = spellcastingDescription.Substring(startIndex, endIndex - startIndex).Trim().Split(',');
                        foreach (var spell in spellsStr)
                        {
                            var name = spell.Trim().Trim('*');
                            var index = spells.FindIndex(s => s.Name.ToLower().Equals(name.ToLower()));
                            if (index == -1)
                            {
                                importer.Errors.Add("could not find spell " + name + " in description " + spellcastingDescription);
                            }
                            spellsInLevel.Add(new ReadiedSpell(name, index));
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
            var startStrings = new[] {"has the following ", "has following ", "knows the following ", "prepared from the "};
            foreach (var startStr in startStrings)
            {
                var stopStrings = new [] {" spells prepared", "spells:", " spell list"};
                foreach (var stopStr in stopStrings)
                {
                    var startIndex = spellcastingDescription.IndexOf(startStr, StringComparison.Ordinal);
                    if (startIndex != -1)
                    {
                        startIndex += startStr.Length;
                        var stopIndex = spellcastingDescription.IndexOf(stopStr, startIndex, StringComparison.Ordinal);
                        if (stopIndex != -1)
                        {
                            return spellcastingDescription.Substring(startIndex, stopIndex - startIndex).Trim();
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

        private int FindSpellCastingLevelBySlots(int[] spellslots)
        {
            for (var i = 0; i < DefaultSpellslotsByLevel.Length; i++)
            {
                if (spellslots.SequenceEqual(DefaultSpellslotsByLevel[i]))
                {
                    return i + 1;
                }
            }
            return -1;
        }

        private int[] CheckSpellslotsByDescription(int[] spellslots, string spellcastingDescription)
        {
            var slots = new int[9];
            for (var i = 0; i < 9; i++)
            {
                if (spellslots[i] > 0 && spellcastingDescription.Contains(thIfy(i + 1) + " level (" + spellslots[i] + " slot"))
                {
                    slots[i] = spellslots[i];
                }
                else
                {
                    for (var j = 1; j < 10; j++)
                    {
                        if (spellcastingDescription.Contains(thIfy(i + 1) + " level (" + j + " slot"))
                        {
                            slots[i] = j;
                        }
                    }
                }
            }
            return slots;
        }

        public int[] GetSpellslotByLevel(int level)
        {
            return DefaultSpellslotsByLevel[level - 1];
        }

        private static readonly int[][] DefaultSpellslotsByLevel =
        {
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

        public ReadiedSpell(string name, int index)
        {
            Name = name;
            Index = index;
        }
    }
}
