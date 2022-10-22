import { Component } from '@angular/core';

import { Monster, PreparedSpell } from "../../models/monster";
import { Spell } from "../../models/spell";
import { Router, ActivatedRoute } from '@angular/router';
import { MonsterService } from '../../services/monster.service';
import { SpellService } from '../../services/spell.service';

@Component({
    selector: 'monsterDetail',
    templateUrl: 'monsterDetail.component.html'
})

export class MonsterDetailComponent {

    constructor(private monsterService: MonsterService, private route: ActivatedRoute, private router: Router, private spellService: SpellService) {

        this.route.params.subscribe(params => {
            this.monsterService.getMonsterById(params['id']).subscribe(response => this.monsterUpdated(response as Monster));
        });

    }

    monster: Monster | undefined = undefined;
    monsterSpells: Spell[] = [];

    public edit() {
        if (this.monster === undefined)
            return;
        this.router.navigateByUrl('/monsterDetail/' + this.monster.id + "/edit");
    }

    public monsterUpdated(monster: Monster) {
        this.monsterSpells = [];
        this.monster = monster;
        if (monster.spellcasting !== undefined && monster.spellcasting.spells.length > 0) {
            var flattened = monster.spellcasting.spells.flat().filter((a: PreparedSpell) => a !== null);
            this.spellService.getSpellsFromIds(flattened.map((s: PreparedSpell) => s.spellId)).subscribe((data) => {
                this.monsterSpells = data;
            });
        }
    }
}
