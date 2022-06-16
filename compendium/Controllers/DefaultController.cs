using compendium.Models.CoreData;
using compendium.Provider;
using LiteDB;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace compendium.Controllers
{
    [Produces("application/json")]
    [Route("api")]
    public class DefaultController : Controller
    {
        private readonly DataProvider _dataProvider;

        public DefaultController(DataProvider dataProvider)
        {
            _dataProvider = dataProvider;
        }

        [HttpGet]
        [Route("monsters")]
        public IEnumerable<Monster> GetAllMonsters()
        {
            return _dataProvider.GetAllMonsters();
        }

        [HttpGet]
        [Route("monsters/{id}")]
        public Monster GetAllMonsters(string id)
        {
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
