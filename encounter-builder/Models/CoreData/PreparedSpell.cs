using LiteDB;

namespace encounter_builder.Models.CoreData
{
    public class PreparedSpell
    {
        public string Name { get; set; }
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