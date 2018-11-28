using System.Collections.Generic;
using compendium.Models.CoreData.Enums;

namespace compendium.Models.CoreData
{
    public class Speed
    {
        public Dictionary<MovementType, int> Speeds { get; set; } = new Dictionary<MovementType, int>();
        public string AdditionalInformation { get; set; }
    }
}