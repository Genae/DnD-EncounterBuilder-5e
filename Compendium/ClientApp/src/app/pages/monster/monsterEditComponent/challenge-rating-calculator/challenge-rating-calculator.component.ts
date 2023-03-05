import {Component, ElementRef, HostListener, Input, OnInit, ViewChild} from '@angular/core';
import {FormControl} from "@angular/forms";
import {Ability, Action, DieRoll, LimitedUsage, Monster, MovementType, Multiattack} from "../../../../models/monster";
import {StatsByCr, StatsByCrList} from "../../../../models/lists/statsByCrList";
import {timeout} from "rxjs";

class ActionDamage {
  actionName: string;
  usesInFirst3Rounds: number;
  numberOfTargets: number;
  singleTargetDamage: number;
  damage: number;
}

@Component({
  selector: 'app-challenge-rating-calculator',
  templateUrl: './challenge-rating-calculator.component.html',
  styleUrls: ['./challenge-rating-calculator.component.css']
})
export class ChallengeRatingCalculatorComponent implements OnInit {

  //Input
  @Input() formGroups: { [id: string]: { [id: string]: FormControl; } };
  @Input() set monster(m:Monster){
    this._monster = m;
    this.calcDefCR();
    this.calcOffCR();
  }

  //Static
  public abilityValues = Object.values(Ability);
  
  
  //Local
  _monster: Monster;
  @ViewChild('crCalc') crCalc: ElementRef;
  expectedCR: any;
  calculatedCR: any;
  
  defensiveCR: any;
  baseHP: any;
  effectiveHP: any;
  acFromSaves: number;
  flyingBonus:number = 0;
  defensiveTraits: any;
  effectiveAC: any;
  
  offensiveCR: any;
  dmgPerRound: any;
  effectiveDMG: any;
  offensiveTraits: any;
  attackDCMultiplier: any;
  
  actionDamage: ActionDamage[];
  damageRounds: ActionDamage[];

  constructor() { }
  
  ngOnInit(): void {
    this.formGroups['basic']['cr'].valueChanges.subscribe(cr =>{
      this.expectedCR = cr;
    })
    
    this.formGroups['defence']['armorClass'].valueChanges.subscribe(_=>{
      setTimeout(()=>this.calcDefCR(), 10);
    })
    this.formGroups['defence']['hitPoints'].valueChanges.subscribe(_=>{
      setTimeout(()=>this.calcDefCR(), 10);
    })
    for(let ability of this.abilityValues) {
      this.formGroups['abilities'][ability + 'saveMultiplier'].valueChanges.subscribe(_=>{
        setTimeout(()=>this.calcDefCR(), 10);
      })
    }
    this.formGroups['defence']['vulnerabilities'].valueChanges.subscribe(_=>{
      setTimeout(()=>this.calcDefCR(), 10);
    });
    this.formGroups['defence']['resistances'].valueChanges.subscribe(_=>{
      setTimeout(()=>this.calcDefCR(), 10);
    });
    this.formGroups['defence']['immunities'].valueChanges.subscribe(_=>{
      setTimeout(()=>this.calcDefCR(), 10);
    });
    this.formGroups['defence']['conditionImmunities'].valueChanges.subscribe(_=>{
      setTimeout(()=>this.calcDefCR(), 10);
    });
        //register flightspeed + actions
  }


  public calcDefCR() {
    if(!this.expectedCR)
      return;
    let hp = this._monster.hitDie.expectedRoll;
    this.baseHP = hp;
    /*
      TODO effective HP: >= 3 resistances/immunities
      cr 0-4: x2 (res), x2 (imm)
      cr 5-10: x1.5, x2
      cr 11-16: x1.25, x1.5
      cr 17++: x1, x1.25
      vulnerable >= 3: x0.5 (dont do that!)
     */
    this.effectiveHP = this.baseHP * this.calcResistanceMultiplier() * this.calcVulMultiplier();
    
    //TODO traits
    this.defensiveTraits = 0;
    let ac = this._monster.armorclass;
    let hpLine = StatsByCrList.findByHP(hp);
    if (hpLine === undefined) return;
    let hpCR = hpLine.cr;
    if(this.expectedCR < 10 && this._monster.speed.speeds[MovementType.Fly+""] > 0 && this._monster.actions.find(a => a.attack?.shortRange??0 > 0))
      this.flyingBonus = 2;
    this.acFromSaves = this.calcCrFromSaves();
    this.effectiveAC = ac + this.flyingBonus + this.acFromSaves;
    let crLine = StatsByCrList.findByCR(this.expectedCR);
    let acCR = parseInt("" + ((this.effectiveAC - crLine!.ac) / 2));
    
    this.defensiveCR = hpCR + acCR;
    this.calcCR();
  }

  calcResistanceMultiplier():number {
    let numImmune = this._monster.immune?.length??0;
    let numRes = this._monster.resist?.length??0;
    if(this.expectedCR <=4 ){
      if(numImmune + numRes >= 3) return 2;
    }
    else if(this.expectedCR <=10 ){
      if(numImmune >= 3) return 2;
      if(numImmune + numRes >= 3) return 1.5;
    }
    else if(this.expectedCR <=16 ){
      if(numImmune >= 3) return 1.5;
      if(numImmune + numRes >= 3) return 1.25;
    }
    else{
      if(numImmune >= 3) return 1.25;
    }
    return 1;
  };
  calcVulMultiplier():number {
    let numVul = this._monster.vulnerable?.length??0;    
      if(numVul >= 3) return 0.5;
    return 1;
  };
  calcCrFromSaves() {
    let saves = 0;
    for(let ability of this.abilityValues) {
      let mult = this.formGroups['abilities'][ability + 'saveMultiplier'].value;
      if(mult > 0)
        saves += mult;
    }
    if(saves>=5)
      return 4;
    if(saves>=3)
      return 2;
    return 0;
  }
  public calcOffCR(){
    //TODO: Spells, Feats, etc..
    this.actionDamage = [];
    for(let i = 0; i < this._monster.actions.length; i++) {
      this.actionDamage.push(this.calcDamage(this._monster.actions[i]));
    }
    if(this._monster.multiattackAction){
      this.actionDamage.push(this.calcMultiattackDamage(this._monster.multiattackAction));
    }
    this.actionDamage.sort((a,b) => b.damage - a.damage);
    let overallDmg = 0;
    this.damageRounds = [];
    for(let r = 0; r < 3; r++){
      if(this.actionDamage[0].usesInFirst3Rounds > r) {
        this.damageRounds[r] = this.actionDamage[0];
      }
      else if(this.actionDamage[1].usesInFirst3Rounds > r-this.actionDamage[0].usesInFirst3Rounds) {
        this.damageRounds[r] = this.actionDamage[1];
      }
      else {
        this.damageRounds[r] = this.actionDamage[2];        
      }
      overallDmg += this.damageRounds[r].damage;
    }
    this.dmgPerRound = Math.round(overallDmg/3);
    this.effectiveDMG = this.dmgPerRound;
    //TODO: Traits
    this.offensiveTraits = 0;
    //TODO: Attack DC Multiplier
    this.attackDCMultiplier = 1;
    let dmgLine = StatsByCrList.findByDamage(this.dmgPerRound);
    this.offensiveCR = dmgLine!.cr;
    this.calcCR();    
  }

  private calcMultiattackDamage(multiAtk: Multiattack): ActionDamage {
    let dmg = 0;
    for (let a in multiAtk.actions) {
      let acDmg = this.actionDamage.find(ad => ad.actionName == a);
      dmg += (acDmg?.damage ?? 0) * multiAtk.actions[a];
    }
    return  {
      actionName: multiAtk.name,
      damage: dmg,
      numberOfTargets: 1,
      singleTargetDamage: dmg,
      usesInFirst3Rounds: 3
    };
  }

  calcDamage(action: Action): ActionDamage {
    let actionDamage = new ActionDamage();
    actionDamage.actionName = action.name;
    actionDamage.usesInFirst3Rounds = 3;
    if(action.limitedUsage) {
      switch (action.limitedUsage){
        case LimitedUsage.Recharge6:
        case LimitedUsage.Recharge5:
        case LimitedUsage.RechargeShort:
        case LimitedUsage.RechargeLong:
        case LimitedUsage.OnePerDay:
          actionDamage.usesInFirst3Rounds = 1;
          break;
        case LimitedUsage.TwoPerDay:
          actionDamage.usesInFirst3Rounds = 2;
          break;
        case LimitedUsage.ThreePerDay:
          actionDamage.usesInFirst3Rounds = 3;
          break;
      }
    }
    actionDamage.numberOfTargets = 1;
    if(action.text.toLowerCase().indexOf("one target") === -1 && action.text.toLowerCase().indexOf("one creature") === -1 ) {
      actionDamage.numberOfTargets = 2;
    }
    if(action.hitEffects) {
      let dmgSum = 0;
      for(let i = 0; i < action.hitEffects.length; i++) {
        if(action.hitEffects[i].damageDie)
          dmgSum+= DieRoll.getExpectedRoll(action.hitEffects[i].damageDie);
      }
      actionDamage.singleTargetDamage = dmgSum;
      actionDamage.damage = actionDamage.singleTargetDamage * actionDamage.numberOfTargets;
    }
    return actionDamage;
  }
  
  
  public calcCR() {
    if(this.defensiveCR && this.offensiveCR) {
      let avg = (this.defensiveCR + this.offensiveCR)/2;
      if(avg > 1){
        this.calculatedCR = Math.round(avg);
      }
      else if(avg < 1/16) {
        this.calculatedCR = 0;        
      }
      else if(avg < 3/16) {
        this.calculatedCR = 1/8;
      }
      else if(avg < 3/8) {
        this.calculatedCR = 1/4;
      }
      else if(avg < 3/4) {
        this.calculatedCR = 1/2;
      }
      else {
        this.calculatedCR = 1;
      }
    }
  }
  
  public crToString(cr: number) {
    if(cr === 1/8)
      return "1/8"
    if(cr === 1/4)
      return "1/4"
    if(cr === 1/2)
      return "1/2"
    return cr + "";
  }

}
