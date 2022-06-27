using System.Collections.Generic;
using System.Xml.Serialization;

namespace compendiumOld.Models.ImportData
{
    public class SpellRaw
    {
        [XmlElement("name")]
        public string Name;
        [XmlElement("school")]
        public int? SchoolId;
        [XmlElement("ritual")]
        public int RitualId;
        [XmlElement("level")]
        public int Level;
        [XmlElement("time")]
        public string Time;
        [XmlElement("range")]
        public string Range;
        [XmlElement("v")]
        public int VocalId;
        [XmlElement("s")]
        public int SomaticId;
        [XmlElement("m")]
        public int MaterialId;
        [XmlElement("materials")]
        public string Materials;
        [XmlElement("duration")]
        public string Duration;
        [XmlElement("text")]
        public string Text;
        [XmlElement("sclass")]
        public List<string> ClassLists;

    }
}