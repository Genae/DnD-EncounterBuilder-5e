using Compendium.Database;
using LiteDB;
using System.Collections.Generic;

namespace Compendium.Models.ProjectData
{
    public class Project : KeyedDocument
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<ObjectId> MonsterIds { get; set; } = new();
        public List<ObjectId> SpellIds { get; set; } = new();
    }
}
