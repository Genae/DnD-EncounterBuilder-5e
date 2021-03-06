﻿using System.Collections.Generic;

namespace compendium.Models.CoreData
{
    public class Action
    {
        public string Name { get; set; }
        public string Text { get; set; }
        public Attack Attack { get; set; }
        public List<HitEffect> HitEffects { get; set; } = new List<HitEffect>();
    }
}