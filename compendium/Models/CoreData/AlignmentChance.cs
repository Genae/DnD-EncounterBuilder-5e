using Compendium.Models.CoreData.Enums;

namespace Compendium.Models.CoreData
{
    public class AlignmentChance
    {
        public Alignment Alignment { get; set; }
        public float Chance { get; set; }

        public AlignmentChance() { }

        public AlignmentChance(Alignment alignment, float chance)
        {
            Alignment = alignment;
            Chance = chance;
        }

        public AlignmentChance(Morality morality, Order order, float chance)
        {
            Alignment = new Alignment(morality, order);
            Chance = chance;
        }
    }
}