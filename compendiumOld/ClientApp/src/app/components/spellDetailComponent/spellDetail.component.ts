import { Component, Input } from '@angular/core';

import { DataService } from "../../services/data.service";
import { Spell } from "../../models/spell";
import { Router, ActivatedRoute, ParamMap } from '@angular/router';

@Component({
    selector: 'spellDetail',
    templateUrl: 'spellDetail.component.html'
})

export class SpellDetailComponent {

    constructor(private dataService: DataService, private route: ActivatedRoute) {
      this.route.params.subscribe(params => {
          if (params['id'])
            this.dataService.getSpellById(params['id']).subscribe(response => this.spellUpdated(response));
        });
    }

  spellUpdated(spell: Spell): void {
    if (spell) {
      this.spell = spell;
    }
    }

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
