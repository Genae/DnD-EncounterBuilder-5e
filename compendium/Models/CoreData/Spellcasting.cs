using Compendium.Models.CoreData.Enums;

namespace Compendium.Models.CoreData
{
    public class Spellcasting
    {
        public string Name { get; set; }
        public string Text { get; set; }
        public int SpellcastingLevel { get; set; }
        public string SpellcastingAbility { get; set; }
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
