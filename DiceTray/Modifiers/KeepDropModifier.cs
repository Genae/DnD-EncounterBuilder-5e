using System;
using System.Text.RegularExpressions;

namespace DiceTray.Modifiers
{
    public class KeepDropModifier : IModifier
    {
        public string Modifier;
        public int Count;

        public KeepDropModifier(string formula)
        {
            var regex = new Regex("([k,d][h,l])([0-9]+)");
            var m = regex.Match(formula);
            if (!m.Value.Equals(formula))
            {
                throw new Exception(formula + " is not a valid KeepDropModifier");
            }

            Modifier = m.Groups[1].ToString();
            Count = Convert.ToInt32(m.Groups[2].ToString());
        }

        public int[] Apply(int[] rolls)
        {
            switch (Modifier)
            {
                case "kl":
                    return KeepLowest(rolls);
                case "kh":
                    return KeepHighest(rolls);
                case "dl":
                    return DropLowest(rolls);
                case "dh":
                    return DropHighest(rolls);
                default:
                    return null;
            }
        }


        private int[] KeepLowest(int[] rolls)
        {
            return SubArray(rolls, 0, Count);
        }

        private int[] KeepHighest(int[] rolls)
        {
            return SubArray(rolls, rolls.Length - Count, Count);
        }

        private int[] DropHighest(int[] rolls)
        {
            return SubArray(rolls, 0, rolls.Length - Count);
        }

        private int[] DropLowest(int[] rolls)
        {
            return SubArray(rolls, Count, rolls.Length - Count);
        }

        private static T[] SubArray<T>(T[] data, int index, int length)
        {
            T[] result = new T[length];
            Array.Copy(data, index, result, 0, length);
            return result;
        }
    }
}
