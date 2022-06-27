using System;
using Newtonsoft.Json;

namespace compendiumOld.Models.CoreData.Enums
{
    [Flags]
    [JsonConverter(typeof(FlagConverter))]
    public enum WeaponProperty
    {
        Ammunition = 1,
        Finesse = 1 << 1,
        Heavy = 1 << 2,
        Light = 1 << 3,
        Loading = 1 << 4,
        Reach = 1 << 5,
        Special = 1 << 6,
        Throw = 1 << 7,
        Twohanded = 1 << 8,
        Versatile = 1 << 9,
        Martial_Weapon = 1 << 10
    }
}