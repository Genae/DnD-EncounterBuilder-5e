import { Component, Input } from '@angular/core';

import { DataService } from "../../core/services/data.service";
import { Monster, PreparedSpell } from "../../core/models/monster";
import { Spell } from "../../core/models/spell";
import { Router } from '@angular/router';

@Component({
    selector: 'monsterList',
    templateUrl: 'monsterList.component.html'
})

export class MonsterListComponent {

    monsters: Monster[] = [];

    constructor(private dataService: DataService, private router: Router) {
        this.dataService.getMonsters().subscribe(response => this.monsters = response);
    }

    public redirect(id: string) {
        this.router.navigateByUrl('/monsterDetails/' + id);
    }
}