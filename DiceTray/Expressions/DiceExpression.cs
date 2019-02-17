using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using DiceTray.Modifiers;
using NUnit.Framework;

namespace DiceTray.Expressions
{
    public class DiceExpression : IExpression
    {
        public int Count;
        public int Sides;
        private Random _rand;
        private List<IModifier> _modifiers;

        public DiceExpression(string formula, Random random = null, List<IModifier> modifiers = null)
        {
            var regex = new Regex("([0-9]+)d([0-9]+)(ro?[<,>,=][0-9+])?([k,d][h,l][0-9]+)?");
            var m = regex.Match(formula);
            if (!m.Value.Equals(formula))
            {
                throw new Exception(formula + " is not a valid DiceExpression");
            }
            Count = Convert.ToInt32(m.Groups[1].Value);
            Sides = Convert.ToInt32(m.Groups[2].Value);
            _rand = random ?? new Random();
            _modifiers = modifiers ?? new List<IModifier>();
            if (m.Groups[3].Success)
            {
                if (_modifiers.All(mod => mod.GetType() != typeof(RerollModifier)))
                    _modifiers.Add(new RerollModifier(m.Groups[3].ToString(), Sides, _rand));
            }
            if (m.Groups[4].Success)
            {
                if(_modifiers.All(mod => mod.GetType() != typeof(KeepDropModifier)))
                    _modifiers.Add(new KeepDropModifier(m.Groups[4].ToString()));
            }
        }

        public double Roll()
        {
            var rolls = new int[Count];
            for (var i = 0; i < Count; i++)
            {
                rolls[i] = _rand.Next(Sides) + 1;
            }
            rolls = rolls.OrderBy(i => i).ToArray();

            if (_modifiers.Any(m => m.GetType() == typeof(RerollModifier)))
            {
                var mod = (RerollModifier)_modifiers.First(m => m.GetType() == typeof(RerollModifier));
                rolls = mod.Apply(rolls);
            }
            if(_modifiers.Any(m => m.GetType() == typeof(KeepDropModifier)))
            {
                var mod = (KeepDropModifier)_modifiers.First(m => m.GetType() == typeof(KeepDropModifier));
                rolls = mod.Apply(rolls);
            }
            return rolls.Sum();
        }

        public double GetExpected(int tries = 10000)
        {
            tries = tries * Count;
            if (_modifiers.Count == 0)
            {
                return Count * (Sides + 1) / 2f;
            }
            else
            {
                var rolls = new int[tries];
                return Math.Round(rolls.Select(i => Roll()).Sum()/tries, 2);
            }
        }

        public double GetMin(int tries = 1000)
        {
            tries = tries * Count;
            if (_modifiers.Count == 0)
            {
                return 1 * Count;
            }
            else
            {
                var rolls = new int[tries];
                return rolls.Select(i => Roll()).Min();
            }
        }

        public double GetMax(int tries = 1000)
        {
            tries = tries * Count;
            if (_modifiers.Count == 0)
            {
                return Sides * Count;
            }
            else
            {
                var rolls = new int[tries];
                return rolls.Select(i => Roll()).Max();
            }
        }
    }
}