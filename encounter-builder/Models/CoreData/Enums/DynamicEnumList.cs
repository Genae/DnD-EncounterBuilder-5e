using System.Collections.Generic;

namespace encounter_builder.Models.CoreData.Enums
{
    public partial class DynamicEnumList
    {
        protected List<DynamicEnum> DefaultEnums = new List<DynamicEnum>();

        public static List<DynamicEnum> GetDefaults()
        {
            return new DynamicEnumList().DefaultEnums;
        }
    }
}