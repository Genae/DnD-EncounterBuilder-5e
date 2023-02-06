import {Component, Input, OnInit} from '@angular/core';
import {ArmorGroup, Condition, DamageType, Monster} from "../../../../models/monster";
import {Armors, ArmorsList} from "../../../../models/lists/armorsList";
import {ArmorGroups, ArmorGroupsList} from "../../../../models/lists/armorGroupsList";
import {HitDieSize, HitDieSizeList} from "../../../../models/lists/hitDieSizeList";
import {StatsByCr, StatsByCrList} from "../../../../models/lists/statsByCrList";

@Component({
  selector: 'app-monster-defense',
  templateUrl: './monster-defense.component.html',
  styleUrls: ['./monster-defense.component.css']
})
export class MonsterDefenseComponent implements OnInit {

  //Inputs
  @Input() monster: Monster
  @Input() formGroups: { [id: string]: any; }
  
  //Static
  public dmgTypeValues = Object.values(DamageType);
  public conditionValues = Object.values(Condition);
  armors: Armors[] = ArmorsList.list;
  armorGroups: ArmorGroups[] = ArmorGroupsList.list;
  hitDieSize: HitDieSize[] = HitDieSizeList.list;
  statsByCr: StatsByCr[] = StatsByCrList.list;
  
  constructor() { }

  ngOnInit(): void {
    this.recalcHP();
  }


  public abilityChange(ability: string) {
    this.monster.abilities[ability].modifier = parseInt((this.monster.abilities[ability].value / 2) + "") - 5
    if (ability === "Dexterity")
      this.recalcAc();
    if (ability === "Constitution")
      this.recalcHP()
  }


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
    var hd = this.hitDieSize.find(hd => hd.size == this.monster.size);
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
    let hpLine = this.statsByCr.find(l => l.hp[0] <= hp && l.hp[1] >= hp);
    if (hpLine === undefined) return;
    let hpCR = hpLine.cr;
    let acCR = parseInt("" + ((ac - hpLine.ac) / 2))
    let defCR = hpCR + acCR;
  }


}
