using Compendium.Database;
using Compendium.Models.CoreData;
using LiteDB;

namespace Compendium.Models.ProjectData
{
    public class Project : KeyedDocument
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public HashSet<ObjectId> MonsterIds { get; set; } = new();
        public HashSet<ObjectId> SpellIds { get; set; } = new();

        internal void StoreItem<T>(T obj) where T : ProjectKeyedDocument
        {
            if (typeof(T).IsAssignableFrom(typeof(Monster)))
            {
                MonsterIds.Add(obj.Id);
            }
            else if (typeof(T).IsAssignableFrom(typeof(Spell)))
            {
                SpellIds.Add(obj.Id);
            }
            else
            {
                throw new Exception($"Could not find List for {typeof(T)}");
            }
        }
    }
}
