using LiteDB;
using Newtonsoft.Json;

namespace compendium.Models.CoreData
{
    public class PreparedSpell
    {
        public string Name { get; set; }

        [JsonConverter(typeof(ObjectIdConverter))]
        public ObjectId SpellId { get; set; }
        public bool Marked { get; set; }

        public PreparedSpell() { }

        public PreparedSpell(string name, ObjectId id, bool marked)
        {
            Name = name;
            SpellId = id;
            Marked = marked;
        }
    }
}