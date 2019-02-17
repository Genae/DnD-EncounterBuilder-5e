using System;
using System.Linq;

namespace DiceTray.Expressions
{
    public class CalculationExpression : IExpression
    {
        public string Operator;
        public IExpression Left;
        public IExpression Right;

        public CalculationExpression(string Operator, IExpression left, IExpression right)
        {
            this.Operator = Operator;
            Left = left;
            Right = right;
            if (!new[] {"+", "-", "*", "/"}.Contains(Operator))
            {
                throw new Exception(Operator + " is not a valid Operator");
            }

            if (Left == null)
            {
                throw new Exception("Left value can not be null");
            }

            if (Right == null)
            {
                throw new Exception("Right value can not be null");
            }
        }

        public double Roll()
        {
            switch (Operator)
            {
                case "+":
                    return Left.Roll() + Right.Roll();
                case "-":
                    return Left.Roll() - Right.Roll();
                case "*":
                    return Left.Roll() * Right.Roll();
                case "/":
                    return Left.Roll() / Right.Roll();
                default:
                    return 0;
            }
        }

        public double GetMax(int tries = 100)
        {
            switch (Operator)
            {
                case "+":
                    return Left.GetMax(tries) + Right.GetMax(tries);
                case "-":
                    return Left.GetMax(tries) - Right.GetMin(tries);
                case "*":
                    return Left.GetMax(tries) * Right.GetMax(tries);
                case "/":
                    return Left.GetMax(tries) / Right.GetMin(tries);
                default:
                    return 0;
            }
        }

        public double GetMin(int tries = 100)
        {
            switch (Operator)
            {
                case "+":
                    return Left.GetMin(tries) + Right.GetMin(tries);
                case "-":
                    return Left.GetMin(tries) - Right.GetMax(tries);
                case "*":
                    return Left.GetMin(tries) * Right.GetMin(tries);
                case "/":
                    return Left.GetMin(tries) / Right.GetMax(tries);
                default:
                    return 0;
            }
        }

        public double GetExpected(int tries = 1000)
        {
            switch (Operator)
            {
                case "+":
                    return Left.GetExpected(tries) + Right.GetExpected(tries);
                case "-":
                    return Left.GetExpected(tries) - Right.GetExpected(tries);
                case "*":
                    return Left.GetExpected(tries) * Right.GetExpected(tries);
                case "/":
                    return Left.GetExpected(tries) / Right.GetExpected(tries);
                default:
                    return 0;
            }
        }
    }
}