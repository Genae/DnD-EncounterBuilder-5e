import {Component, ElementRef, HostListener, Input, OnInit, ViewChild} from '@angular/core';
import {FormControl} from "@angular/forms";
import {Action, DieRoll, Monster, MovementType} from "../../../../models/monster";
import {StatsByCr, StatsByCrList} from "../../../../models/lists/statsByCrList";

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
    let hpLine = this.statsByCr.find(l => l.hp[0] <= hp && l.hp[1] >= hp);
    if (hpLine === undefined) return;
    let hpCR = hpLine.cr;
    let acCR = parseInt("" + ((ac - hpLine.ac) / 2))
    let flyingBonus = 0;
    if(this._monster.speed.speeds[MovementType.Fly+""] > 0 && this._monster.actions.find(a => a.attack?.shortRange??0 > 0))
      flyingBonus = 2;
    this.effectiveAC = ac + flyingBonus;
    this.defensiveCR = hpCR + acCR;
    this.calcCR();
  }
  
  public calcOffCR(){
    //TODO: Multiattack, Spells, Feats, etc..
    let dmgs: number[] = [];
    for(let i = 0; i < this._monster.actions.length; i++) {
      dmgs.push(this.calcDamage(this._monster.actions[i]));
    }
    this.dmgPerRound = Math.max(...dmgs);
    this.effectiveDMG = this.dmgPerRound;
    //TODO: Traits
    this.offensiveTraits = 0;
    //TODO: Attack DC Multiplier
    this.attackDCMultiplier = 1;
    let dmgLine = this.statsByCr.find(l => l.dmg[0] <= this.dmgPerRound && l.dmg[1] >= this.dmgPerRound);
    this.offensiveCR = dmgLine!.cr;
    this.calcCR();    
  }

  calcDamage(action: Action): number {
    if(action.hitEffects) {
      let dmgSum = 0;
      for(let i = 0; i < action.hitEffects.length; i++) {
        dmgSum+= DieRoll.getExpectedRoll(action.hitEffects[i].damageDie);
      }
      return dmgSum;
    }
    return 0;
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
