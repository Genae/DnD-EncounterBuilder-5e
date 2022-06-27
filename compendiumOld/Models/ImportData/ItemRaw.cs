using System.Collections.Generic;
using System.Xml.Serialization;
using compendiumOld.Models.CoreData.Enums;

namespace compendiumOld.Models.ImportData
{
    public class ItemRaw
    {
        [XmlElement("name")]
        public string Name;
        [XmlElement("detail")]
        public string Detail;
        [XmlElement("text")]
        public string Text;
        [XmlElement("type")]
        public int? TypeId;
        [XmlElement("magic")]
        public int? MagicId;
        [XmlElement("value")]
        public float Value;
        [XmlElement("weight")]
        public float Weight;
        [XmlElement("ac")]
        public float AC;
        [XmlElement("stealth")]
        public int? StealthId;
        [XmlElement("damage1H")]
        public string Damage1H;
        [XmlElement("damage2H")]
        public string Damage2H;
        [XmlElement("damageType")]
        public int? DamageTypeId;
        [XmlElement("weaponRange")]
        public int WeaponRange;
        [XmlElement("weaponLongRange")]
        public int WeaponLongRange;
        [XmlElement("weaponProperty")]
        public int? WeaponPropertyId;

        [XmlElement("mod")]
        public List<ModifierRaw> Modidfiers;

        public bool IsMagic => MagicId == 1;
        public bool DisadvantageOnStealth => StealthId == 1;
        public ItemType? Type => (ItemType?)TypeId;
        public DamageType? DamageType => (DamageType?)DamageTypeId;
        public WeaponProperty? WeaponProperties => (WeaponProperty?)WeaponPropertyId;
    }
}