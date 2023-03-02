import {Component, ElementRef, HostListener, Input, OnInit, ViewChild} from '@angular/core';
import {FormControl} from "@angular/forms";
import {Action, DieRoll, LimitedUsage, Monster, MovementType, Multiattack} from "../../../../models/monster";
import {StatsByCr, StatsByCrList} from "../../../../models/lists/statsByCrList";

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
  statsByCr: StatsByCr[] = StatsByCrList.list;
  
  
  //Local
  _monster: Monster;
  @ViewChild('crCalc') crCalc: ElementRef;
  expectedCR: any;
  calculatedCR: any;
  
  defensiveCR: any;
  baseHP: any;
  effectiveHP: any;
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
      this.calcDefCR();
    })
    this.formGroups['defence']['hitPoints'].valueChanges.subscribe(_=>{
      this.calcDefCR();
    })
    //register flightspeed + actions
  }


  public calcDefCR() {
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
    this.effectiveHP = this.baseHP;
    
    //TODO traits
    this.defensiveTraits = 0;
    let ac = this._monster.armorclass;
    let hpLine = StatsByCrList.findByHP(hp);
    if (hpLine === undefined) return;
    let hpCR = hpLine.cr;
    let flyingBonus = 0;
    if(this._monster.speed.speeds[MovementType.Fly+""] > 0 && this._monster.actions.find(a => a.attack?.shortRange??0 > 0))
      flyingBonus = 2;
    this.effectiveAC = ac + flyingBonus;
    let acCR = parseInt("" + ((this.effectiveAC - hpLine.ac) / 2))
    this.defensiveCR = hpCR + acCR;
    this.calcCR();
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

  @HostListener('window:scroll', ['$event']) // for window scroll events
  onScroll() {
    let topPos = this.crCalc.nativeElement.getBoundingClientRect().top;
    this.crCalc.nativeElement.style.top = 200+window.scrollY + "px";
  }

}
