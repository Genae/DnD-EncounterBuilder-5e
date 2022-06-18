import { Component, Input } from '@angular/core';

import { Monster, PreparedSpell, Size, MonsterType, Ability } from "../../models/monster";
import { Spell } from "../../models/spell";
import { Router, ActivatedRoute, ParamMap } from '@angular/router';
import { DataService } from "../../services/data.service";

@Component({
    selector: 'monsterEdit',
    templateUrl: 'monsterEdit.component.html'
})

export class MonsterEditComponent {

    constructor(private dataService: DataService, private route: ActivatedRoute) {

        this.route.params.subscribe(params => {
            
        });
        this.route.params.subscribe(params => {
            if (params['id'])
                this.dataService.getMonsterById(params['id']).subscribe(response => this.monsterUpdated(response as Monster));
        });

    }

    monster: Monster;
    monsterSpells: Spell[];

    public monsterUpdated(monster: Monster) {
        this.monsterSpells = [];
        this.monster = monster;
        if (monster.spellcasting !== undefined && monster.spellcasting.spells.length > 0) {
            var flattened = [].concat.apply([], monster.spellcasting.spells).filter((a: PreparedSpell) => a !== null);
            this.dataService.getSpellsFromIds(flattened.map((s: PreparedSpell) => s.spellId)).subscribe((data) => {
                this.monsterSpells = data;
            });
        }
    }

    public abilityChange(ability: Ability) {
        this.monster.abilities[ability].modifier = parseInt((this.monster.abilities[ability].value / 2) + "") - 5
    }

    public enumValues(enumType): any[] {
        switch (enumType) {
            case "Size":
                return Object.keys(Size).filter(key => !isNaN(Number(Size[key])));
            case "MonsterType":
                return Object.keys(MonsterType).filter(key => !isNaN(Number(MonsterType[key])));
            case "Ability":
                return Object.keys(Ability).filter(key => !isNaN(Number(Ability[key])));
        }
    }
}
