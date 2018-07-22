using System;
using Newtonsoft.Json;

namespace encounter_builder.Models
{
    [Flags]
    [JsonConverter(typeof(FlagConverter))]
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