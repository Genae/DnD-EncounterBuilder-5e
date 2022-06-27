using System.Collections.Generic;

namespace Compendium.Models.CoreData.Enums
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