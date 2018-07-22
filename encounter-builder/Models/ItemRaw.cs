using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace encounter_builder.Models
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
        public ItemType? Type => (ItemType?) TypeId;
        public DamageType? DamageType => (DamageType?)DamageTypeId;
        public WeaponProperty? WeaponProperties => (WeaponProperty?)WeaponPropertyId;
    }

    [Flags]
    public enum WeaponProperty
    {
        Ammunition,
        Finesse,
        Heavy,
        Light,
        Loading,
        Reach,
        Special,
        Throw,
        Twohanded,
        Veratile,
        Martial_Weapon
    }

    public enum DamageType
    {
        Acid,
        Bludgeoning,
        Cold,
        Fire,
        Force,
        Lightning,
        Necrotic,
        Piercing,
        Poison,
        Psychic,
        Radiant,
        Slashing,
        Thunder
    }

    public enum ItemType
    {
        Unknown00,
        Light_Armor,
        Medium_Armor,
        Heavy_Armor,
        Shield,
        Melee_Weapon,
        Ranged_Weapon,
        Ammunition,
        Rod,
        Staff,
        Wand,
        Ring,
        Potion,
        Scroll,
        Wondrous_Item,
        Wealth
    }

    public class ModifierRaw
    {
        [XmlElement("type")]
        public int StatId;
        [XmlElement("value")]
        public int Value;

        public ModifierStat Stat => (ModifierStat) StatId;
    }

    public enum ModifierStat
    {
        Unknown0,
        WeaponAttack,
        WeaponDamage,
        Unknown3,
        Unknown4,
        Unknown5,
        Unknown6,
        SpellAttack,
        Unknown8,
        Unknown9,
        Armorclass,
        Unknown11
    }
}