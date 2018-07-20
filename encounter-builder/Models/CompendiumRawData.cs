using System.Collections.Generic;
using System.Xml.Serialization;

namespace encounter_builder.Models
{
    [XmlRoot("compendium")]
    public class CompendiumRawData
    {
        [XmlElement("version")]
        public int Version;
        [XmlElement("monster")]
        public List<MonsterRawData> Monsters;
    }
}
