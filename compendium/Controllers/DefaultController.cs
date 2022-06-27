using Compendium.Models.CoreData;
using Compendium.Models.CoreData.Enums;
using Compendium.Provider;
using LiteDB;
using Microsoft.AspNetCore.Mvc;

namespace Compendium.Controllers
{
    [Produces("application/json")]
    [Route("api")]
    public class DefaultController : Controller
    {
        private readonly DataProvider _dataProvider;
        private readonly DynamicEnumProvider dep;

        public DefaultController(DataProvider dataProvider, DynamicEnumProvider dep)
        {
            _dataProvider = dataProvider;
            this.dep = dep;
        }

        [HttpGet]
        [Route("monsters")]
        public IEnumerable<Monster> GetAllMonsters()
        {
            return _dataProvider.GetAllMonsters();
        }

        [HttpGet]
        [Route("cr")]
        public Dictionary<int, AbilityDistribution> GetAllCrAbilityPoints()
        {
            return _dataProvider.GetAllMonsters().GroupBy(m => m.ChallengeRating.Value).ToDictionary(g => g.Key, g => new AbilityDistribution(g.ToList()));
        }

        [HttpGet]
        [Route("tags")]
        public Dictionary<MonsterType, string[]> GetTagsForType()
        {
            return _dataProvider.GetAllMonsters().
                GroupBy(m => m.Race.MonsterType).
                ToDictionary(g => g.Key, g => g.ToList().
                    Select(m => m.Race.Tags).
                    Where(t => t != null).
                    Distinct().
                    ToArray());
        }


        [HttpGet]
        [Route("monsters/{id}")]
        public Monster GetMonsterById(string id)
        {
            if (id == "0")
                return new Monster(dep);
            return _dataProvider.GetAllMonsters().FirstOrDefault(m => m.Id.ToString().Equals(id));
        }

        [HttpPost]
        [Route("monsters")]
        public IEnumerable<Monster> GetAllMonsters([FromBody] string[] ids)
        {
            return _dataProvider.GetAllMonstersWithIds(ids.Select(id => new ObjectId(id)).ToArray());
        }

        [HttpGet]
        [Route("spells")]
        public IEnumerable<Spell> GetAllSpells()
        {
            return _dataProvider.GetAllSpells();
        }

        [HttpGet]
        [Route("spells/{id}")]
        public Spell GetSpell(string id)
        {
            return _dataProvider.GetAllSpells().FirstOrDefault(s => s.Id.ToString().Equals(id));
        }

        [HttpPost]
        [Route("spells")]
        public IEnumerable<Spell> GetAllSpells([FromBody] string[] ids)
        {
            return _dataProvider.GetAllSpellsWithIds(ids.Select(id => new ObjectId(id)).ToArray());
        }
    }
}
