using compendiumOld.Models.CoreData;
using compendiumOld.Models.CoreData.Enums;
using compendiumOld.Models.ImportData;

namespace compendiumOld.Parser
{
    public class SpellParser
    {
        public Spell Parse(SpellRaw raw)
        {
            return new Spell()
            {
                CastAsRitual = raw.RitualId == 1,
                ClassLists = raw.ClassLists,
                Duration = raw.Duration,
                Level = raw.Level,
                Materials = raw.Materials,
                Name = raw.Name,
                Range = raw.Range,
                School = raw.SchoolId.HasValue ? (SpellSchool)raw.SchoolId.Value : SpellSchool.None,
                SomaticComponent = raw.SomaticId == 1,
                Text = raw.Text,
                Time = raw.Time,
                VocalComponent = raw.VocalId == 1
            };
        }
    }
}