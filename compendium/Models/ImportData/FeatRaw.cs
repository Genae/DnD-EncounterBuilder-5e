using System.Xml.Serialization;

namespace compendium.Models.ImportData
{
    public class FeatRaw
    {
        [XmlElement("name")]
        public string Name;
        [XmlElement("text")]
        public string Text;
        [XmlElement("prereq")]
        public string Prereq;
    }
}
