using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using encounter_builder.Models.ImportData;

namespace encounter_builder.Parser
{
    public class Importer
    {
        public List<string> Errors = new List<string>();
        public CompendiumRaw ImportCompendium(string path)
        {
            string testData = File.ReadAllText(path);
            CompendiumRaw compendium;
            XmlSerializer serializer = new XmlSerializer(typeof(CompendiumRaw));
            using (TextReader reader = new StringReader(testData))
            {
                compendium = (CompendiumRaw)serializer.Deserialize(reader);
            }
            var casters = new List<MonsterRaw>();
            foreach (var compendiumMonster in compendium.Monsters)
            {
                if (compendiumMonster.CheckForSpellcasting(compendium.Spells, this)) 
                    casters.Add(compendiumMonster);
            }
            var incorrectCasters = casters.Where(c => !c.Spellcasting.SpellSlotsCorrect || Math.Abs(c.Spellcasting.SpellcastingLevel - c.Spellcasting.SpellcastingLevelByDescription) > 1);
            foreach (var incorrectCaster in incorrectCasters)
            {
                Errors.Add("Incorrect Caster Spellslots: " + incorrectCaster.Spellcasting.Text);
            }
                
            return compendium;
        }

        
    }
}
