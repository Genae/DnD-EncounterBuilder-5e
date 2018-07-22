using System;

namespace encounter_builder.Models
{
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
}