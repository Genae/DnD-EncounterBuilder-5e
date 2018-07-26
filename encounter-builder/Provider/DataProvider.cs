using System.Linq;
using encounter_builder.Database;
using encounter_builder.Models.ImportData;

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
            /*Compendium = new Importer().ImportCompendium(@"D:\Dateien\OneDrive\Xerios\AllData.xml");
            foreach (var compendiumMonster in Compendium.Monsters)
            {
                db.GetCollection<MonsterRaw>().Insert(compendiumMonster);
            }*/
        }

        public MonsterRaw[] GetAllMonsters()
        {
            return _db.GetCollection<MonsterRaw>().FindAll().ToArray();
        }
    }
}
