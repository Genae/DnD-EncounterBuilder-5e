using System.Collections.Generic;
using System.Linq;
using encounter_builder.Models.CoreData;
using encounter_builder.Provider;
using LiteDB;
using Microsoft.AspNetCore.Mvc;

namespace encounter_builder.Controllers
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
