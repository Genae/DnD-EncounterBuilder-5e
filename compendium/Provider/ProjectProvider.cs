using Compendium.Database;
using Compendium.Models.CoreData;
using Compendium.Models.ProjectData;

namespace Compendium.Provider
{
    public class ProjectProvider : Provider<Project>
    {
        private readonly Provider<Monster> monsterProvider;
        private readonly Provider<Spell> spellProvider;

        public ProjectProvider(IDatabaseConnection db, Provider<Monster> monsterProvider, Provider<Spell> spellProvider) : base(db)
        {
            this.monsterProvider = monsterProvider;
            this.spellProvider = spellProvider;
        }

        public void RegisterAll()
        {
            (monsterProvider as ProjectDocumentProvider<Monster>)?.RegisterProjectProvider(this);
            (spellProvider as ProjectDocumentProvider<Spell>)?.RegisterProjectProvider(this);
        }

        public IEnumerable<string> GetAllProjectNames()
        {
            return new JsonDatabaseConnection().GetAllProjectNames();
        }
        public override bool Delete(Project obj)
        {
            var old = Get(obj.Id);
            if (old == null)
                return false;
            if (base.Delete(obj))
            {
                JsonDatabaseConnection.GetProjectDb(old.Name).Delete();
                return true;
            }
            return false;
        }

        public override Project Store(Project obj)
        {
            var old = Get(obj.Id);
            JsonDatabaseConnection jDb;
            if (old == null)
            {
                jDb = JsonDatabaseConnection.GetProjectDb(obj.Name);
            }
            else
            {
                jDb = JsonDatabaseConnection.GetProjectDb(old.Name);
                if (!old.Name.Equals(obj.Name))
                    jDb.Rename(obj.Name);
            }
            foreach (var monsters in monsterProvider.Get(m => obj.MonsterIds.Contains(m.Id)))
            {
                jDb.Store(monsters);
            }
            foreach (var spell in spellProvider.Get(m => obj.SpellIds.Contains(m.Id)))
            {
                jDb.Store(spell);
            }
            base.Store(obj);
            jDb.Store(obj);
            return obj;
        }

    }
}
