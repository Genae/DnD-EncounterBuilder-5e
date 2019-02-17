using System;
using System.Text.RegularExpressions;

namespace DiceTray.Modifiers
{
    public class RerollModifier : IModifier
    {
        public bool Once;
        public string Comparator;
        public int CompareValue;
        private int _diceSides;
        private readonly Random _random;

        public RerollModifier(string formula, int diceSides, Random random)
        {
            var regex = new Regex("r(o?)([<,>,=])([0-9+])");
            var m = regex.Match(formula);
            if (!m.Value.Equals(formula))
            {
                throw new Exception(formula + " is not a valid RerollModifier");
            }

            Once = m.Groups[1].Value.Length > 0;
            Comparator = m.Groups[2].ToString();
            CompareValue = Convert.ToInt32(m.Groups[3].ToString());
            _diceSides = diceSides;
            _random = random;

            if(!HasValidRolls())
                throw new Exception("A 1d" + diceSides + " has no valid rolls for the modifier " + formula);
        }

        private bool HasValidRolls()
        {
            for (var i = 1; i <= _diceSides; i++)
            {
                if (!ShouldReroll(i))
                    return true;
            }
            return false;
        }

        public int[] Apply(int[] rolls)
        {
            for (int i = 0; i < rolls.Length; i++)
            {
                if (ShouldReroll(rolls[i]))
                {
                    if (Once)
                    {
                        Reroll(rolls, i);
                    }
                    else
                    {
                        while(ShouldReroll(rolls[i]))
                            Reroll(rolls, i);
                    }
                }
            }
            return rolls;
        }

        private void Reroll(int[] rolls, int i)
        {
            rolls[i] = _random.Next(_diceSides) + 1;
        }

        private bool ShouldReroll(int roll)
        {
            switch (Comparator)
            {
                case "<":
                    return roll < CompareValue;
                case ">":
                    return roll > CompareValue;
                case "=":
                    return roll == CompareValue;
                default:
                    return false;
            }
        }
    }
}