import { Component, Input } from '@angular/core';

import { Monster, PreparedSpell, Size, MonsterType, Ability, ChallengeRating, ArmorGroup, ArmorPiece, DamageType, ArmorInfo, Condition, Morality, Order, Multiattack } from "../../models/monster";
import { Spell } from "../../models/spell";
import { Router, ActivatedRoute, ParamMap } from '@angular/router';
import { DataService } from "../../services/data.service";
import { FormControl } from '@angular/forms';

@Component({
    selector: 'monsterEdit',
    templateUrl: 'monsterEdit.component.html'
})

export class MonsterEditComponent {

    vul = new FormControl('');

    constructor(private dataService: DataService, private route: ActivatedRoute) {

        this.route.params.subscribe(params => {
            if (params['id'])
                this.dataService.getMonsterById(params['id']).subscribe(response => this.monsterUpdated(response as Monster));
        });
        this.dataService.getTags().subscribe(res => this.tags = res);
    }

    monster: Monster;
    hover: boolean;
    save: { [id: string]: boolean; } = {}
    proficency: number;
    alignment: { [id: string]: boolean; } = {};
    alignmentList: string[] = [];
    hasMultiattack: boolean;

    monsterSpells: Spell[];
    tags: { [id: string]: string; }

    public getTags() {
        let mtv = this.monsterTypeValues.find(mtv => this.monster.race.monsterType == mtv)
        if(mtv !== undefined)
            return this.tags[mtv];
        return "";
    }

    public hoverToggle() {
        if (this.hover) {
            this.monster.speed.speeds['Hover'] = this.monster.speed.speeds['Fly'];
            this.monster.speed.speeds['Fly'] = 0;
        }
        else {
            this.monster.speed.speeds['Fly'] = this.monster.speed.speeds['Hover'];
            this.monster.speed.speeds['Hover'] = 0;

        }
    }

    public getObjectKeys(dic: { [id: string]: number }) {
        return Object.keys(dic);
    }

    public removeActionFromMulti(action: string) {
        delete this.monster.multiattackAction.actions[action];
    }

    addActionToMultiSelection: string;

    public addActionToMulti() {
        this.monster.multiattackAction.actions[this.addActionToMultiSelection] = 1;
        this.addActionToMultiSelection = "";
    }

    public getUnusedMultiActions() {
        var used = this.getObjectKeys(this.monster.multiattackAction.actions);
        var actions = this.monster.actions.map(a => a.name);
        return actions.filter(a => !used.includes(a));
    }

    public hasMultiattackChange() {
        this.monster.multiattackAction = new Multiattack()
        this.monster.multiattackAction.name = "Multiattack"
    }

    public changeSave(ability: string) {
        if (this.save[ability])
            this.monster.savingThrows[ability] = this.monster.abilities[ability].modifier + this.proficency
        else
            delete this.monster.savingThrows[ability]
    }

    public monsterUpdated(monster: Monster) {
        this.monsterSpells = [];
        if (!monster.armorInfo)
            monster.armorInfo = new ArmorInfo();
        this.monster = monster;

        //fix dropdown values
        let fixCr = this.crValues.find(v => v.description === this.monster.challengeRating.description);
        if (fixCr !== undefined) {
            this.monster.challengeRating = fixCr;
        }

        if (monster.spellcasting !== undefined && monster.spellcasting.spells.length > 0) {
            var flattened = monster.spellcasting.spells.flat().filter((a: PreparedSpell) => a !== null);
            this.dataService.getSpellsFromIds(flattened.map((s: PreparedSpell) => s.spellId)).subscribe((data) => {
                this.monsterSpells = data;
            });
        }

        //fix hp
        this.recalcHP();

        //fix hover
        this.hover = this.monster.speed.speeds['Hover'] > 0;

        //fix save
        this.save = {};
        for (let ability of this.abilityValues) {
            if (this.monster.savingThrows[ability] > 0)
                this.save[ability] = true;
        }

        //set CR
        if (this.monster.challengeRating)
            this.setCr();

        //alignment grid
        for (let m of Object.values(Morality)) {
            for (let o of Object.values(Order)) {
                this.alignmentList.push(o + ' ' + m)
                this.alignment[o + ' ' + m] = this.monster.alignment.alignmentChances.find(a => a.alignment.morality === m && a.alignment.order === o) !== undefined;
            }
        }

        //multiattack
        this.hasMultiattack = this.monster.multiattackAction !== undefined;
    }

    public getSkills() {
        return Object.keys(this.monster.skillmodifiers)
    }

    public addSkillSelection: any;

    public addSkill() {
        this.monster.skillmodifiers[this.addSkillSelection] = this.getSkillDefaultValue(this.addSkillSelection);
        this.addSkillSelection = "";
    }

    public removeSkill(skill:string) {
        delete this.monster.skillmodifiers[skill];
    }

    public getSkillValues() {
        var active = Object.keys(this.monster.skillmodifiers);
        return Object.keys(this.skillList).filter(s => active.indexOf(s) === -1)
    }

    public getSkillDefaultValue(skill:string) {
        let ability = this.skillList[skill];
        return this.monster.abilities[ability].modifier + this.proficency
    }

    public abilityChange(ability: string) {
        this.monster.abilities[ability].modifier = parseInt((this.monster.abilities[ability].value / 2) + "") - 5
        if (ability === "Dexterity")
            this.recalcAc();
        if (ability === "Constitution")
            this.recalcHP()
    }

    public setCr() {
        var cr = this.crFromValue(this.monster.challengeRating.value);
        var line = this.statByCr.find(l => l.cr === cr);
        if(line)
            this.proficency = line.prof;
    }

    public crFromValue(val: number):number{
        if (val === -3)
            return 0
        if (val === -2)
            return 1/8
        if (val === -1)
            return 1/4
        if (val === 0)
            return 1 / 2
        return val;
    }

    public sizeValues = Object.values(Size);
    public dmgTypeValues = Object.values(DamageType);
    public conditionValues = Object.values(Condition);
    public monsterTypeValues = Object.values(MonsterType);
    public abilityValues = Object.values(Ability);

    public vulDesc(vul: DamageType[] | string[]): string {
        if (vul === undefined)
            return "";
        return vul.map(v => {v + ""}).join(", ")
    }

    public skillList: { [id: string]: string; } = {
        "Acrobatics": "Dexterity",
        "Animal_Handling": "Wisdom",
        "Arcana": "Intelligence",
        "Athletics": "Strength",
        "Deception": "Charisma",
        "History": "Intelligence",
        "Insight": "Wisdom",
        "Intimidation": "Charisma",
        "Investigation": "Intelligence",
        "Medicine": "Wisdom",
        "Nature": "Intelligence",
        "Perception": "Wisdom",
        "Performance": "Charisma",
        "Persuasion": "Charisma",
        "Religion": "Intelligence",
        "Sleight_Of_Hand": "Dexterity",
        "Stealth": "Dexterity",
        "Survival": "Wisdom"
    }


    public crValues: ChallengeRating[] = [
        { value: -3, experience: 0, description: "0 (0 XP)" },
        { value: -3, experience: 10, description: "0 (10 XP)" },
        { value: -2, experience: 25, description: "1/8 (25 XP)" },
        { value: -1, experience: 50, description: "1/4 (50 XP)" },
        { value: 0, experience: 100, description: "1/2 (100 XP)" },
        { value: 1, experience: 200, description: "1 (200 XP)" },
        { value: 2, experience: 450, description: "2 (450 XP)" },
        { value: 3, experience: 700, description: "3 (700 XP)" },
        { value: 4, experience: 1100, description: "4 (1.100 XP)" },
        { value: 5, experience: 1800, description: "5 (1.800 XP)" },
        { value: 6, experience: 2300, description: "6 (2.300 XP)" },
        { value: 7, experience: 2900, description: "7 (2.900 XP)" },
        { value: 8, experience: 3900, description: "8 (3.900 XP)" },
        { value: 9, experience: 5000, description: "9 (5.000 XP)" },
        { value: 10, experience: 5900, description: "10 (5.900 XP)" },
        { value: 11, experience: 7200, description: "11 (7.200 XP)" },
        { value: 12, experience: 8400, description: "12 (8.400 XP)" },
        { value: 13, experience: 10000, description: "13 (10.000 XP)" },
        { value: 14, experience: 11500, description: "14 (11.500 XP)" },
        { value: 15, experience: 13000, description: "15 (13.000 XP)" },
        { value: 16, experience: 15000, description: "16 (15.000 XP)" },
        { value: 17, experience: 18000, description: "17 (18.000 XP)" },
        { value: 18, experience: 20000, description: "18 (20.000 XP)" },
        { value: 19, experience: 22000, description: "19 (22.000 XP)" },
        { value: 20, experience: 25000, description: "20 (25.000 XP)" },
        { value: 21, experience: 33000, description: "21 (33.000 XP)" },
        { value: 22, experience: 41000, description: "22 (41.000 XP)" },
        { value: 23, experience: 50000, description: "23 (50.000 XP)" },
        { value: 24, experience: 62000, description: "24 (62.000 XP)" },
        { value: 25, experience: 75000, description: "25 (75.000 XP)" },
        { value: 26, experience: 90000, description: "26 (90.000 XP)" },
        { value: 27, experience: 105000, description: "27 (105.000 XP)" },
        { value: 28, experience: 120000, description: "28 (120.000 XP)" },
        { value: 29, experience: 135000, description: "29 (135.000 XP)" },
        { value: 30, experience: 155000, description: "30 (155.000 XP)" },
        { value: 31, experience: 175000, description: "31 (175.000 XP)" },
        { value: 32, experience: 195000, description: "32 (195.000 XP)" },
        { value: 33, experience: 215000, description: "33 (215.000 XP)" },
        { value: 34, experience: 240000, description: "34 (240.000 XP)" },
        { value: 35, experience: 265000, description: "35 (265.000 XP)" },
        { value: 36, experience: 290000, description: "36 (290.000 XP)" },
        { value: 37, experience: 315000, description: "37 (315.000 XP)" },
        { value: 38, experience: 345000, description: "38 (345.000 XP)" },
        { value: 39, experience: 375000, description: "39 (375.000 XP)" },
        { value: 40, experience: 405000, description: "40 (405.000 XP)" },
        { value: 41, experience: 435000, description: "41 (435.000 XP)" },
        { value: 42, experience: 475000, description: "42 (475.000 XP)" },
        { value: 43, experience: 515000, description: "43 (515.000 XP)" },
        { value: 44, experience: 555000, description: "44 (555.000 XP)" },
        { value: 45, experience: 595000, description: "45 (595.000 XP)" },
        { value: 46, experience: 635000, description: "46 (635.000 XP)" },
        { value: 47, experience: 685000, description: "47 (685.000 XP)" },
        { value: 48, experience: 735000, description: "48 (735.000 XP)" },
        { value: 49, experience: 835000, description: "49 (835.000 XP)" },
    ];

    public armorgroups = [
        { name: "Natural Armor", value: ArmorGroup.NaturalArmor },
        { name: "Light Armor", value: ArmorGroup.LightArmor },
        { name: "Medium Armor", value: ArmorGroup.MediumArmor },
        { name: "Heavy Armor", value: ArmorGroup.HeavyArmor }
    ]

    private armors = [
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


    public getArmorPieces(group: ArmorGroup) {
        return this.armors.filter(a => a.group === group);
    }

    public hasShieldChange() {
        this.recalcAc();
    }

    public acPieceChange() {
        this.recalcAc();
    }

    public recalcAc() {
        let shield = 0;
        if (this.monster.armorInfo.hasShield)
            shield = 2;
        if (this.monster.armorInfo.piece) {
            let piece = this.armors.find(n => this.monster.armorInfo.piece == n.value)
            if (!piece)
                return;
            this.monster.armorInfo.group = piece.group
            this.monster.armor = piece.name.split("(")[0].trim();
            switch (this.monster.armorInfo.group) {
                case ArmorGroup.NaturalArmor:
                    this.monster.armor = "natural armor";
                    this.monster.armorclass = 10 + this.monster.abilities["Dexterity"].modifier + piece.ac + shield;
                    break;
                case ArmorGroup.LightArmor:
                    this.monster.armorclass = this.monster.abilities["Dexterity"].modifier + piece.ac + shield;
                    break;
                case ArmorGroup.MediumArmor:
                    this.monster.armorclass = Math.min(2, this.monster.abilities["Dexterity"].modifier) + piece.ac + shield;
                    break;
                case ArmorGroup.HeavyArmor:
                    this.monster.armorclass = piece.ac + shield;
                    break;
            }
        }
        
        this.calcDefCR()
    }
    public acGroupChange() {
        this.monster.armorInfo.piece = undefined;
        this.monster.armor = "";
        this.recalcAc();
    }

    public sizeChanged() {
        var hd = this.hitDiceSize.find(hd => hd.size == this.monster.size);
        if (hd === undefined)
            return;
        this.monster.hitDie.die = hd.die;
        this.recalcHP();
    }

    public recalcHP() {
        let hd = this.monster.hitDie;
        hd.offset = hd.dieCount * this.monster.abilities["Constitution"].modifier;
        hd.expectedRoll = parseInt(((hd.dieCount * (hd.die + 1)) / 2 + hd.offset) + "");
        hd.description = "(" + hd.dieCount + "d" + hd.die + " + " + hd.offset + ")"
        this.monster.maximumHitpoints = hd.expectedRoll;
        this.calcDefCR();
    }

    public calcDefCR() {
        let hp = this.monster.hitDie.expectedRoll;
        let ac = this.monster.armorclass;
        let hpLine = this.statByCr.find(l => l.hp[0] <= hp && l.hp[1] >= hp);
        if (hpLine === undefined) return;
        let hpCR = hpLine.cr;
        let acCR = parseInt("" + ((ac - hpLine.ac) / 2))
        let defCR = hpCR + acCR;
    }

    public hitDiceSize = [
        { size: Size.Tiny, die: 4, dieStr: "d4" },
        { size: Size.Small, die: 6, dieStr: "d6" },
        { size: Size.Medium, die: 8, dieStr: "d8" },
        { size: Size.Large, die: 10, dieStr: "d10" },
        { size: Size.Huge, die: 12, dieStr: "d12" },
        { size: Size.Gargantuan, die: 20, dieStr: "d20" },
        { size: Size.Collosal, die: 20, dieStr: "d20" }
    ]

    public statByCr = [
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
}
