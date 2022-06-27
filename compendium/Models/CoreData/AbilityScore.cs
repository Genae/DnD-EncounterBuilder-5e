using System;
using System.Collections.Generic;
using System.Linq;
using Compendium.Models.CoreData.Enums;
using LiteDB;

namespace Compendium.Models.CoreData
{
    public class AbilityScore
    {
        public int Value { get; set; }
        public int Modifier { get; set; }
        [BsonIgnore]
        public string Description => ToString();

        public AbilityScore() { }

        public AbilityScore(int value)
        {
            Value = value;
            Modifier = value / 2 - 5;
        }

        public static Dictionary<Ability, AbilityScore> GetFromString(string abilities, ref List<string> errors)
        {
            try
            {
                var abilityArray = abilities.Trim(',').Split(',').Select(s => Convert.ToInt32(s)).ToArray();
                return new Dictionary<Ability, AbilityScore>
                {
                    {Ability.Strength, new AbilityScore(abilityArray[0])},
                    {Ability.Dexterity, new AbilityScore(abilityArray[1])},
                    {Ability.Constitution, new AbilityScore(abilityArray[2])},
                    {Ability.Intelligence, new AbilityScore(abilityArray[3])},
                    {Ability.Wisdom, new AbilityScore(abilityArray[4])},
                    {Ability.Charisma, new AbilityScore(abilityArray[5])}
                };
            }
            catch (Exception ex)
            {
                errors.Add("Error from parsing ability string" + abilities + ": " + ex);
                return null;
            }
        }

        public override string ToString()
        {
            if (Modifier < 0)
                return $"{Value} ({Modifier})";
            return $"{Value} (+{Modifier})";

        }
    }
}