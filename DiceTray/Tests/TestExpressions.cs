using System;
using System.Collections.Generic;
using DiceTray.Expressions;
using NUnit.Framework;

namespace DiceTray.Tests
{
    [TestFixture]
    public class TestExpressions
    {
        [Test]
        public void TestDieExpression()
        {
            var de = new DiceExpression("1d6");
            Assert.AreEqual(3.5, de.GetExpected());
            Assert.AreEqual(1, de.GetMin());
            Assert.AreEqual(6, de.GetMax());

            de = new DiceExpression("2d6");
            Assert.AreEqual(7, de.GetExpected());
            Assert.AreEqual(2, de.GetMin());
            Assert.AreEqual(12, de.GetMax());

            de = new DiceExpression("1d12");
            Assert.AreEqual(6.5, de.GetExpected());
            Assert.AreEqual(1, de.GetMin());
            Assert.AreEqual(12, de.GetMax());

            de = new DiceExpression("1d6", new Random(0));
            var results = new Dictionary<int, int>()
            {
                {1, 0},
                {2, 0},
                {3, 0},
                {4, 0},
                {5, 0},
                {6, 0}
            };
            for (var i = 0; i < 6000; i++)
            {
                results[(int) de.Roll()]++;
            }

            Assert.LessOrEqual(Math.Abs(results[1] - 1000), 50);
            Assert.LessOrEqual(Math.Abs(results[2] - 1000), 50);
            Assert.LessOrEqual(Math.Abs(results[3] - 1000), 50);
            Assert.LessOrEqual(Math.Abs(results[4] - 1000), 50);
            Assert.LessOrEqual(Math.Abs(results[5] - 1000), 50);
            Assert.LessOrEqual(Math.Abs(results[6] - 1000), 50);
        }

        [Test]
        public void TestConstantExpression()
        {
            var ce = new ConstantExpression("5");
            Assert.AreEqual(5, ce.GetExpected());
            Assert.AreEqual(5, ce.GetMin());
            Assert.AreEqual(5, ce.GetMax());
            Assert.AreEqual(5, ce.Roll());

            ce = new ConstantExpression("-5");
            Assert.AreEqual(-5, ce.GetExpected());
            Assert.AreEqual(-5, ce.GetMin());
            Assert.AreEqual(-5, ce.GetMax());
            Assert.AreEqual(-5, ce.Roll());
        }

        [Test]
        public void TestCalculationExpressionWithConstants()
        {
            var const1 = new ConstantExpression("1");
            var const2 = new ConstantExpression("-1");

            var ce = new CalculationExpression("+", const1, const2);
            Assert.AreEqual(0, ce.GetExpected());
            Assert.AreEqual(0, ce.GetMin());
            Assert.AreEqual(0, ce.GetMax());
            Assert.AreEqual(0, ce.Roll());

            ce = new CalculationExpression("-", const1, const2);
            Assert.AreEqual(2, ce.GetExpected());
            Assert.AreEqual(2, ce.GetMin());
            Assert.AreEqual(2, ce.GetMax());
            Assert.AreEqual(2, ce.Roll());

            ce = new CalculationExpression("*", const1, const2);
            Assert.AreEqual(-1, ce.GetExpected());
            Assert.AreEqual(-1, ce.GetMin());
            Assert.AreEqual(-1, ce.GetMax());
            Assert.AreEqual(-1, ce.Roll());

            ce = new CalculationExpression("/", const1, const2);
            Assert.AreEqual(-1, ce.GetExpected());
            Assert.AreEqual(-1, ce.GetMin());
            Assert.AreEqual(-1, ce.GetMax());
            Assert.AreEqual(-1, ce.Roll());
        }

        [Test]
        public void TestCalculationExpressionWithDies()
        {
            var d1 = new DiceExpression("1d6");
            var d2 = new DiceExpression("1d8");

            var ce = new CalculationExpression("+", d1, d2);
            Assert.AreEqual(8, ce.GetExpected());
            Assert.AreEqual(2, ce.GetMin());
            Assert.AreEqual(14, ce.GetMax());

            ce = new CalculationExpression("-", d1, d2);
            Assert.AreEqual(-1, ce.GetExpected());
            Assert.AreEqual(-7, ce.GetMin());
            Assert.AreEqual(5, ce.GetMax());

            ce = new CalculationExpression("*", d1, d2);
            Assert.AreEqual(3.5 * 4.5, ce.GetExpected());
            Assert.AreEqual(1, ce.GetMin());
            Assert.AreEqual(48, ce.GetMax());

            ce = new CalculationExpression("/", d1, d2);
            Assert.AreEqual(3.5 / 4.5, ce.GetExpected());
            Assert.AreEqual(0.125, ce.GetMin());
            Assert.AreEqual(6, ce.GetMax());
        }

        [Test]
        public void TestCalculationExpressionWithCalculations()
        {
            var const1 = new ConstantExpression("1");
            var const2 = new ConstantExpression("-1");
            var d1 = new DiceExpression("1d6");
            var d2 = new DiceExpression("1d8");

            var ce = new CalculationExpression("+", new CalculationExpression("+", d1, d2), new CalculationExpression("+", const1, const2));
            Assert.AreEqual(8, ce.GetExpected());
            Assert.AreEqual(2, ce.GetMin());
            Assert.AreEqual(14, ce.GetMax());
        }
    }
}