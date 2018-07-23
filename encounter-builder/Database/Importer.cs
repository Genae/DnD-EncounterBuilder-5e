using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using encounter_builder.Models.ImportData;

namespace encounter_builder.Database
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
            var incorrectCasters = casters.Where(c => !c.Spellcasting.SpellSlotsCorrect);
            if(incorrectCasters.Any())
                throw new Exception("Incorrect Caster");
            return compendium;
        }

        
    }
}
