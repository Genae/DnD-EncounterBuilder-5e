namespace encounter_builder.Models.CoreData
{
    public class PreparedSpell
    {
        public string Name { get; set; }
        public int Index { get; set; }
        public bool Marked { get; set; }

        public PreparedSpell() { }

        public PreparedSpell(string name, int index, bool marked)
        {
            Name = name;
            Index = index;
            Marked = marked;
        }
    }
}