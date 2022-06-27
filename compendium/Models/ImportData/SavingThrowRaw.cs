using System.Xml.Serialization;
using Compendium.Models.CoreData.Enums;
using Compendium.Provider;

namespace Compendium.Models.ImportData
{
    public class SavingThrowRaw
    {
        [XmlElement("ability")]
        public int AbilityId;
        [XmlElement("modifier")]
        public int Modifier;
        public string GetAbility(DynamicEnumProvider dep)
        {
            return dep.GetEnumValues("Ability").GetFromInt(AbilityId);
        }
    }
}