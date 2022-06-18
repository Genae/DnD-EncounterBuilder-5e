import { Component, Input } from '@angular/core';

import { Monster, PreparedSpell, Size, MonsterType, Ability, ChallengeRating, ArmorGroup, ArmorPiece } from "../../models/monster";
import { Spell } from "../../models/spell";
import { Router, ActivatedRoute, ParamMap } from '@angular/router';
import { DataService } from "../../services/data.service";

@Component({
    selector: 'monsterEdit',
    templateUrl: 'monsterEdit.component.html'
})

export class MonsterEditComponent {

    constructor(private dataService: DataService, private route: ActivatedRoute) {

        this.route.params.subscribe(params => {
            if (params['id'])
                this.dataService.getMonsterById(params['id']).subscribe(response => this.monsterUpdated(response as Monster));
        });
        this.dataService.getTags().subscribe(res => this.tags = res);
    }

    monster: Monster;
    monsterSpells: Spell[];
    tags: { [id: string]: string; }

    public getTags() {
        return this.tags[this.monsterTypeValues.find(mtv => this.monster.race.monsterType == mtv.value).key];
    }

    public monsterUpdated(monster: Monster) {
        this.monsterSpells = [];
        this.monster = monster;

        //fix dropdown values
        this.monster.challengeRating = this.crValues.find(v => v.description === this.monster.challengeRating.description);

        if (monster.spellcasting !== undefined && monster.spellcasting.spells.length > 0) {
            var flattened = [].concat.apply([], monster.spellcasting.spells).filter((a: PreparedSpell) => a !== null);
            this.dataService.getSpellsFromIds(flattened.map((s: PreparedSpell) => s.spellId)).subscribe((data) => {
                this.monsterSpells = data;
            });
        }
    }

    public abilityChange(ability: string) {
        this.monster.abilities[ability].modifier = parseInt((this.monster.abilities[ability].value / 2) + "") - 5
        if (ability == "Dexterity")
            this.recalcAc();
    }

    public setCr() {
    }

    public sizeValues = Object.values(Size).filter(key => !isNaN(Number(Size[key]))).map(
        o => { return { key: o, value: Size[o] } }
    );
    public monsterTypeValues = Object.values(MonsterType).filter(key => !isNaN(Number(MonsterType[key]))).map(
        o => { return { key: o, value: MonsterType[o] } }
    );
    public abilityValues = Object.keys(Ability).filter(key => !isNaN(Number(Ability[key])));

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

    private agNat = [
        { name: "No Armor (10 + Dex)", value: ArmorPiece.Nat0, ac: 0 },
        { name: "Thin Hide (11 + Dex)", value: ArmorPiece.Nat1, ac: 1 },
        { name: "Hide (12 + Dex)", value: ArmorPiece.Nat2, ac: 2 },
        { name: "Thick Hide (13 + Dex)", value: ArmorPiece.Nat3, ac: 3 },
        { name: "- (14 + Dex)", value: ArmorPiece.Nat4, ac: 4  },
        { name: "- (15 + Dex)", value: ArmorPiece.Nat5, ac: 5  },
        { name: "- (16 + Dex)", value: ArmorPiece.Nat6, ac: 6  },
        { name: "- (17 + Dex)", value: ArmorPiece.Nat7, ac: 7  },
        { name: "- (18 + Dex)", value: ArmorPiece.Nat8, ac: 8 },
        { name: "Steelplated Body (19 + Dex)", value: ArmorPiece.Nat9, ac: 9 }
    ]

    private agLight = [
        { name: "Padded (11 + Dex)", value: ArmorPiece.Padded, ac: 11 },
        { name: "Leather (11 + Dex)", value: ArmorPiece.Leather, ac: 11 },
        { name: "Studded Leather (12 + Dex)", value: ArmorPiece.StuddedLeather, ac: 12 },
    ]

    private agMedium = [
        { name: "Hide (12 + Dex)", value: ArmorPiece.Hide, ac: 12 },
        { name: "Chain Shirt (13 + Dex)", value: ArmorPiece.ChainShirt, ac: 13 },
        { name: "Scale Mail (14 + Dex)", value: ArmorPiece.ScaleMail, ac: 14 },
        { name: "Breastplate (14 + Dex)", value: ArmorPiece.Brestplate, ac: 14 },
        { name: "Half Plate (15 + Dex)", value: ArmorPiece.HalfPlate, ac: 15 },
    ]

    private agHeavy = [
        { name: "Ring Mail (14)", value: ArmorPiece.RingMail, ac: 14 },
        { name: "Chain Mail (16)", value: ArmorPiece.ChainMail, ac: 16 },
        { name: "Splint (17)", value: ArmorPiece.Splint, ac: 17 },
        { name: "Plate (18)", value: ArmorPiece.Plate, ac: 18 }
    ]


    public getArmorPieces(group: ArmorGroup) {
        switch (group) {
            case ArmorGroup.NaturalArmor:
                return this.agNat;
            case ArmorGroup.LightArmor:
                return this.agLight;
            case ArmorGroup.MediumArmor:
                return this.agMedium;
            case ArmorGroup.HeavyArmor:
                return this.agHeavy;
        }
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
        switch (this.monster.armorInfo.group) {
            case ArmorGroup.NaturalArmor:
                let piece = this.agNat.find(n => this.monster.armorInfo.piece == n.value)
                this.monster.armor = "natural armor";
                this.monster.armorclass = 10 + this.monster.abilities["Dexterity"].modifier + piece.ac + shield;
                break;
            case ArmorGroup.LightArmor:
                piece = this.agLight.find(n => this.monster.armorInfo.piece == n.value)
                this.monster.armorclass = this.monster.abilities["Dexterity"].modifier + piece.ac + shield;
                break;
            case ArmorGroup.MediumArmor:
                piece = this.agMedium.find(n => this.monster.armorInfo.piece == n.value)
                this.monster.armorclass = Math.min(2, this.monster.abilities["Dexterity"].modifier) + piece.ac + shield;
                break;
            case ArmorGroup.HeavyArmor:
                piece = this.agHeavy.find(n => this.monster.armorInfo.piece == n.value)
                this.monster.armorclass = piece.ac + shield;
                break;
        }
    }
    public acGroupChange() {
        this.monster.armorInfo.piece = undefined;
        this.monster.armor = "";
        this.recalcAc();
    }
}
