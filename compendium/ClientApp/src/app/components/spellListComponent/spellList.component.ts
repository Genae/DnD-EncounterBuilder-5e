import { Component, Input, OnDestroy } from '@angular/core';

import { Spell } from "../../models/spell";
import { Router } from '@angular/router';
import { Pipe, PipeTransform } from '@angular/core';
import { Subject } from 'rxjs';
import { SpellService } from '../../services/spell.service';

@Component({
    selector: 'spellList',
    templateUrl: 'spellList.component.html'
})

export class SpellListComponent implements OnDestroy {

    spells: Spell[] = [];
    search: any;
    @Input() ids: string[]

    constructor(private spellService: SpellService, private router: Router) {
        this.loadSpells();
        this.search = {};
    }

    ngOnDestroy(): void {
        // Do not forget to unsubscribe the event
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
            this.spellService.getSpellsFromIds(this.ids).subscribe(response => {
                this.spells = response;
            });
        }
        else {
            this.spellService.getSpells().subscribe(response => {
                if (this.ids === undefined)
                    this.spells = response;
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
