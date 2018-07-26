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
            return compendium;
        }

        
    }
}
