using System.Xml.Serialization;
using encounter_builder.Provider;

namespace encounter_builder.Models.ImportData
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