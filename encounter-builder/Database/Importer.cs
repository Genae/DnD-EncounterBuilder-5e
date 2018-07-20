using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;
using encounter_builder.Models;

namespace encounter_builder.Database
{
    public class Importer
    {

        public void ImportCompendium(string path)
        {
            string testData = File.ReadAllText(path);

            XmlSerializer serializer = new XmlSerializer(typeof(CompendiumRawData));
            using (TextReader reader = new StringReader(testData))
            {
                var result = (CompendiumRawData)serializer.Deserialize(reader);
            }
        }
    }
}
