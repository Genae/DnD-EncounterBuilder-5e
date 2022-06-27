using System;
using System.Collections.Generic;
using System.Linq;
using encounter_builder.Models.CoreData.Enums;
using LiteDB;

namespace encounter_builder.Models.CoreData
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

        public static Dictionary<string, AbilityScore> GetFromString(string abilities, ref List<string> errors)
        {
            try
            {
                var abilityArray = abilities.Trim(',').Split(',').Select(s => Convert.ToInt32((string) s)).ToArray();
                return new Dictionary<string, AbilityScore>
                {
                    {"Strength", new AbilityScore(abilityArray[0])},
                    {"Dexterity", new AbilityScore(abilityArray[1])},
                    {"Constitution", new AbilityScore(abilityArray[2])},
                    {"Intelligence", new AbilityScore(abilityArray[3])},
                    {"Wisdom", new AbilityScore(abilityArray[4])},
                    {"Charisma", new AbilityScore(abilityArray[5])}
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