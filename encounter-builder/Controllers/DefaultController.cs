using System.Collections.Generic;
using encounter_builder.Models.CoreData;
using encounter_builder.Provider;
using Microsoft.AspNetCore.Mvc;

namespace encounter_builder.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class DefaultController : Controller
    {
        private readonly DataProvider _dataProvider;

        public DefaultController(DataProvider dataProvider)
        {
            _dataProvider = dataProvider;
        }

        [HttpGet]
        public IEnumerable<Monster> Get(string y)
        {
            return _dataProvider.GetAllMonsters();
        }
    }
}
