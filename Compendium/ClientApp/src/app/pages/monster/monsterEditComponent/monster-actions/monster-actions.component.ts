import {Component, Input, OnInit} from '@angular/core';
import {WeaponCategory, WeaponType} from "../../../../models/weapon";
import {
  Ability,
  Action,
  Attack,
  AttackType,
  DamageType,
  HitEffect, Monster,
  Multiattack,
  SavingThrow
} from "../../../../models/monster";
import {WeaponTypeService} from "../../../../services/weaponType.service";
import {HitDieSize, HitDieSizeList} from "../../../../models/lists/hitDieSizeList";
import {FormControl} from "@angular/forms";
import {TextgenService} from "../../../../services/textgen.service";
import {zip} from "rxjs";

@Component({
  selector: 'app-monster-actions',
  templateUrl: './monster-actions.component.html',
  styleUrls: ['./monster-actions.component.css']
})
export class MonsterActionsComponent implements OnInit {

  //Inputs
  @Input() set monster(m: Monster) {
    this._monster = m;
  }
  @Input() formGroups: { [id: string]: { [id: string]: FormControl; } }
  
  //Static
  weapons: WeaponType[];
  weapongroups: WeaponCategory[];
  dmgTypeValues = Object.values(DamageType);
  atkTypeValues = Object.values(AttackType);
  hitDieSize: HitDieSize[] = HitDieSizeList.list;
  
  //Local
  _monster: Monster;
  hasMultiattack: boolean;
  addSelectedActionToMonster: WeaponType;
  addActionToMultiSelection: string;
  
  //Forms
  _loaded:boolean = false;
  
  constructor(private weaponTypeService: WeaponTypeService, private textGen: TextgenService) {
    let loadingWeapons = this.weaponTypeService.getWeapons();
    loadingWeapons.subscribe(wep => {
      this.weapons = wep;
      this.weapongroups = Object.values(WeaponCategory);
    });
    zip(loadingWeapons).subscribe(_ =>{
      this._loaded = true;
    })
  }

  ngOnInit(): void {
    this.monsterUpdated();
  }

  public getObjectKeys(dic: { [id: string]: number }) {
    if(dic)
      return Object.keys(dic);
    return [];
  }

  public abilityValues = Object.values(Ability);

  monsterUpdated() {
    //multiattack
    this.hasMultiattack = this._monster.multiattackAction !== undefined;

    //attack
    if (this._monster.actions)
      for (let act of this._monster.actions) {
        act.hasAttack = act.attack !== undefined;
      }
    else
      this._monster.actions = [];
    if (this._monster.legendaryActions)
      for (let act of this._monster.legendaryActions) {
        act.action.hasAttack = act.action.attack !== undefined;
      }
    if (this._monster.reactions)
      for (let act of this._monster.reactions) {
        act.action.hasAttack = act.action.attack !== undefined;
      }
  }


  public removeActionFromMulti(action: string) {
    delete this._monster.multiattackAction!.actions[action];
    this.updateMulti()
  }


  public addActionToMulti() {
    if (!this._monster.multiattackAction!.actions)
      this._monster.multiattackAction!.actions = {}
    this._monster.multiattackAction!.actions[this.addActionToMultiSelection] = 1;
    this.addActionToMultiSelection = "";
    this.updateMulti()
  }

  public updateMulti() {
    this.textGen.generateMultiattackText(this._monster).subscribe(res => {
      this._monster.multiattackAction = res;
    })
  }

  public getUnusedMultiActions() {
    var used = this.getObjectKeys(this._monster.multiattackAction!.actions);
    var actions = this._monster.actions.map(a => a.name);
    return actions.filter(a => !used.includes(a));
  }

  public hasMultiattackChange() {
    if (this.hasMultiattack) {
      this._monster.multiattackAction = new Multiattack()
      this._monster.multiattackAction.name = "Multiattack"
      this._monster.multiattackAction.text = ""
    }
    else {
      delete this._monster.multiattackAction;
    }
  }

  public addActionToMonster() {
    if (this.addSelectedActionToMonster == undefined)
      return;
    let action: Action = {
      hasAttack: true,
      name: this.addSelectedActionToMonster.name,
      attack: this.addSelectedActionToMonster.attack,
      hitEffects: [this.addSelectedActionToMonster.hitEffect],
      limitedUsageText: "",
      text: "None"
    };
    this._monster.actions.push(action);
    this.updateAction(action);
    this.addSelectedActionToMonster = new WeaponType();
  }

  public removeAction(a: Action) {
    this._monster.actions.splice(this._monster.actions.indexOf(a), 1)
  }

  public getUnusedWeapons(cat: WeaponCategory): WeaponType[] {
    let used = this._monster.actions?.map(a => a.name) ?? [];
    return this.weapons.filter(w => !used.includes(w.name) && w.weaponCategory === cat);
  }


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

}
