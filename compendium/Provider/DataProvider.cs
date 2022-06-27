
using Compendium.Database;
using Compendium.Models.CoreData;
using Compendium.Models.ImportData;
using Compendium.Models.ProjectData;
using Compendium.Parser;
using LiteDB;
using System.Reflection;

namespace Compendium.Provider
{
    public class DataProvider
    {
        private readonly IDatabaseConnection _db;
        public CompendiumRaw Compendium;

        public DataProvider(IDatabaseConnection db)
        {
            _db = db;
            ImportXML(db);
            LoadAllProjects(db);
        }

        private void ImportXML(IDatabaseConnection db)
        {
            var srd = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "Data", "SRD.xml");
            if (File.Exists(@"D:\Eigene Dateien\OneDrive\Xerios\AllData.xml"))
            {
                Compendium = new Importer().ImportCompendium(@"D:\Eigene Dateien\OneDrive\Xerios\AllData.xml");
            }
            else if (File.Exists(srd))
            {
                Compendium = new Importer().ImportCompendium(srd);
            }
            else
            {
                return;
            }

            var monsterParser = new MonsterParser(new SpellcastingParser(), new ActionParser(), new DynamicEnumProvider(db));
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

        private void LoadAllProjects(IDatabaseConnection db)
        {
            var allProjects = GetAllProjects();
            var allMonsters = GetAllMonsters();
            var allSpells = GetAllSpells();
            var jDb = new JsonDatabaseConnection();
            foreach (var project in jDb.GetAllProjectNames())
            {
                var pDb = JsonDatabaseConnection.GetProjectDb(project);
                foreach (var p in pDb.GetQueryable<Project>().Where(p => !allProjects.Any(mp => p.Id == mp.Id)).ToList())
                    db.Add(p);
                foreach (var m in pDb.GetQueryable<Monster>().Where(m => !allMonsters.Any(mm => m.Id == mm.Id)).ToList())
                    db.Add(m);
                foreach (var s in pDb.GetQueryable<Spell>().Where(s => !allSpells.Any(ms => s.Id == ms.Id)).ToList())
                    db.Add(s);
            }
        }

        internal IEnumerable<Monster> GetAllMonstersWithIds(ObjectId[] ids)
        {
            return _db.GetQueryable<Monster>().Where(s => ids.Contains(s.Id)).ToArray();
        }

        internal void DeleteProject(Project project)
        {
            var old = GetAllProjects().FirstOrDefault(p => p.Id.Equals(project.Id));
            if (old == null)
                return;
            JsonDatabaseConnection.GetProjectDb(old.Name).Delete();
            _db.Remove(project);
        }

        internal Project EditProject(Project project)
        {
            var old = GetAllProjects().FirstOrDefault(p => p.Id.Equals(project.Id));
            if (old == null)
                return CreateProject(project);

            var jDb = JsonDatabaseConnection.GetProjectDb(old.Name);
            if (!old.Name.Equals(project.Name))
                jDb.Rename(project.Name);
            _db.Update(project);
            jDb.Update(project);
            foreach (var monsters in GetAllMonsters().Where(m => project.MonsterIds.Contains(m.Id)))
            {
                jDb.Update(monsters);
            }
            foreach (var spell in GetAllSpells().Where(m => project.SpellIds.Contains(m.Id)))
            {
                jDb.Update(spell);
            }
            return project;
        }

        internal Project CreateProject(Project project)
        {
            _db.Add(project);
            JsonDatabaseConnection.GetProjectDb(project.Name).Add(project);
            return project;
        }

        internal IEnumerable<Project> GetAllProjects()
        {
            return _db.GetQueryable<Project>().ToArray();
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
