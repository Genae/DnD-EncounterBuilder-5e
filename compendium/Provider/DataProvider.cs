using Compendium.Database;
using Compendium.Models.CoreData;
using Compendium.Models.ImportData;
using Compendium.Models.ProjectData;
using Compendium.Parser;
using LiteDB;
using System.Reflection;

namespace Compendium.Provider
{

    public class DataLoader
    {
        private readonly Provider<Spell> spellProvider;
        private readonly Provider<Monster> monsterProvider;
        private readonly ProjectProvider projectProvider;
        private readonly MonsterParser monsterParser;
        private readonly SpellParser spellParser;
        public CompendiumRaw Compendium;

        public DataLoader(Provider<Spell> spellProvider, Provider<Monster> monsterProvider, ProjectProvider projectProvider, DynamicEnumProvider dynamicEnumProvider)
        {
            this.spellProvider = spellProvider;
            this.monsterProvider = monsterProvider;
            this.projectProvider = projectProvider;
            spellParser = new SpellParser(dynamicEnumProvider);
            monsterParser = new MonsterParser(new SpellcastingParser(), new ActionParser(), dynamicEnumProvider);
        }

        public void LoadData()
        {
            ImportXML();
            LoadAllProjects();
        }

        private void ImportXML()
        {
            var oneDrive = Directory.Exists(@"C:\Users\stefa\OneDrive\Xerios")
                ? @"C:\Users\stefa\OneDrive\Xerios"
                : @"D:\Dateien\OneDrive\Xerios";
            var srd = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "Data", "SRD.xml");
            if (File.Exists(oneDrive + @"\AllData.xml"))
            {
                Compendium = new Importer().ImportCompendium(oneDrive + @"\AllData.xml");
            }
            else if (File.Exists(srd))
            {
                Compendium = new Importer().ImportCompendium(srd);
            }
            else
            {
                return;
            }

            var allMonsters = monsterProvider.GetAll();
            var allSpells = spellProvider.GetAll();
            foreach (var compendiumSpell in Compendium.Spells)
            {
                if (allSpells.Any(m => m.Name.Equals(compendiumSpell.Name)))
                    continue;
                var spell = spellParser.Parse(compendiumSpell);
                spellProvider.Store(spell);
            }
            allSpells = spellProvider.GetAll();

            foreach (var compendiumMonster in Compendium.Monsters)
            {
                if (allMonsters.Any(m => m.Name.Equals(compendiumMonster.Name)))
                    continue;
                var monster = monsterParser.Parse(compendiumMonster, allSpells);
                monsterProvider.Store(monster);
            }
        }

        private void LoadAllProjects()
        {
            var allProjects = projectProvider.GetAll();
            var allMonsters = monsterProvider.GetAll();
            var allSpells = spellProvider.GetAll();
            foreach (var project in projectProvider.GetAllProjectNames())
            {
                var pDb = JsonDatabaseConnection.GetProjectDb(project);
                foreach (var p in pDb.GetQueryable<Project>().Where(p => !allProjects.Any(mp => p.Id == mp.Id)).ToList())
                    projectProvider.Store(p);
                foreach (var m in pDb.GetQueryable<Monster>().Where(m => !allMonsters.Any(mm => m.Id == mm.Id)).ToList())
                    monsterProvider.Store(m);
                foreach (var s in pDb.GetQueryable<Spell>().Where(s => !allSpells.Any(ms => s.Id == ms.Id)).ToList())
                    spellProvider.Store(s);
            }
        }
    }
}
