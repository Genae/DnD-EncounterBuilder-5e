export class StatsByCr {
    public cr: number;
    public prof: number;
    public ac: number;
    public hp: number[];
    public atk: number;
    public dmg: number[];
    public save: number;
}

export class StatsByCrList {
    public static list:StatsByCr[] = [
        { cr: 0, prof: 2, ac: 13, hp: [1, 6], atk: 3, dmg: [0, 1], save: 13 },
        { cr: 1/8, prof: 2, ac: 13, hp: [7, 35], atk: 3, dmg: [2, 3], save: 13 },
        { cr: 1/4, prof: 2, ac: 13, hp: [36, 49], atk: 3, dmg: [4, 5], save: 13 },
        { cr: 1/2, prof: 2, ac: 13, hp: [50, 70], atk: 3, dmg: [6, 8], save: 13 },
        { cr: 1, prof: 2, ac: 13, hp: [71, 85], atk: 3, dmg: [9, 14], save: 13 },
        { cr: 2, prof: 2, ac: 13, hp: [86, 100], atk: 3, dmg: [15, 20], save: 13 },
        { cr: 3, prof: 2, ac: 13, hp: [101, 115], atk: 4, dmg: [21, 26], save: 13 },
        { cr: 4, prof: 2, ac: 14, hp: [116, 130], atk: 5, dmg: [27, 32], save: 14 },
        { cr: 5, prof: 3, ac: 15, hp: [131, 145], atk: 6, dmg: [33, 38], save: 15 },
        { cr: 6, prof: 3, ac: 15, hp: [146, 160], atk: 6, dmg: [39, 44], save: 15 },
        { cr: 7, prof: 3, ac: 15, hp: [161, 175], atk: 6, dmg: [45, 50], save: 15 },
        { cr: 8, prof: 3, ac: 16, hp: [176, 190], atk: 7, dmg: [51, 56], save: 16 },
        { cr: 9, prof: 4, ac: 16, hp: [191, 205], atk: 7, dmg: [57, 62], save: 16 },
        { cr: 10, prof: 4, ac: 17, hp: [206, 220], atk: 7, dmg: [63, 68], save: 16 },
        { cr: 11, prof: 4, ac: 17, hp: [221, 235], atk: 8, dmg: [69, 74], save: 17 },
        { cr: 12, prof: 4, ac: 17, hp: [236, 250], atk: 8, dmg: [75, 80], save: 18 },
        { cr: 13, prof: 5, ac: 18, hp: [251, 265], atk: 8, dmg: [81, 86], save: 18 },
        { cr: 14, prof: 5, ac: 18, hp: [266, 280], atk: 8, dmg: [87, 92], save: 18 },
        { cr: 15, prof: 5, ac: 18, hp: [281, 295], atk: 8, dmg: [93, 98], save: 18 },
        { cr: 16, prof: 5, ac: 18, hp: [296, 310], atk: 9, dmg: [99, 104], save: 18 },
        { cr: 17, prof: 6, ac: 19, hp: [311, 325], atk: 10, dmg: [105, 110], save: 19 },
        { cr: 18, prof: 6, ac: 19, hp: [326, 340], atk: 10, dmg: [111, 116], save: 19 },
        { cr: 19, prof: 6, ac: 19, hp: [341, 355], atk: 10, dmg: [117, 122], save: 19 },
        { cr: 20, prof: 6, ac: 19, hp: [356, 400], atk: 10, dmg: [123, 140], save: 19 },
        { cr: 21, prof: 7, ac: 19, hp: [401, 445], atk: 11, dmg: [141, 158], save: 20 },
        { cr: 22, prof: 7, ac: 19, hp: [446, 490], atk: 11, dmg: [159, 176], save: 20 },
        { cr: 23, prof: 7, ac: 19, hp: [491, 535], atk: 11, dmg: [177, 194], save: 20 },
        { cr: 24, prof: 7, ac: 19, hp: [536, 580], atk: 11, dmg: [195, 212], save: 21 },
        { cr: 25, prof: 8, ac: 19, hp: [581, 625], atk: 12, dmg: [213, 230], save: 21 },
        { cr: 26, prof: 8, ac: 19, hp: [626, 670], atk: 12, dmg: [231, 248], save: 21 },
        { cr: 27, prof: 8, ac: 19, hp: [671, 715], atk: 13, dmg: [249, 266], save: 22 },
        { cr: 28, prof: 8, ac: 19, hp: [716, 760], atk: 13, dmg: [267, 284], save: 22 },
        { cr: 29, prof: 9, ac: 19, hp: [760, 805], atk: 13, dmg: [285, 302], save: 22 },
        { cr: 30, prof: 9, ac: 19, hp: [805, 850], atk: 14, dmg: [303, 320], save: 23 },
    ]

    static findByDamage(dmgPerRound: any) {
        return StatsByCrList.list.find(l => l.dmg[0] <= dmgPerRound && l.dmg[1] >= dmgPerRound)
    }

    static findByCR(cr: any) {
        return StatsByCrList.list.find(l => l.cr == cr)
    }
    
    static findByHP(hp: any) {
        return StatsByCrList.list.find(l => l.hp[0] <= hp && l.hp[1] >= hp)
    }
}