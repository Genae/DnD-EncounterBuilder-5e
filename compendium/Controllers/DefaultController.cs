using System.Collections.Generic;
using System.Linq;
using compendium.Models.CoreData;
using compendium.Provider;
using LiteDB;
using Microsoft.AspNetCore.Mvc;

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

        [HttpGet]
        [Route("spells")]
        public IEnumerable<Spell> GetAllSpells()
        {
            return _dataProvider.GetAllSpells();
        }

        [HttpPost]
        [Route("spells")]
        public IEnumerable<Spell> GetAllSpells([FromBody]string[] ids)
        {
            return _dataProvider.GetAllSpellsWithIds(ids.Select(id => new ObjectId(id)).ToArray());
        }
    }
}
