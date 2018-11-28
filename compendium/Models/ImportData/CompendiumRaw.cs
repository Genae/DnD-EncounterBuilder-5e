using System.Collections.Generic;
using System.Xml.Serialization;

namespace compendium.Models.ImportData
{
    [XmlRoot("data")]
    public class CompendiumRaw
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
        public List<MonsterRaw> Monsters;
        [XmlElement("spell")]
        public List<SpellRaw> Spells;
    }
}
