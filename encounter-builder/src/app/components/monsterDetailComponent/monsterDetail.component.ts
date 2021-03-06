﻿import { Component, Input } from '@angular/core';

import { DataService } from "../../core/services/data.service";
import { Monster, PreparedSpell } from "../../core/models/monster";
import { Spell } from "../../core/models/spell";
import { Router, ActivatedRoute, ParamMap } from '@angular/router';
import { switchMap } from 'rxjs/operators';

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

    data: any;


    public monsterUpdated(monster: Monster) {
        this.data = {};
        this.data.monsterSpells = [];
        this.data.myMonster = [];
        this.data.myMonster.push(monster);
        if (monster.spellcasting !== undefined && monster.spellcasting.spells.length > 0) {
            var flattened = [].concat.apply([], monster.spellcasting.spells).filter((a: PreparedSpell) => a !== null);
            this.dataService.getSpellsFromIds(flattened.map((s: PreparedSpell) => s.spellId)).subscribe((data) => {
                this.data.monsterSpells = data;
            });
        }
    }
}