using Compendium.Models.CoreData;
using Compendium.Provider;
using Microsoft.AspNetCore.Mvc;

namespace Compendium.Controllers
{
    [Route("api/spell")]
    public class SpellController : StandardController<Spell>
    {
        public SpellController(Provider<Spell> spellProvider, DynamicEnumProvider dep) : base(spellProvider, dep)
        { }
        
        [HttpGet]
        [Route("filtered")]
        public List<Spell> GetAll(string listClass, int level)
        {
            return provider.GetAll().Where(spell => spell.Level == level && spell.ClassLists.Any(c => c.Contains(listClass,StringComparison.InvariantCultureIgnoreCase))).ToList();
        }
    }

}
