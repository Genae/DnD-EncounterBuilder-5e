namespace encounter_builder.Models.CoreData
{
    public class PreparedSpell
    {
        public string Name;
        public int Index;
        public bool Marked;

        public PreparedSpell(string name, int index, bool marked)
        {
            Name = name;
            Index = index;
            Marked = marked;
        }
    }
}