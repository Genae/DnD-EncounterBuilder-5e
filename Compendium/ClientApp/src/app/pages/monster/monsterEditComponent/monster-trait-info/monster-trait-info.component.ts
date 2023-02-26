import {Component, Input, OnInit} from '@angular/core';
import {Monster, Trait} from "../../../../models/monster";
import {FormControl} from "@angular/forms";
import {MonsterService} from "../../../../services/monster.service";
import {zip} from "rxjs";

@Component({
  selector: 'app-monster-trait-info',
  templateUrl: './monster-trait-info.component.html',
  styleUrls: ['./monster-trait-info.component.css']
})
export class MonsterTraitInfoComponent implements OnInit {
  //Inputs
  @Input() set monster(m: Monster) {
    this._monster = m;
  }
  @Input() formGroups: { [id: string]: { [id: string]: FormControl; } }

  //Static
  traits:  { [id: string]: Trait[]; }
  
  //Local
  _monster: Monster;
  addSelectedTraitToMonster: Trait;
  traitGroups: string[];
  
  //Forms
  _loaded: boolean = false;
  
  
  constructor(private monsterService: MonsterService) {
    let loadingTraits = this.monsterService.getTraits();
    loadingTraits.subscribe(res => {
      this.traits = res;
      this.traitGroups = Object.keys(this.traits);
    });
    zip(loadingTraits).subscribe(_ =>{
      this._loaded = true;
    })
  }

  ngOnInit(): void {
  }

  removeTrait(trait: Trait) {
    this._monster.traits.splice(this._monster.traits.indexOf(trait), 1)
  }

  addTraitToMonster() {
    if (this.addSelectedTraitToMonster == undefined)
      return;
    this._monster.traits.push(this.addSelectedTraitToMonster);
    this.addSelectedTraitToMonster = new Trait();
  }

  getUnusedTrait(group: string) : Trait[] {
    return this.traits[group];
  }
  

}
