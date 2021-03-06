import { Component, Input } from '@angular/core';

import { Monster, PreparedSpell, Size, MonsterType } from "../../models/monster";
import { Spell } from "../../models/spell";
import { Router, ActivatedRoute, ParamMap } from '@angular/router';
import { DataService } from "../../services/data.service";

@Component({
    selector: 'monsterDetail',
    templateUrl: 'monsterDetail.component.html'
})

export class MonsterDetailComponent {

    constructor(private dataService: DataService, private route: ActivatedRoute) {

        this.route.params.subscribe(params => {
            this.dataService.getMonsterById(params['id']).subscribe(response => this.monsterUpdated(response as Monster));
        });
        
    }

    myMonster: Monster;
  monsterSpells: Spell[];
  edit: boolean = false;

    public monsterUpdated(monster: Monster) {
        this.monsterSpells = [];
        this.myMonster = monster;
        if (monster.spellcasting !== undefined && monster.spellcasting.spells.length > 0) {
            var flattened = [].concat.apply([], monster.spellcasting.spells).filter((a: PreparedSpell) => a !== null);
            this.dataService.getSpellsFromIds(flattened.map((s: PreparedSpell) => s.spellId)).subscribe((data) => {
                this.monsterSpells = data;
            });
        }
  }

  public enumValues(enumType): any[] {
    switch (enumType) {
    case "Size": 
        return Object.keys(Size).filter(key => !isNaN(Number(Size[key])));
    case "MonsterType":
        return Object.keys(MonsterType).filter(key => !isNaN(Number(MonsterType[key])));
    }
  }
}
