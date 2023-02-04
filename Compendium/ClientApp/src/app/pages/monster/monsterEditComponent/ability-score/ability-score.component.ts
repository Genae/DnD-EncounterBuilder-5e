import {Component, Input, OnInit} from '@angular/core';
import {Ability, AbilityScore, Monster} from "../../../../models/monster";
import {FormControl, FormGroup} from "@angular/forms";
import {MatSliderChange} from "@angular/material/slider";
import {SkillList} from "../../../../models/lists/skillList";

@Component({
  selector: 'app-ability-score',
  templateUrl: './ability-score.component.html',
  styleUrls: ['./ability-score.component.css']
})
export class AbilityScoreComponent implements OnInit {
  
  //Inputs
  @Input() abilities: { [id: string]: AbilityScore; }
  @Input() savingThrows: { [id: string]: number; }
  @Input() skillModifiers: { [id: string]: number; }
  _proficiency: number;
  @Input() set proficiency(p: number){
    if(this._proficiency) {
      this._proficiency = p;
      for(let ability of this.abilityValues) {
        this.updateSave(ability)
      }
    }
    else
      this._proficiency = p;
  }

  //static values
  public abilityValues = Object.values(Ability);
  
  //form
  public abilitiesFormGroup: FormGroup;
  private _group: any = {};
  
  //local
  saveMultipliers: { [id: string]: number; } = {};
  syncedSave: { [id: string]: boolean; } = {};
  
  constructor() {     
  } 

  ngOnInit(): void {
    let group = this._group;
    for(let ability of this.abilityValues) {
      group[ability] = new FormControl(this.abilities[ability].value)
      group[ability + 'slider'] = new FormControl(this.abilities[ability].value)
      this.calculateSaveMultiplier(ability);
      group[ability + 'saveMultiplier'] = new FormControl(this.saveMultipliers[ability])
      group[ability + 'save'] = new FormControl(this.savingThrows[ability])
      
      group[ability].valueChanges.subscribe((ev: number) => {
        if(group[ability + 'slider'].value != ev)
          group[ability + 'slider'].setValue(ev)
        this.updateModifier(ability, ev)
      })
      group[ability + 'save'].valueChanges.subscribe((ev: number) => {
        this.savingThrows[ability] = ev;
        this.calculateSaveMultiplier(ability);
        if(group[ability + 'saveMultiplier'].value != this.saveMultipliers[ability])
          group[ability + 'saveMultiplier'].setValue(this.saveMultipliers[ability])
      })
    }
    this.abilitiesFormGroup = new FormGroup(group);
  }
  
  updateModifier(ability: string, value: number){
    this.abilities[ability].value = value
    this.abilities[ability].modifier = parseInt((this.abilities[ability].value / 2) + "") - 5
    this.updateSave(ability);
  }

  updateSave(ability: string) {
    if(this.saveMultipliers[ability] === 0){
      delete this.savingThrows[ability];
    }
    else {
      this.savingThrows[ability] = this.abilities[ability].modifier + this._proficiency * this.saveMultipliers[ability];
      if(this._group[ability + 'save'].value != this.savingThrows[ability])
        this._group[ability + 'save'].setValue(this.savingThrows[ability])
    }
  }

  slide($event: MatSliderChange, ability:string) {
    let form = this.abilitiesFormGroup.get(ability);
    if(form?.value != $event.value)
      form?.setValue($event.value)
  };
  
  slideSave($event: MatSliderChange, ability:string) {
    let multiplier = 0;
    if($event.value) multiplier = $event.value;
    this.saveMultipliers[ability] = multiplier;
    this.updateSave(ability)
  };

  public getSkills() {
    return Object.keys(this.skillModifiers)
  }

  public addSkillSelection: any;

  public addSkill() {
    this.skillModifiers[this.addSkillSelection] = this.getSkillDefaultValue(this.addSkillSelection);
    this.addSkillSelection = "";
  }

  public removeSkill(skill:string) {
    delete this.skillModifiers[skill];
  }

  public getSkillValues() {
    var active = Object.keys(this.skillModifiers);
    return Object.keys(SkillList.list).filter(s => active.indexOf(s) === -1)
  }

  public getSkillDefaultValue(skill:string) {
    let ability = SkillList.list[skill];
    return this.abilities[ability].modifier + this._proficiency
  }

  private calculateSaveMultiplier(ability: string) {
    let calc = (this.savingThrows[ability] - this.abilities[ability].modifier) / this._proficiency
    this.saveMultipliers[ability] = parseInt(calc + "");
    this.syncedSave[ability] = this.syncedSave[ability] && calc == this.saveMultipliers[ability];
  }
}
