using System.Collections.Generic;

namespace encounter_builder.Models.CoreData.Enums
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