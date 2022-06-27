using System.Collections.Generic;

namespace Compendium.Models.CoreData.Enums
{
    public partial class DynamicEnumList
    {
        public DynamicEnumList()
        {
            DefaultEnums.Add(new DynamicEnum
            {
                Name = "Ability",
                Data = new List<string>
                {
                    "Strength",
                    "Dexterity",
                    "Constitution",
                    "Intelligence",
                    "Wisdom",
                    "Charisma"
                }
            });
        }
    }
}