namespace Compendium.Models.CoreData
{
    public class ArmorInfo
    {
        public ArmorGroup Group { get; set; }
        public ArmorPiece Piece { get; set; }
        public bool HasShield { get; set; }
    }

    public enum ArmorGroup
    {
        NaturalArmor,
        LightArmor,
        MediumArmor,
        HeavyArmor
    }
    public enum ArmorPiece
    {
        Nat0,
        Nat1,
        Nat2,
        Nat3,
        Nat4,
        Nat5,
        Nat6,
        Nat7,
        Nat8,
        Nat9,
        Padded,
        Leather,
        StuddedLeather,
        Hide,
        ChainShirt,
        ScaleMail,
        Brestplate,
        HalfPlate,
        RingMail,
        ChainMail,
        Splint,
        Plate
    }
}