﻿using encounter_builder.Models.ImportData;

namespace encounter_builder.Models.CoreData
{
    public class Spellcasting : TraitRaw
    {
        public int SpellcastingLevel { get; set; }
        public Ability SpellcastingAbility { get; set; }
        public int[] Spellslots { get; set; }
        public int SpellDC { get; set; }
        public int SpellcastingModifier { get; set; }
        public string SpellListClass { get; set; }
        public PreparedSpell[][] Spells { get; set; }

        //Text
        public string TextBeforeTable { get; set; }
        public string TextAfterTable { get; set; }
    }
}