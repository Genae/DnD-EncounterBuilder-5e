using System;
using System.Text.RegularExpressions;

namespace DiceTray.Expressions
{
    public class ConstantExpression : IExpression
    {
        public int Number;

        public ConstantExpression(string formula)
        {
            var regex = new Regex("(-?[0-9]+)");
            var m = regex.Match(formula);
            if (!m.Value.Equals(formula))
            {
                throw new Exception(formula + " is not a valid ConstantExpression");
            }
            Number = Convert.ToInt32(m.Groups[1].Value);
        }

        public double Roll()
        {
            return Number;
        }

        public double GetMax(int ties = 100)
        {
            return Number;
        }

        public double GetMin(int ties = 100)
        {
            return Number;
        }

        public double GetExpected(int ties = 100)
        {
            return Number;
        }
    }
}