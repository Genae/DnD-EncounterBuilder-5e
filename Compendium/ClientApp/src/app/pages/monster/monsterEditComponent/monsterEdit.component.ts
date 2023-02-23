import {Component, OnInit} from '@angular/core';

import {
    Monster,
    PreparedSpell,
    Size,
    MonsterType,
    Ability,
    ChallengeRating,
    DamageType,
    ArmorInfo,
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
import { Router, ActivatedRoute } from '@angular/router';
import { TextgenService } from '../../../services/textgen.service';
import { WeaponCategory, WeaponType } from '../../../models/weapon';
import { MonsterService } from '../../../services/monster.service';
import { SpellService } from '../../../services/spell.service';
import { WeaponTypeService } from '../../../services/weaponType.service';
import { Project } from '../../../models/project';
import {FormControl} from "@angular/forms";
import {StatsByCr, StatsByCrList} from "../../../models/lists/statsByCrList";
import {HitDieSize, HitDieSizeList} from "../../../models/lists/hitDieSizeList";
import {CRList} from "../../../models/lists/CRList";

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
        this.formGroups['basic']['cr'] = new FormControl();
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
            this.formGroups['basic']['cr'].setValue(cr)
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
    public monsterTypeValues = Object.values(MonsterType);
    public atkTypeValues = Object.values(AttackType);



    public crValues: ChallengeRating[] = CRList.list;
    hitDieSize: HitDieSize[] = HitDieSizeList.list;

    sizeChanged(size: number) {
        this.formGroups['basic']['size'].setValue(size)
    }
}
