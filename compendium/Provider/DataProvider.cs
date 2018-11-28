using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using compendium.Database;
using compendium.Models.CoreData;
using compendium.Models.ImportData;
using compendium.Parser;
using LiteDB;

namespace compendium.Provider
{
    public class DataProvider
    {
        private readonly IDatabaseConnection _db;
        public CompendiumRaw Compendium;

        public DataProvider(IDatabaseConnection db)
        {
            _db = db;
            ImportXML(db);
        }

        private void ImportXML(IDatabaseConnection db)
        {
            var srd = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "Data", "SRD.xml");
            if (File.Exists(@"D:\Dateien\OneDrive\Xerios\AllData.xml"))
            {
                Compendium = new Importer().ImportCompendium(@"D:\Dateien\OneDrive\Xerios\AllData.xml");
            }
            else if (File.Exists(srd))
            {
                Compendium = new Importer().ImportCompendium(srd);
            }
            else
            {
                return;
            }

            var monsterParser = new MonsterParser(new SpellcastingParser(), new ActionParser());
            var spellParser = new SpellParser();
            var allMonsters = GetAllMonsters();
            var allSpells = GetAllSpells();
            foreach (var compendiumSpell in Compendium.Spells)
            {
                if (allSpells.Any(m => m.Name.Equals(compendiumSpell.Name)))
                    continue;
                var spell = spellParser.Parse(compendiumSpell);
                db.Add(spell);
            }

            allSpells = GetAllSpells();
            foreach (var compendiumMonster in Compendium.Monsters)
            {
                if (allMonsters.Any(m => m.Name.Equals(compendiumMonster.Name)))
                    continue;
                var monster = monsterParser.Parse(compendiumMonster, allSpells);
                db.Add(monster);
            }
        }

        public Monster[] GetAllMonsters()
        {
            return _db.GetQueryable<Monster>().ToArray();
        }

        public List<Spell> GetAllSpells()
        {
            return _db.GetQueryable<Spell>().ToList();
        }

        public IEnumerable<Spell> GetAllSpellsWithIds(ObjectId[] ids)
        {
            return _db.GetQueryable<Spell>().Where(s => ids.Contains(s.Id)).ToArray();
        }
    }
}
