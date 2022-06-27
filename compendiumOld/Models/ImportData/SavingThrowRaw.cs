using System.Xml.Serialization;
using compendiumOld.Models.CoreData.Enums;

namespace compendiumOld.Models.ImportData
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