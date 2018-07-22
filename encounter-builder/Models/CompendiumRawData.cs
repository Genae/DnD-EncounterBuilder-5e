using System.Collections.Generic;
using System.Xml.Serialization;

namespace encounter_builder.Models
{
    [XmlRoot("data")]
    public class CompendiumRawData
    {
        [XmlElement("version")]
        public int Version;
        [XmlElement("uid")]
        public int UID;
        [XmlElement("feat")]
        public List<FeatRaw> Feats;
        [XmlElement("item")]
        public List<ItemRaw> Items;
        [XmlElement("monster")]
        public List<MonsterRawData> Monsters;
    }
}
