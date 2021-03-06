﻿using System.Collections.Generic;
using compendium.Database;
using compendium.Models.CoreData.Enums;

namespace compendium.Models.CoreData
{
    public class Spell : KeyedDocument
    {
        public string Name { get; set; }
        public SpellSchool School { get; set; }
        public bool CastAsRitual { get; set; }
        public int Level { get; set; }
        public string Time { get; set; }
        public string Range { get; set; }
        public bool VocalComponent { get; set; }
        public bool SomaticComponent { get; set; }
        public string Materials { get; set; }
        public string Duration { get; set; }
        public string Text { get; set; }
        public List<string> ClassLists { get; set; } = new List<string>();
    }
}
