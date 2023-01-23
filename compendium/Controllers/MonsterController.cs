using Compendium.Models.CoreData;
using Compendium.Models.CoreData.Enums;
using Compendium.Provider;
using Microsoft.AspNetCore.Mvc;
using Action = Compendium.Models.CoreData.Action;

namespace Compendium.Controllers
{
    [Route("api/monster")]
    public class MonsterController : StandardController<Monster>
    {
        public MonsterController(Provider<Monster> monsterProvider, DynamicEnumProvider dep) : base(monsterProvider, dep)
        { }

        [HttpGet]
        [Route("cr")]
        public Dictionary<int, AbilityDistribution> GetAllCrAbilityPoints()
        {
            return provider.GetAll().GroupBy(m => m.ChallengeRating.Value).ToDictionary(g => g.Key, g => new AbilityDistribution(g.ToList()));
        }

        [HttpGet]
        [Route("tags")]
        public Dictionary<MonsterType, string[]> GetTagsForType()
        {
            return provider.GetAll().
                GroupBy(m => m.Race.MonsterType).
                ToDictionary(g => g.Key, g => g.ToList().
                    Select(m => m.Race.Tags).
                    Where(t => t != null).
                    Distinct().
                    ToArray());
        }
        
        [HttpGet]
        [Route("traits")]
        public Dictionary<string, Trait[]> GetTraitsForType()
        {
            string GroupKey(Monster m)
            {
                if (!string.IsNullOrWhiteSpace(m.Race.Tags))
                    return $"{m.Race.MonsterType} ({m.Race.Tags})";
                return m.Race.MonsterType.ToString();
            }

            return provider.GetAll().GroupBy(GroupKey).ToDictionary(g => g.Key, g => g.ToList()
                .SelectMany(m => m.Traits)
                .GroupBy(t => t.Name)
                .ToDictionary(i => i.Key, i => i.ToList().First().Text)
                .Select(t => new Trait() { Name = t.Key, Text = t.Value })
                .ToArray());
        }
    }

    public class AbilityDistribution
    {
        public int AbilityMax;
        public double AbilityAvgMax;
        public int AbilityMin;
        public double AbilityAvgMin;
        public double AbilityAvg;

        public int PointsMax;
        public int PointsMin;
        public double PointsAvg;

        public AbilityDistribution(List<Monster> monsters)
        {
            AbilityMax = monsters.Max(m => m.Abilities.Values.Max(a => a.Value));
            AbilityAvgMax = monsters.Average(m => m.Abilities.Values.Max(a => a.Value));
            AbilityMin = monsters.Min(m => m.Abilities.Values.Min(a => a.Value));
            AbilityAvgMin = monsters.Average(m => m.Abilities.Values.Min(a => a.Value));
            AbilityAvg = monsters.Average(m => m.Abilities.Values.Average(a => a.Value));

            var points = monsters.Select(m => GetPoints(m.Abilities));
            PointsMax = points.Max();
            PointsMin = points.Min();
            PointsAvg = points.Average();
        }

        private int GetPoints(Dictionary<string, AbilityScore> values)
        {
            var sum = 0;
            foreach (var v in values.Values.Where(v => v.Value < 30))
                sum += points[v.Value] - 10;
            return sum;
        }

        private static int[] points = new[] { 0, 0, 2, 4, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 17, 19, 21, 24, 27, 30, 33, 37, 41, 45, 49, 54, 59, 64, 69, 75, 81 };
    }
}
