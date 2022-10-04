using Compendium.Models.CoreData;
using Compendium.Provider;
using Microsoft.AspNetCore.Mvc;

namespace Compendium.Controllers
{
    [Route("api/weaponType")]
    public class WeaponTypeController : StandardController<WeaponType>
    {
        public WeaponTypeController(Provider<WeaponType> weaponTypeProvider, DynamicEnumProvider dep) : base(weaponTypeProvider, dep)
        { }
    }

}
