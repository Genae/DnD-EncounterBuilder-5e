using System.Xml.Serialization;

namespace encounter_builder.Models.CoreData
{
    public class DieRoll
    {
        public int Die;
        public int DieCount;
        public int Offset;
        [XmlIgnore]
        public int ExpectedRoll => (int)((Die / 2f + 0.5f) * DieCount + Offset);
        [XmlIgnore]
        public string Description => ToString();

        public DieRoll(int die, int dieCount, int offset)
        {
            Die = die;
            DieCount = dieCount;
            Offset = offset;
        }

        public override string ToString()
        {
            if(Offset > 0)
                return $"{DieCount}d{Die} + {Offset}";
            if (Offset < 0)
                return $"{DieCount}d{Die} - {Offset*-1}";
            return $"{DieCount}d{Die}";
        }
    }
}