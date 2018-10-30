import { Component, Input } from '@angular/core';

import { DataService } from "../../core/services/data.service";
import { Spell } from "../../core/models/spell";

@Component({
    selector: 'spellDetail',
    templateUrl: 'spellDetail.component.html'
})

export class SpellDetailComponent {

    constructor(private dataService: DataService) { }

    @Input() spell: Spell;

    getComponents() {
        let d = [];
        if (this.spell.vocalComponent)
            d.push("V");
        if (this.spell.vocalComponent)
            d.push("S");
        if (this.spell.materials !== undefined)
            d.push("M (" + this.spell.materials + ")");
        return d.join(", ");
    }
}