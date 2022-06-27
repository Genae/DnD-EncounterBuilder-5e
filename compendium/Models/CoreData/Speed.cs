using System.Collections.Generic;
using Compendium.Models.CoreData.Enums;

namespace Compendium.Models.CoreData
{
    public class Speed
    {
        public Dictionary<MovementType, int> Speeds { get; set; } = new Dictionary<MovementType, int>();
        public string AdditionalInformation { get; set; }
    }
}