using System.Xml.Serialization;
using encounter_builder.Models.CoreData.Enums;

namespace encounter_builder.Models.ImportData
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