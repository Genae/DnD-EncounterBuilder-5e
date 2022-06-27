using LiteDB;

namespace compendiumOld.Models.CoreData
{
    public class ChallengeRating
    {
        public int Value { get; set; }
        public int Experience { get; set; }
        [BsonIgnore]
        public string Description => ToString();
        public ChallengeRating() { }

        private static readonly int[] ExperienceTable = {
            0,
            10,
            25,
            50,
            100,
            200,
            450,
            700,
            1100,
            1800,
            2300,
            2900,
            3900,
            5000,
            5900,
            7200,
            8400,
            10000,
            11500,
            13000,
            15000,
            18000,
            20000,
            22000,
            25000,
            33000,
            41000,
            50000,
            62000,
            75000,
            90000,
            105000,
            120000,
            135000,
            155000,
            175000,
            195000,
            215000,
            240000,
            265000,
            290000,
            315000,
            345000,
            375000,
            405000,
            435000,
            475000,
            515000,
            555000,
            595000,
            635000,
            685000,
            735000,
            835000
        };

        public ChallengeRating(int cr)
        {
            Value = cr;
            Experience = ExperienceTable[cr + 4];
        }

        public override string ToString()
        {
            var cr = Value + "";
            if (Value == 0)
                cr = "1/2";
            if (Value == -1)
                cr = "1/4";
            if (Value == -2)
                cr = "1/8";
            if (Value == -3)
                cr = "0";
            return $"{cr} ({Experience:N0} XP)";
        }
    }
}