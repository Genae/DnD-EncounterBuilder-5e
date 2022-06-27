using System.Collections.Generic;
using compendiumOld.Models.CoreData.Enums;

namespace compendiumOld.Models.CoreData
{
    public class Senses
    {
        public int PassivePerception { get; set; }
        public bool BlindOutsideRange { get; set; }
        public Dictionary<Sense, int> SenseRanges { get; set; } = new Dictionary<Sense, int>();
        public string Description { get; set; }
    }
}