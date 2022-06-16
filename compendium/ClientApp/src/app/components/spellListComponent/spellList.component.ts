import { Component, Input } from '@angular/core';

import { DataService } from "../../services/data.service";
import { Monster, PreparedSpell } from "../../models/monster";
import { Spell } from "../../models/spell";
import { Router } from '@angular/router';
import { Pipe, PipeTransform } from '@angular/core';

@Component({
    selector: 'spellList',
    templateUrl: 'spellList.component.html'
})

export class SpellListComponent {

    spells: Spell[] = [];
    search: any;
    @Input() ids: string[]

    constructor(private dataService: DataService, private router: Router) {
        this.loadSpells();
        this.search = {};
    }

    public redirect(id: string) {
        this.router.navigateByUrl('/spellDetails/' + id);
    }


    ngOnChanges(changes: any) {
        console.log(changes);
        this.loadSpells();
    }

    private loadSpells() {
        if (this.ids) {
            this.dataService.getSpellsFromIds(this.ids).subscribe(response => this.spells = response);
        }
        else {
            this.dataService.getSpells().subscribe(response => {
                if (this.ids === undefined)
                    this.spells = response
            });
        }
    }
}

@Pipe({
    name: 'filterSpells',
    pure: false
})
export class FilterSpellsPipe implements PipeTransform {
    transform(items: any[], filter: any): any[] {
        if (!items) return [];
        if (!filter.name) return items;
        let searchtext = filter.name.toLowerCase();
        return items.filter(it => {
            return it.name.toLowerCase().includes(searchtext);
        });
    }
}
