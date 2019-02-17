using System.Linq.Expressions;

namespace DiceTray.Expressions
{
    public interface IExpression
    {
        double Roll();
        double GetMax(int tries = 1000);
        double GetMin(int tries = 1000);
        double GetExpected(int tries = 10000);
    }
}