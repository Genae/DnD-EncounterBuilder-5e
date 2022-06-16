using compendium.Models.CoreData;
using System.Collections.Generic;

namespace compendium.Models.ProjectData
{
    public class Project
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<Monster> Monsters { get; set; }


    }
}
