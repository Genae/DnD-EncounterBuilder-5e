import { Component, Input } from '@angular/core';

import { DataService } from "../../services/data.service";
import { Monster, PreparedSpell } from "../../models/monster";
import { Spell } from "../../models/spell";
import { Router } from '@angular/router';
import { Pipe, PipeTransform } from '@angular/core';

@Component({
    selector: 'monsterList',
    templateUrl: 'monsterList.component.html'
})

export class MonsterListComponent {

    monsters: Monster[] = [];
    search: any;

    constructor(private dataService: DataService, private router: Router) {
        this.dataService.getMonsters().subscribe(response => this.monsters = response);
        this.search = {};
    }

    public redirect(id: string) {
        this.router.navigateByUrl('/monsterDetails/' + id);
    }
}

@Pipe({
    name: 'filter',
    pure: false
})
export class FilterPipe implements PipeTransform {
    transform(items: any[], filter: any): any[] {
        if (!items) return [];
        if (!filter.name) return items;
        let searchtext = filter.name.toLowerCase();
        return items.filter(it => {
            return it.name.toLowerCase().includes(searchtext);
        });
    }
}
