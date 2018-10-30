import { Component, Input } from '@angular/core';

import { DataService } from "../../core/services/data.service";
import { Monster, PreparedSpell } from "../../core/models/monster";
import { ActivatedRoute } from '@angular/router';
import { Spell } from "../../core/models/spell";

@Component({
    selector: 'monsterDetail',
    templateUrl: 'monsterDetail.component.html'
})

export class MonsterDetailComponent {

    constructor(private dataService: DataService) { }

    @Input() monsters: Monster[];
    currentIndex: number;
    monsterSpells: Spell[];
    @Input()
    set index(index: number) {
        this.currentIndex = index;
        this.indexUpdated();
    }

    public indexUpdated() {
        this.monsterSpells = [];
        const monster = this.monsters[this.currentIndex];
        if (monster.spellcasting !== undefined && monster.spellcasting.spells.length > 0) {
            var flattened = [].concat.apply([], monster.spellcasting.spells).filter((a: PreparedSpell) => a !== null);
            this.dataService.getSpellsFromIds(flattened.map((s: PreparedSpell) => s.spellId)).subscribe((data) => {
                this.monsterSpells = data;
            });
        }
    }
}