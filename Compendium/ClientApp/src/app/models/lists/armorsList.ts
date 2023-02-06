import {ArmorGroup, ArmorPiece} from "../monster";

export class Armors {
    public group: ArmorGroup;
    public name: string;
    public value: ArmorPiece;
    public ac: number;
}

export class ArmorsList {
    public static list: Armors[] = [
        { group: ArmorGroup.NaturalArmor, name: "No Armor (10 + Dex)", value: ArmorPiece.Nat0, ac: 0 },
        { group: ArmorGroup.NaturalArmor, name: "Thin Hide (11 + Dex)", value: ArmorPiece.Nat1, ac: 1 },
        { group: ArmorGroup.NaturalArmor, name: "Hide (12 + Dex)", value: ArmorPiece.Nat2, ac: 2 },
        { group: ArmorGroup.NaturalArmor, name: "Thick Hide (13 + Dex)", value: ArmorPiece.Nat3, ac: 3 },
        { group: ArmorGroup.NaturalArmor, name: "- (14 + Dex)", value: ArmorPiece.Nat4, ac: 4  },
        { group: ArmorGroup.NaturalArmor, name: "- (15 + Dex)", value: ArmorPiece.Nat5, ac: 5  },
        { group: ArmorGroup.NaturalArmor, name: "- (16 + Dex)", value: ArmorPiece.Nat6, ac: 6  },
        { group: ArmorGroup.NaturalArmor, name: "- (17 + Dex)", value: ArmorPiece.Nat7, ac: 7  },
        { group: ArmorGroup.NaturalArmor, name: "- (18 + Dex)", value: ArmorPiece.Nat8, ac: 8 },
        { group: ArmorGroup.NaturalArmor, name: "Steelplated Body (19 + Dex)", value: ArmorPiece.Nat9, ac: 9 },
        { group: ArmorGroup.LightArmor, name: "Padded (11 + Dex)", value: ArmorPiece.Padded, ac: 11 },
        { group: ArmorGroup.LightArmor, name: "Leather (11 + Dex)", value: ArmorPiece.Leather, ac: 11 },
        { group: ArmorGroup.LightArmor, name: "Studded Leather (12 + Dex)", value: ArmorPiece.StuddedLeather, ac: 12 },
        { group: ArmorGroup.MediumArmor, name: "Hide (12 + Dex)", value: ArmorPiece.Hide, ac: 12 },
        { group: ArmorGroup.MediumArmor, name: "Chain Shirt (13 + Dex)", value: ArmorPiece.ChainShirt, ac: 13 },
        { group: ArmorGroup.MediumArmor, name: "Scale Mail (14 + Dex)", value: ArmorPiece.ScaleMail, ac: 14 },
        { group: ArmorGroup.MediumArmor, name: "Breastplate (14 + Dex)", value: ArmorPiece.Brestplate, ac: 14 },
        { group: ArmorGroup.MediumArmor, name: "Half Plate (15 + Dex)", value: ArmorPiece.HalfPlate, ac: 15 },
        { group: ArmorGroup.HeavyArmor, name: "Ring Mail (14)", value: ArmorPiece.RingMail, ac: 14 },
        { group: ArmorGroup.HeavyArmor, name: "Chain Mail (16)", value: ArmorPiece.ChainMail, ac: 16 },
        { group: ArmorGroup.HeavyArmor, name: "Splint (17)", value: ArmorPiece.Splint, ac: 17 },
        { group: ArmorGroup.HeavyArmor, name: "Plate (18)", value: ArmorPiece.Plate, ac: 18 }
    ]
}