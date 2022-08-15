namespace Compendium.Models.CoreData
{
    public class Multiattack
    {
        public string Name { get; set; }
        public string Text { get; set; }
        public Dictionary<string, int> Actions { get; set; }
    }
}
