import { AfterViewInit, Component, Input, OnDestroy } from '@angular/core';

import { DataService } from "../../services/data.service";
import { Monster, PreparedSpell } from "../../models/monster";
import { Spell } from "../../models/spell";
import { Router } from '@angular/router';
import { Pipe, PipeTransform } from '@angular/core';
import { Subject } from 'rxjs';

@Component({
    selector: 'monsterList',
    templateUrl: 'monsterList.component.html'
})

export class MonsterListComponent implements AfterViewInit, OnDestroy {

    monsters: Monster[] = [];
    search: any;
    dtTrigger: Subject<any> = new Subject<any>();
    dtOptions: DataTables.Settings = {};

    @Input() ids: string[];

    constructor(private dataService: DataService, private router: Router) {
        this.search = {};
    }

    ngAfterViewInit() {
        this.loadMonsters();
    }

    ngOnChanges(changes: any) {
        this.loadMonsters();
    }

    private loadMonsters() {
        if (this.ids) {
            this.dataService.getMonstersFromIds(this.ids).subscribe(response => {
                this.monsters = response;
                this.dtTrigger.next();
            });
        }
        else {
            this.dataService.getMonsters().subscribe(response => {
                if (this.ids === undefined)
                    this.monsters = response
                this.dtTrigger.next();
            });
        }
    }

    ngOnDestroy(): void {
        // Do not forget to unsubscribe the event
        this.dtTrigger.unsubscribe();
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
