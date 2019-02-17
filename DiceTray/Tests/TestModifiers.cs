using System;
using System.Collections.Generic;
using DiceTray.Expressions;
using DiceTray.Modifiers;
using NUnit.Framework;

namespace DiceTray.Tests
{
    [TestFixture]
    public class TestModifiers
    {
        [Test]
        public void TestKeepDropModifier()
        {
            var de = new DiceExpression("4d6dl1", new RandomMock());
            Assert.AreEqual(9, de.Roll());
            Assert.AreEqual(12.2, de.GetExpected(), 0.15);

            de = new DiceExpression("4d6kl1", new RandomMock());
            Assert.AreEqual(1, de.Roll());
            Assert.AreEqual(1.8, de.GetExpected(), 0.15);

            de = new DiceExpression("4d6dh1", new RandomMock());
            Assert.AreEqual(6, de.Roll());
            Assert.AreEqual(8.8, de.GetExpected(), 0.15);

            de = new DiceExpression("4d6kh1", new RandomMock());
            Assert.AreEqual(4, de.Roll());
            Assert.AreEqual(5.2, de.GetExpected(), 0.15);

            de = new DiceExpression("4d6dl2", new RandomMock());
            Assert.AreEqual(7, de.Roll());

            de = new DiceExpression("4d6kl2", new RandomMock());
            Assert.AreEqual(3, de.Roll());

            de = new DiceExpression("4d6dh2", new RandomMock());
            Assert.AreEqual(3, de.Roll());

            de = new DiceExpression("4d6kh2", new RandomMock());
            Assert.AreEqual(7, de.Roll());


            de = new DiceExpression("4d6kl2", new Random());
            var exp = de.GetExpected(10000);
            Console.WriteLine(exp);
            Assert.AreEqual(4.66, exp, 0.05);

            de = new DiceExpression("4d6kh2", new Random());
            exp = de.GetExpected(10000);
            Console.WriteLine(exp);
            Assert.AreEqual(9.34, exp, 0.05);
        }


    }
}