using Compendium.Database;
using Compendium.Models.CoreData.Enums;

namespace Compendium.Models.CoreData
{
    public class Spell : ProjectKeyedDocument
    {
        public string Name { get; set; }
        public SpellSchool School { get; set; }
        public bool CastAsRitual { get; set; }
        public int Level { get; set; }
        public string Time { get; set; }
        public CastingTime? CastingTime { get; set; }
        public string Range { get; set; }
        public bool VocalComponent { get; set; }
        public bool SomaticComponent { get; set; }
        public string Materials { get; set; }
        public string Duration { get; set; }
        public string Text { get; set; }
        public List<string> ClassLists { get; set; } = new List<string>();
        public bool IsMultiTarget { get; set; }
        public List<HitEffect> Effects { get; set; }
        public string AtHigherLevels { get; set; }
        public List<HitEffect> AtHigherLevelEffects { get; set; }
    }

    public enum CastingTime
    {
        Action,
        BonusAction,
        Reaction,
        AttackAction,
        Minute,
        _10Minutes,
        Hour,
        _8Hours,
        _12Hours,
        _24Hours,
        Varies
    }
}
