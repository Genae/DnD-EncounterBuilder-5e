namespace Compendium.Models.CoreData
{
    public class Trait
    {
        public string Name { get; set; }
        public LimitedUsage? LimitedUsage { get; set; }
        public string Text { get; set; }
        public string LimitedUsageText => LimitedUsage.HasValue ? $" ({LimitedUsage.Value.ToDescriptionString()})" : "";
    }
}