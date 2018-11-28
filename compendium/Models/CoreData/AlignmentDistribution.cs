using System.Collections.Generic;
using compendium.Models.CoreData.Enums;

namespace compendium.Models.CoreData
{
    public class AlignmentDistribution
    {
        public List<AlignmentChance> AlignmentChances { get; set; }
        public string Description { get; set; }

        public AlignmentDistribution()
        {
            AlignmentChances = new List<AlignmentChance>();
        }

        public AlignmentDistribution(Alignment alignment, string description)
        {
            AlignmentChances = new List<AlignmentChance>
            {
                new AlignmentChance(alignment, 1)
            };
            Description = description;
        }

        public static AlignmentDistribution Any(string description)
        {
            return new AlignmentDistribution
            {
                AlignmentChances = new List<AlignmentChance>
                {
                    new AlignmentChance(Morality.Good, Order.Lawful, 1/9f),
                    new AlignmentChance(Morality.Good, Order.Neutral, 1/9f),
                    new AlignmentChance(Morality.Good, Order.Chaotic, 1/9f),
                    new AlignmentChance(Morality.Neutral, Order.Lawful, 1/9f),
                    new AlignmentChance(Morality.Neutral, Order.Neutral, 1/9f),
                    new AlignmentChance(Morality.Neutral, Order.Chaotic, 1/9f),
                    new AlignmentChance(Morality.Evil, Order.Lawful, 1/9f),
                    new AlignmentChance(Morality.Evil, Order.Neutral, 1/9f),
                    new AlignmentChance(Morality.Evil, Order.Chaotic, 1/9f),
                },
                Description = description
            };
        }

        public static AlignmentDistribution Unaligned(string description)
        {
            return new AlignmentDistribution
            {
                AlignmentChances = new List<AlignmentChance>(),
                Description = description
            };
        }

        public static AlignmentDistribution Any(string description, Morality morality, float multiplier = 1f)
        {
            return new AlignmentDistribution
            {
                AlignmentChances = new List<AlignmentChance>()
                {
                    new AlignmentChance(morality, Order.Lawful, 1/3f * multiplier),
                    new AlignmentChance(morality, Order.Neutral, 1/3f * multiplier),
                    new AlignmentChance(morality, Order.Chaotic, 1/3f * multiplier)
                },
                Description = description
            };
        }
    }
}