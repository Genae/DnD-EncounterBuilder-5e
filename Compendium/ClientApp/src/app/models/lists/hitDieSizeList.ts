import {Size} from "../monster";

export class HitDieSize {
    public size: Size;
    public die: number;
    public dieStr: string;
}

export class HitDieSizeList{
    public static list: HitDieSize[] = [
        { size: Size.Tiny, die: 4, dieStr: "d4" },
        { size: Size.Small, die: 6, dieStr: "d6" },
        { size: Size.Medium, die: 8, dieStr: "d8" },
        { size: Size.Large, die: 10, dieStr: "d10" },
        { size: Size.Huge, die: 12, dieStr: "d12" },
        { size: Size.Gargantuan, die: 20, dieStr: "d20" },
        { size: Size.Collosal, die: 20, dieStr: "d20" }
    ]
}