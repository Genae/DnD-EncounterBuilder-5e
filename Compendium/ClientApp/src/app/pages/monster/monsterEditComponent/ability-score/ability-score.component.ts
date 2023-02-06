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
      for(let skill in this.skillModifiers){
        this.updateSkill(skill)
      }
    }
    else
      this._proficiency = p;
  }

  //static values
  public abilityValues = Object.values(Ability);
  public skillList = SkillList.list;
  
  //form
  public abilitiesFormGroup: FormGroup;
  private _group: any = {};
  
  //local
  saveMultipliers: { [id: string]: number; } = {};
  syncedSave: { [id: string]: boolean; } = {};
  skillMultipliers: { [id: string]: number; } = {};
  syncedSkills: { [id: string]: boolean; } = {};
  
  
  constructor() {     
  } 

  ngOnInit(): void {
    let group = this._group;
    for(let ability of this.abilityValues) {
      group[ability] = new FormControl(this.abilities[ability].value)
      group[ability + 'slider'] = new FormControl(this.abilities[ability].value)
      group[ability].valueChanges.subscribe((ev: number) => {
        if(group[ability + 'slider'].value != ev)
          group[ability + 'slider'].setValue(ev)
        this.updateModifier(ability, ev)
      })

      this.calculateSaveMultiplier(ability);
      group[ability + 'saveMultiplier'] = new FormControl(this.saveMultipliers[ability])
      group[ability + 'save'] = new FormControl(this.savingThrows[ability])
      group[ability + 'save'].valueChanges.subscribe((ev: number) => {
        this.savingThrows[ability] = ev;
        this.calculateSaveMultiplier(ability);
        if(group[ability + 'saveMultiplier'].value != this.saveMultipliers[ability])
          group[ability + 'saveMultiplier'].setValue(this.saveMultipliers[ability])
      })
    }
    for(let skill in this.skillModifiers) {
      this.calculateSkillMultiplier(skill);
      this.addSkillGroup(skill);
    }
    group['skillFilter'] = new FormControl();
    group['addSkill'] = new FormControl();
    group['addSkill'].valueChanges.subscribe((val:string) => {
      if(!val) return;
      this.addSkill(val);
      group['addSkill'].setValue(undefined);
    })
    this.abilitiesFormGroup = new FormGroup(group);
  }
  
  addSkillGroup(skill: string){
    if(this._group[skill])
      return;
    this._group[skill] = new FormControl(this.skillModifiers[skill])
    this._group[skill + 'Multiplier'] = new FormControl(this.skillMultipliers[skill])
  }
  
  //Modifiers
  updateModifier(ability: string, value: number){
    this.abilities[ability].value = value
    this.abilities[ability].modifier = parseInt((this.abilities[ability].value / 2) + "") - 5
    this.updateSave(ability);

    for(let skill in this.skillModifiers){
      this.updateSkill(skill)
    }
  }
  
  slide($event: MatSliderChange, ability:string) {
    let form = this.abilitiesFormGroup.get(ability);
    if(form?.value != $event.value)
      form?.setValue($event.value)
  };

  
  //Saves
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
 
  slideSave($event: MatSliderChange, ability:string) {
    let multiplier = 0;
    if($event.value) 
      multiplier = $event.value;
    this.saveMultipliers[ability] = multiplier;
    this.updateSave(ability)
  };
  
  private calculateSaveMultiplier(ability: string) {
    let calc = (this.savingThrows[ability] - this.abilities[ability].modifier) / this._proficiency
    this.saveMultipliers[ability] = parseInt(calc + "");
    this.syncedSave[ability] = calc == this.saveMultipliers[ability];
  }

  
  //Skills
  public getSkills() {
    return Object.keys(this.skillModifiers)
  }

  public addSkill(skill: string) {
    this.skillMultipliers[skill] = 1;
    this.addSkillGroup(skill);
    this.updateSkill(skill);
  }

  public removeSkill(skill:string) {
    delete this.skillModifiers[skill];
  }

  public getSkillValues() {
    let active = Object.keys(this.skillModifiers);
    let filter = this._group['skillFilter'].value;
    return Object.keys(this.skillList).filter(s => active.indexOf(s) === -1 && (!filter || s.indexOf(filter) !== -1))
  }

  updateSkill(skill: string) {
    let ability = this.skillList[skill];
    this.skillModifiers[skill] = this.abilities[ability].modifier + this._proficiency * this.skillMultipliers[skill];
    if(this._group[skill].value != this.skillModifiers[skill])
      this._group[skill].setValue(this.skillModifiers[skill])
  }
  
  private calculateSkillMultiplier(skill: string) {
    let ability = this.skillList[skill];
    let calc = (this.skillModifiers[skill] - this.abilities[ability].modifier) / this._proficiency
    this.skillMultipliers[skill] = parseInt(calc + "");
    this.syncedSkills[skill] = calc == this.skillMultipliers[skill];
  }

  slideSkill($event: MatSliderChange, skill: string) {
    let multiplier = 0;
    if($event.value)
      multiplier = $event.value;
    this.skillMultipliers[skill] = multiplier;
    this.updateSkill(skill)
  }
}
