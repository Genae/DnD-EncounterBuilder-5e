using System.Collections.Generic;
using compendiumOld.Models.CoreData.Enums;

namespace compendiumOld.Models.CoreData
{
    public class Speed
    {
        public Dictionary<MovementType, int> Speeds { get; set; } = new Dictionary<MovementType, int>();
        public string AdditionalInformation { get; set; }
    }
}