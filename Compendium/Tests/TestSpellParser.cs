using Compendium.Database;
using Compendium.Models.ImportData;
using Compendium.Parser;
using Compendium.Provider;
using NUnit.Framework;

namespace Compendium.Tests;

[TestFixture]
public class TestSpellParser
{
    [Test]
    public void Test1()
    {
        var dep = new DynamicEnumProvider(new JsonDatabaseConnection());
        var ap = new SpellParser(dep);
        var spell = ap.Parse(new SpellRaw()
        {
            Name = "Acid Splash",
            Text = @"\n      You hurl a bubble of acid. Choose one creature within range, or choose two creatures within range that are within 5 feet of each other. A target must succeed on a Dexterity saving throw or take 1d6 acid damage.\n\n      This spells damage increases by 1d6 when you reach 5th Level (2d6), 11th level (3d6) and 17th level (4d6).\n    ",
            Duration = "Instantaneous",
            ClassLists = new List<string>() {"Sorcerer", "Wizard", "Fighter (Eldritch Knight)", "Rogue (Arcane Trickster)"},
            Level = 0,
            Range = "60 feet",
            SchoolId = 2,
            SomaticId = 1,
            VocalId = 1,
            Materials = "",
            Time = "1 action",
            MaterialId = 0,
            RitualId = 0
        });
    }
}