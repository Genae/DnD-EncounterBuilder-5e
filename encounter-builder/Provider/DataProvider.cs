using System.Linq;
using encounter_builder.Database;
using encounter_builder.Models.CoreData;
using encounter_builder.Models.ImportData;
using encounter_builder.Parser;

namespace encounter_builder.Provider
{
    public class DataProvider
    {
        private readonly DatabaseConnection _db;
        public CompendiumRaw Compendium;

        public DataProvider(DatabaseConnection db)
        {
            _db = db;
            //Compendium = new Importer().ImportCompendium(Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "Data", "SRD.xml"));
            Compendium = new Importer().ImportCompendium(@"D:\Eigene Dateien\OneDrive\Xerios\AllData.xml");
            var parser = new MonsterParser(new SpellcastingParser(), new ActionParser());
            foreach (var compendiumMonster in Compendium.Monsters)
            {
                var monster = parser.Parse(compendiumMonster, Compendium.Spells);
                db.Add(monster);
            }
        }

        public Monster[] GetAllMonsters()
        {
            return _db.GetQueryable<Monster>().ToArray();
        }
    }
}
