﻿using Compendium.Database;

namespace Compendium.Models.CoreData
{
    public class WeaponType : KeyedDocument
    {
        public WeaponType(WeaponCategory weaponCategory, string name, Attack attack, HitEffect hitEffect, string[] properties)
        {
            Id = ObjectIdExt.FromHash(name);
            WeaponCategory = weaponCategory;
            Name = name;
            Attack = attack;
            HitEffect = hitEffect;
            Properties = properties;
        }
        public WeaponType() { }

        public WeaponCategory WeaponCategory { get; set; }
        public string Name { get; set; }
        public Attack Attack { get; set; }
        public HitEffect HitEffect { get; set; }
        public string[] Properties { get; set; }
    }

    public enum WeaponCategory
    {
        Body,
        SimpleMeleeWeapon,
        SimpleRangedWeapon,
        MartialMeleeWeapon,
        MartialRangedWeapon
    }
}