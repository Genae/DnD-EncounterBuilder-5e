using LiteDB;
using System.Xml.Serialization;

namespace Compendium.Models.CoreData
{
    public class DieRoll
    {
        public int Die { get; set; }
        public int DieCount { get; set; }
        public int Offset { get; set; }
        [XmlIgnore, BsonIgnore]
        public int ExpectedRoll => (int)((Die / 2f + 0.5f) * DieCount + Offset);
        [XmlIgnore, BsonIgnore]
        public string Description => ToString();

        public DieRoll() { }

        public DieRoll(int die, int dieCount, int offset)
        {
            Die = die;
            DieCount = dieCount;
            Offset = offset;
        }

        public override string ToString()
        {
            if (Die == 0)
                return Offset.ToString();
            if (Offset > 0)
                return $"{DieCount}d{Die} + {Offset}";
            if (Offset < 0)
                return $"{DieCount}d{Die} - {Offset * -1}";
            return $"{DieCount}d{Die}";
        }

        public override bool Equals(object obj)
        {
            return obj is DieRoll roll &&
                   Die == roll.Die &&
                   DieCount == roll.DieCount &&
                   Offset == roll.Offset;
        }
    }
}