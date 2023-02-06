import {Component, Input, OnInit} from '@angular/core';

import {
    Monster,
    PreparedSpell,
    Size,
    MonsterType,
    Ability,
    ChallengeRating,
    ArmorGroup,
    ArmorPiece,
    DamageType,
    ArmorInfo,
    Condition,
    Morality,
    Order,
    Multiattack,
    Action,
    Attack,
    AttackType,
    Senses,
    HitEffect,
    SavingThrow,
    Trait
} from "../../../models/monster";
import { Spell } from "../../../models/spell";
import { Router, ActivatedRoute, ParamMap } from '@angular/router';
import { TextgenService } from '../../../services/textgen.service';
import { WeaponCategory, WeaponType } from '../../../models/weapon';
import { MonsterService } from '../../../services/monster.service';
import { SpellService } from '../../../services/spell.service';
import { WeaponTypeService } from '../../../services/weaponType.service';
import { Project } from '../../../models/project';
import {FormControl, FormGroup} from "@angular/forms";
import {StatsByCr, StatsByCrList} from "../../../models/lists/statsByCrList";
import {HitDieSize, HitDieSizeList} from "../../../models/lists/hitDieSizeList";

@Component({
    selector: 'monsterEdit',
    templateUrl: 'monsterEdit.component.html'
})

export class MonsterEditComponent implements OnInit {

    constructor(private monsterService: MonsterService, private spellService: SpellService, private weaponTypeService: WeaponTypeService, private textGen: TextgenService, private route: ActivatedRoute, private router: Router) {
        this.monsterService.getTraits().subscribe(res => {
            this.traits = res;
            this.traitGroups = Object.keys(this.traits);
            this.monsterService.getTags().subscribe(res => {
                this.tags = res
                this.weaponTypeService.getWeapons().subscribe(wep => {
                    this.weapons = wep;
                    this.weapongroups = Object.values(WeaponCategory);
                    this.route.params.subscribe(params => {
                        if (params['id'])
                            this.monsterService.getMonsterById(params['id']).subscribe(response => this.monsterUpdated(response as Monster));
                    });
                });
            });
        });        
    }

    public formGroups: { [id: string]: { [id: string]: FormControl; }; } = {
        'basic': {},
        'abilities': {},
        'defence': {}
    }

    ngOnInit(): void {
        this.formGroups['basic']['proficiency'] = new FormControl(this.proficency);
    }

    public view() {
        if (this.monster === undefined)
            return;
        this.router.navigateByUrl('/monsterDetail/' + this.monster.id);
    }

    monster: Monster;
    hover: boolean;
    save: { [id: string]: boolean; } = {}
    proficency: number;
    alignment: { [id: string]: boolean; } = {};
    alignmentList: string[] = [];
    hasMultiattack: boolean;
    weapons: WeaponType[];
    weapongroups: WeaponCategory[];

    monsterSpells: Spell[];
    tags: { [id: string]: string; }
    traits:  { [id: string]: Trait[]; }

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

    public updateProjectTags(projects: Project[]) {
        this.monster.projectTags = projects.map(p => p.id);
    }

    public getObjectKeys(dic: { [id: string]: number }) {
        if(dic)
            return Object.keys(dic);
        return [];
    }

    public removeActionFromMulti(action: string) {
        delete this.monster.multiattackAction!.actions[action];
        this.updateMulti()
    }

    addActionToMultiSelection: string;

    public addActionToMulti() {
        if (!this.monster.multiattackAction!.actions)
            this.monster.multiattackAction!.actions = {}
        this.monster.multiattackAction!.actions[this.addActionToMultiSelection] = 1;
        this.addActionToMultiSelection = "";
        this.updateMulti()
    }

    public updateMulti() {
        this.textGen.generateMultiattackText(this.monster).subscribe(res => {
            this.monster.multiattackAction = res;
        })
    }

    public getUnusedMultiActions() {
        var used = this.getObjectKeys(this.monster.multiattackAction!.actions);
        var actions = this.monster.actions.map(a => a.name);
        return actions.filter(a => !used.includes(a));
    }

    public hasMultiattackChange() {
        if (this.hasMultiattack) {
            this.monster.multiattackAction = new Multiattack()
            this.monster.multiattackAction.name = "Multiattack"
            this.monster.multiattackAction.text = ""
        }
        else {
            delete this.monster.multiattackAction;
        }
    }

    addSelectedActionToMonster: WeaponType;
    public addActionToMonster() {
        if (this.addSelectedActionToMonster == undefined)
            return;
        let action: Action = {
            hasAttack: true,
            name: this.addSelectedActionToMonster.name,
            attack: this.addSelectedActionToMonster.attack,
            hitEffects: [this.addSelectedActionToMonster.hitEffect],
            text: "None"
        };
        this.monster.actions.push(action);
        this.updateAction(action);
        this.addSelectedActionToMonster = new WeaponType();
    }

    public removeAction(a: Action) {
        this.monster.actions.splice(this.monster.actions.indexOf(a), 1)
    }

    public getUnusedWeapons(cat: WeaponCategory): WeaponType[] {
        let used = this.monster.actions?.map(a => a.name) ?? [];
        return this.weapons.filter(w => !used.includes(w.name) && w.weaponCategory === cat);
    }


    addSelectedTraitToMonster: Trait;
    traitGroups: string[];
    removeTrait(trait: Trait) {
        this.monster.traits.splice(this.monster.traits.indexOf(trait), 1)
    }

    addTraitToMonster() {
        if (this.addSelectedTraitToMonster == undefined)
            return;
        this.monster.traits.push(this.addSelectedTraitToMonster);
        this.addSelectedTraitToMonster = new Trait();
    }

    getUnusedTrait(group: string) : Trait[] {
        return this.traits[group];
    }

    public submit() {
        this.monsterService.saveMonster(this.monster).subscribe(m => {
            this.monster = m;
            this.view();
        });
    }

    public monsterUpdated(monster: Monster) {
        this.monster = monster;
        this.monsterSpells = [];
        if (!this.monster.armorInfo)
            this.monster.armorInfo = new ArmorInfo();

        //fix dropdown values
        let fixCr = this.crValues.find(v => v.description === this.monster.challengeRating.description);
        if (fixCr !== undefined) {
            this.monster.challengeRating = fixCr;
        }

        if (this.monster.spellcasting !== undefined && this.monster.spellcasting.spells.length > 0) {
            var flattened = this.monster.spellcasting.spells.flat().filter((a: PreparedSpell) => a !== null);
            this.spellService.getSpellsFromIds(flattened.map((s: PreparedSpell) => s.spellId)).subscribe((data) => {
                this.monsterSpells = data;
            });
        }

        //fix senses
        if (!this.monster.senses) {
            this.monster.senses = new Senses();
            this.monster.senses.description = "";
        }

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

        //attack
        if (this.monster.actions)
            for (let act of this.monster.actions) {
                act.hasAttack = act.attack !== undefined;
            }
        else
            this.monster.actions = [];
        if (this.monster.legendaryActions)
            for (let act of monster.legendaryActions) {
                act.action.hasAttack = act.action.attack !== undefined;
            }
        if (this.monster.reactions)
            for (let act of this.monster.reactions) {
                act.action.hasAttack = act.action.attack !== undefined;
            }

        this.route.queryParams.subscribe((params) => {
            if (params.pId && this.monster.projectTags.indexOf(params.pId) === -1) {
                this.monster.projectTags.push(params.pId);
            }
        });

    }
    public abilityValues = Object.values(Ability);

    public updateAction(act: Action) {
        if (act.hasAttack) {
            if(act.attack === undefined)
                act.attack = new Attack();
        }
        else
            act.attack = undefined;

        this.textGen.generateActionText(act).subscribe(res => {
            act.text = res;
        })
    
    }

    public isAtkRanged(act: Action) {
        let atk = act.attack;
        if (!atk) return false;
        return atk.type === AttackType.Melee_or_Ranged_Spell_Attack || atk.type === AttackType.Melee_or_Ranged_Weapon_Attack || atk.type === AttackType.Ranged_Spell_Attack || atk.type === AttackType.Ranged_Weapon_Attack;
    }

    public isAtkMelee(act: Action) {
        let atk = act.attack;
        if (!atk) return false;
        return atk.type === AttackType.Melee_or_Ranged_Spell_Attack || atk.type === AttackType.Melee_or_Ranged_Weapon_Attack || atk.type === AttackType.Melee_Spell_Attack || atk.type === AttackType.Melee_Weapon_Attack;
    }

    public getSavingThrow(he: HitEffect): SavingThrow {
        return he.dc as SavingThrow;
    }
    
    statByCr: StatsByCr[] = StatsByCrList.list;
    public setCr() {
        var cr = this.crFromValue(this.monster.challengeRating.value);
        var line = this.statByCr.find(l => l.cr === cr);
        if(line && this.proficency != line.prof)
        {
            this.proficency = line.prof
            this.formGroups['basic']['proficiency'].setValue(this.proficency)
        }
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
    public atkTypeValues = Object.values(AttackType);

    public vulDesc(vul: DamageType[] | string[]): string {
        if (vul === undefined)
            return "";
        return vul.map(v => {v + ""}).join(", ")
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
    hitDieSize: HitDieSize[] = HitDieSizeList.list;

    sizeChanged(size: number) {
        this.formGroups['basic']['size'].setValue(size)
    }
}
