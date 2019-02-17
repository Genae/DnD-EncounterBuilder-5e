using NUnit.Framework;
using System;
using System.Linq;

namespace DiceTray.Tests
{
    [TestFixture]
    class TestExpressionParser
    {
        [Test]
        public void TestSingleExpressions()
        {
            var exp = ExpressionParser.Parse("1");
            Assert.AreEqual(1, exp.GetExpected());

            exp = ExpressionParser.Parse("1d6");
            Assert.AreEqual(3.5, exp.GetExpected());
        }

        [Test]
        public void TestSingleCalculation()
        {
            var exp = ExpressionParser.Parse("4/2");
            Assert.AreEqual(2, exp.GetExpected());

            exp = ExpressionParser.Parse("1d6*2");
            Assert.AreEqual(7, exp.GetExpected());

            exp = ExpressionParser.Parse("4+2");
            Assert.AreEqual(6, exp.GetExpected());

            exp = ExpressionParser.Parse("1d6+2");
            Assert.AreEqual(5.5, exp.GetExpected());
        }

        [Test]
        public void TestMultipleCalculations()
        {
            var exp = ExpressionParser.Parse("4/2/2");
            Assert.AreEqual(1, exp.GetExpected());

            exp = ExpressionParser.Parse("1d6*7/7/7");
            Assert.AreEqual(0.5, exp.GetExpected());

            exp = ExpressionParser.Parse("4+2-3");
            Assert.AreEqual(3, exp.GetExpected());

            exp = ExpressionParser.Parse("1d6+1d8+1");
            Assert.AreEqual(9, exp.GetExpected());

            exp = ExpressionParser.Parse("4+2*2-3");
            Assert.AreEqual(5, exp.GetExpected());

            exp = ExpressionParser.Parse("4*3-2*2");
            Assert.AreEqual(8, exp.GetExpected());
        }

        [Test]
        public void TestModifierParsing()
        {
            var exp = ExpressionParser.Parse("3d6ro<3 + 4");
            Console.WriteLine(exp.Roll());
            Console.WriteLine(exp.GetExpected());
            Console.WriteLine(exp.GetMax());
            Console.WriteLine(exp.GetMin(10000));
        }

        [Test]
        public void TestBrackets()
        {
            for (int i = 10; i < 30; i++)
            {
                var expectedDamage = ExpectedDamageAgainstAC(i);
                var expectedDamage2 = ExpectedDamageAgainstAC2(i);
                Console.WriteLine($"{Math.Max(expectedDamage, expectedDamage2)}");
            }
        }

        private static double ExpectedDamageAgainstAC(int ac)
        {
            var hitChance = ExpressionParser.Parse("2d20kh1+1d4+9");
            var dmg = ExpressionParser.Parse("2d6ro<3+1d10ro<3+18").GetExpected();
            var critChance = 3 / 20f;
            var critDmg = ExpressionParser.Parse("2d6ro<3+1d10ro<3+18").GetMax() + 3.5f;
            var arr = new int[10000];
            arr = arr.Select(i => (int) hitChance.Roll()).ToArray();
            var hChance = arr.Count(v => v > ac) / 10000f;
            return Math.Max(hChance-critChance, 0)*dmg*5 + (critChance * critDmg)*5 + critChance * dmg;
        }
        private static double ExpectedDamageAgainstAC2(int ac)
        {
            var hitChance = ExpressionParser.Parse("2d20kh1+1d4+14");
            var dmg = ExpressionParser.Parse("2d6ro<3+1d10ro<3+8").GetExpected();
            var critChance = 3 / 20f;
            var critDmg = ExpressionParser.Parse("2d6ro<3+1d10ro<3+8").GetMax() + 3.5f;
            var arr = new int[10000];
            arr = arr.Select(i => (int)hitChance.Roll()).ToArray();
            var hChance = arr.Count(v => v > ac) / 10000f;
            return Math.Max(hChance - critChance, 0) * dmg * 5 + (critChance * critDmg) * 5 + critChance * hChance * dmg;
        }
    }
}
