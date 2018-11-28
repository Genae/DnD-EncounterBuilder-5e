using System.Xml.Serialization;
using compendium.Models.CoreData.Enums;

namespace compendium.Models.ImportData
{
    public class SavingThrowRaw
    {
        [XmlElement("ability")]
        public int AbilityId;
        [XmlElement("modifier")]
        public int Modifier;
        public Ability Ability => (Ability)AbilityId;
    }
}