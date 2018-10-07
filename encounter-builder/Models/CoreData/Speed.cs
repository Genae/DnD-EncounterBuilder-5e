using System.Collections.Generic;
using encounter_builder.Models.CoreData.Enums;

namespace encounter_builder.Models.CoreData
{
    public class Speed
    {
        public Dictionary<MovementType, int> Speeds { get; set; } = new Dictionary<MovementType, int>();
        public string AdditionalInformation { get; set; }
    }
}