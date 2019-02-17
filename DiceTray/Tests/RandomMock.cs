using System;

namespace DiceTray.Tests
{
    public class RandomMock : Random
    {
        private int _counter = 0;
        public override int Next(int minValue, int maxValue)
        {
            return _counter++%(maxValue-minValue) + minValue;
        }
        public override int Next(int maxValue)
        {
            return Next(0, maxValue);
        }
    }
}