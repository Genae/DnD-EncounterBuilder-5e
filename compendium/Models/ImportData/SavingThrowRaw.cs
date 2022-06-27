using System.Xml.Serialization;
using Compendium.Models.CoreData.Enums;

namespace Compendium.Models.ImportData
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