using System.Linq.Expressions;
using DiceTray.Expressions;

namespace DiceTray
{
    public class DiceTray
    {
        public static double Roll(string formula)
        {
            return new DiceExpression(formula).Roll();
        }

        public static double GetExpected(string formula)
        {
            return new DiceExpression(formula).GetExpected();
        }

        public static double GetMin(string formula)
        {
            return new DiceExpression(formula).GetMin();
        }

        public static double GetMax(string formula)
        {
            return new DiceExpression(formula).GetMax();
        }
    }
}
