import {Component, Input, OnInit} from '@angular/core';
import {Monster} from "../../../../../models/monster";
import {FormControl, FormGroup} from "@angular/forms";

@Component({
  selector: 'app-spellcasting-info',
  templateUrl: './spellcasting-info.component.html',
  styleUrls: ['./spellcasting-info.component.css']
})
export class SpellcastingInfoComponent implements OnInit {
  //Inputs
  @Input() set monster(m: Monster) {
    this._monster = m;
  }
  @Input() formGroups: { [id: string]: { [id: string]: FormControl; } }

  //form
  public spellCastingFormGroup: FormGroup;
  private _group: { [id: string]: FormControl; } = {};
  
  //Local
  _monster:Monster;
  hasSpellCasting: boolean;
  spellsForLevel: string[][] = [];
  spellLabelForLevel: string[] = [];
  constructor() { }

  static initForm(group: { [id: string]: FormControl; }) {
    group['hasSpellCasting'] = new FormControl();
    return group;
  }
  ngOnInit(): void {
    this._group = this.formGroups['spellCasting']
    let group = this._group;
    group['hasSpellCasting'].valueChanges.subscribe(val => {
      this.hasSpellCasting = val;
    })
    group['hasSpellCasting'].setValue(this._monster.spellcasting !== undefined)
    
    let lvl = 0;
    for(let lvlSpells of this._monster.spellcasting.spells) {
      if(lvlSpells != null){
        this.spellsForLevel[lvl] = lvlSpells.map(s => s.spellId)
        if(lvl === 0)
          this.spellLabelForLevel[lvl] = "Cantrips (at will)"
        else
          this.spellLabelForLevel[lvl] = this.thIfy(lvl) + " level (" + this._monster.spellcasting.spellslots[lvl-1] + " slots)"
      }
      lvl++;
    }
    
    this.spellCastingFormGroup = new FormGroup(group);
  }

  private thIfy(num: number)
  {
    if (num == 1)
      return "1st";
    if (num == 2)
      return "2nd";
    if (num == 3)
      return "3rd";
    return num + "th";
  }
}
