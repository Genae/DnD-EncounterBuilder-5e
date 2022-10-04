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
    }

}
