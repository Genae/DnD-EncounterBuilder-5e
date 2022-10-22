import { AfterViewInit, Component, Input, OnDestroy } from '@angular/core';

import { Monster } from "../../models/monster";
import { Router } from '@angular/router';
import { Pipe, PipeTransform } from '@angular/core';
import { MonsterService } from '../../services/monster.service';

@Component({
    selector: 'monsterList',
    templateUrl: 'monsterList.component.html'
})

export class MonsterListComponent implements AfterViewInit, OnDestroy {

    monsters: Monster[] = [];
    search: any;

    @Input() ids: string[];

    constructor(private monsterService: MonsterService, private router: Router) {
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
            this.monsterService.getMonstersFromIds(this.ids).subscribe(response => {
                this.monsters = response;
            });
        }
        else {
            this.monsterService.getMonsters().subscribe(response => {
                if (this.ids === undefined)
                    this.monsters = response
            });
        }
    }

    ngOnDestroy(): void {
        // Do not forget to unsubscribe the event
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
