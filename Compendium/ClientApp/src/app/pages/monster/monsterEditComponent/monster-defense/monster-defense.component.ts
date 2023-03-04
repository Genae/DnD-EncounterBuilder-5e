import {Component, Input, OnInit} from '@angular/core';
import {ArmorGroup, ArmorPiece, Condition, DamageType, DieRoll, Monster} from "../../../../models/monster";
import {Armors, ArmorsList} from "../../../../models/lists/armorsList";
import {ArmorGroups, ArmorGroupsList} from "../../../../models/lists/armorGroupsList";
import {HitDieSize, HitDieSizeList} from "../../../../models/lists/hitDieSizeList";
import {StatsByCr, StatsByCrList} from "../../../../models/lists/statsByCrList";
import {FormControl, FormGroup} from "@angular/forms";
import {MatSliderChange} from "@angular/material/slider";

@Component({
  selector: 'app-monster-defense',
  templateUrl: './monster-defense.component.html',
  styleUrls: ['./monster-defense.component.css']
})
export class MonsterDefenseComponent implements OnInit {

  //Inputs
  @Input() set monster(m: Monster) {
    this._monster = m;
    this._dexMod = m.abilities['Dexterity'].modifier;
    this._conMod = m.abilities['Constitution'].modifier;
  }
  @Input() formGroups: { [id: string]: { [id: string]: FormControl; } }
  
  //Static
  public dmgTypeValues = Object.values(DamageType);
  public conditionValues = Object.values(Condition);
  armors: Armors[] = ArmorsList.list;
  armorGroups: ArmorGroups[] = ArmorGroupsList.list;
  hitDieSize: HitDieSize[] = HitDieSizeList.list;
  statsByCr: StatsByCr[] = StatsByCrList.list;
  
  //form
  public defenseFormGroup: FormGroup;
  private _group: { [id: string]: FormControl; } = {};
  
  //local
  _cr: number;
  _conMod: number;
  _dexMod: number;
  _monster: Monster;
  _hitDieMultiplier: number = 1;
  
  constructor() {   }


  static initForm(group: { [id: string]: FormControl; }) {
    group['armorPiece'] = new FormControl();
    group['hasShield'] = new FormControl();
    group['armorClass'] = new FormControl({value: 0, disabled: true});
    group['armor'] = new FormControl();
    group['hitDieCount'] = new FormControl();
    group['hitDieMultiplier'] = new FormControl();
    group['hitDie'] = new FormControl({value: 0, disabled: true});
    group['hitPoints'] = new FormControl({value: 0, disabled: true});
    group['hitPointsDescription'] = new FormControl({value: 0, disabled: true});
    group['vulnerabilities'] = new FormControl();
    group['resistances'] = new FormControl();
    group['immunities'] = new FormControl();
    group['conditionImmunities'] = new FormControl();
    return group;
  }

  ngOnInit(): void {
    this._group = this.formGroups['defence']
    let group = this._group;
    this.setCr(this.formGroups['basic']['cr'].value);

    group['armorPiece'].setValue(this._monster.armorInfo.piece);
    group['armorPiece'].valueChanges.subscribe(piece => {
      this._monster.armorInfo.piece = piece;
      this.recalcAc()
    })
    group['hasShield'].setValue(this._monster.armorInfo.hasShield);
    group['hasShield'].valueChanges.subscribe(val => {
      this._monster.armorInfo.hasShield = val;
      this.recalcAc()
    });
    group['armorClass'].setValue(this._monster.armorclass);
    group['armorClass'].valueChanges.subscribe(ac => {
      this._monster.armorclass = ac;
    })
    group['armor'].setValue(this._monster.armor);
    group['armor'].valueChanges.subscribe(armor => {
      this._monster.armor = armor;
    })
    group['hitDieCount'].setValue(this._monster.hitDie.dieCount);
    group['hitDieCount'].valueChanges.subscribe(hitDieCount => {
      this._monster.hitDie.dieCount = hitDieCount;
      this.calculateHitDieMultiplier();
      if(group['hitDieMultiplier'].value != this._hitDieMultiplier)
        group['hitDieMultiplier'].setValue(this._hitDieMultiplier);
    })
    group['hitDieMultiplier'].setValue(this._hitDieMultiplier);

    group['hitDie'].setValue(this._monster.hitDie.die);
    group['hitDie'].valueChanges.subscribe(hitDie => {
      this._monster.hitDie.die = hitDie;
      this.recalcHP();
      this.calculateHitDieMultiplier();
    })

    group['hitPoints'].setValue(this._monster.maximumHitpoints);
    group['hitPointsDescription'].setValue(this._monster.maximumHitpoints);
    
    this.formGroups['basic']['cr'].valueChanges.subscribe((cr: number) => {
      this.setCr(cr);
    })
    this.formGroups['basic']['size'].valueChanges.subscribe(size=> {
      this.sizeChanged();
    })
    this.formGroups['abilities']['Dexterity'].valueChanges.subscribe((prof: number) => {
      this._dexMod = parseInt((prof / 2) + "") - 5
      this.recalcAc();
    })
    this.formGroups['abilities']['Constitution'].valueChanges.subscribe((prof: number) => {
      this._conMod = parseInt((prof / 2) + "") - 5
      this.recalcHP();
      this.calculateHitDieMultiplier();
    })
    this.calculateHitDieMultiplier();
    this.recalcHP();
    group['vulnerabilities'].setValue(this._monster.vulnerable);
    group['vulnerabilities'].valueChanges.subscribe((value) => {this._monster.vulnerable = value})
    group['resistances'].setValue(this._monster.resist);
    group['resistances'].valueChanges.subscribe((value) => {this._monster.resist = value})
    group['immunities'].setValue(this._monster.immune);
    group['immunities'].valueChanges.subscribe((value) => {this._monster.immune = value})
    group['conditionImmunities'].setValue(this._monster.conditionImmune);
    group['conditionImmunities'].valueChanges.subscribe((value) => {this._monster.conditionImmune = value})
    this.defenseFormGroup = new FormGroup(group);
  }

  public getArmorPieces(group: ArmorGroup) {
    return this.armors.filter(a => a.group === group);
  }

  public recalcAc() {
    let shield = 0;
    let shieldText = '';
    if (this._monster.armorInfo.hasShield) {
      shield = 2;
      shieldText = ' (shield)'
    }
    if (this._monster.armorInfo.piece) {
      let piece = this.armors.find(n => this._monster.armorInfo.piece == n.value)
      if (!piece)
        return;
      this._monster.armorInfo.group = piece.group
      this._group['armor'].setValue(piece.name.split("(")[0].trim() + shieldText);
      switch (this._monster.armorInfo.group) {
        case ArmorGroup.NaturalArmor:
          this._group['armor'].setValue((piece.ac > 0 ? "natural armor" : "")+ shieldText );
          this._group['armorClass'].setValue(10 + this._dexMod + piece.ac + shield);
          break;
        case ArmorGroup.LightArmor:
          this._group['armorClass'].setValue(this._dexMod + piece.ac + shield);
          break;
        case ArmorGroup.MediumArmor:
          this._group['armorClass'].setValue(Math.min(2, this._dexMod) + piece.ac + shield);
          break;
        case ArmorGroup.HeavyArmor:
          this._group['armorClass'].setValue(piece.ac + shield);
          break;
      }
    }
  }
  public acGroupChange() {
    this._monster.armorInfo.piece = undefined;
    this._monster.armor = "";
    this.recalcAc();
  }

  public sizeChanged() {
    var hd = this.hitDieSize.find(hd => hd.size == this._monster.size);
    if (hd === undefined)
      return;
    this._monster.hitDie.die = hd.die;
    this.recalcHP();
  }

  public recalcHP() {
    let hd = this._monster.hitDie;
    hd.offset = hd.dieCount * this._conMod;
    hd.expectedRoll = DieRoll.getExpectedRoll(hd);
    hd.description = "(" + hd.dieCount + "d" + hd.die + " + " + hd.offset + ")"
    this._group['hitPointsDescription'].setValue(hd.description);
    this._monster.maximumHitpoints = hd.expectedRoll;
    this._group['hitPoints'].setValue(this._monster.maximumHitpoints);
  }


  private setCr(cr: number) {
    if(this._cr) {
      this._cr = cr;
      this.updateHitDie();
    }
    else
      this._cr = cr;
  }
  
  private getExpectedHitPoints() {
    let line = this.statsByCr.find(l => l.cr === this._cr)!;
    return (line.hp[0] + line.hp[1])/2
  }
  
  private getExpectedDieCount() {
    let hd = this._monster.hitDie;
    let expectedDieCount = this.getExpectedHitPoints()/(DieRoll.getExpectedDieRoll(hd.die) + this._conMod);
    return Math.round(expectedDieCount);
  }
  
  slide($event: MatSliderChange) {
    this._hitDieMultiplier = $event.value!;
    this.updateHitDie(); 
  }
  
  private updateHitDie() {
    let form = this._group['hitDieCount']
    let dieCount = Math.round(this.getExpectedDieCount() * this._hitDieMultiplier);
    if(form.value != dieCount)
      form.setValue(dieCount);
    this.recalcHP();
  }

  private calculateHitDieMultiplier() {
    let hd = this._monster.hitDie;
    let expectedDieCount = this.getExpectedDieCount();
    if(hd.dieCount == expectedDieCount)
      return;
    if(expectedDieCount >= 1)
      this._hitDieMultiplier = hd.dieCount/expectedDieCount;
    else 
      this._hitDieMultiplier = 0;
    let form = this._group['hitDieMultiplier']
    if(form.value != this._hitDieMultiplier)
      form.setValue(this._hitDieMultiplier);
  }

}
