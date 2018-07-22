using System.IO;
using System.Xml.Serialization;
using encounter_builder.Models.ImportData;

namespace encounter_builder.Database
{
    public class Importer
    {

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
