import { AfterViewInit, Component, Input, ViewChild } from '@angular/core';

import { Monster } from "../../../models/monster";
import { Router } from '@angular/router';
import { Pipe, PipeTransform } from '@angular/core';
import { MonsterService } from '../../../services/monster.service';
import {MatPaginator} from '@angular/material/paginator';
import {MatSort} from '@angular/material/sort';
import {MatTableDataSource} from '@angular/material/table';

@Component({
    selector: 'monsterList',
    templateUrl: 'monsterList.component.html'
})

export class MonsterListComponent implements AfterViewInit {

    displayedColumns: string[] = ['name', 'type', 'cr', 'hp'];
    dataSource: MatTableDataSource<Monster>;    
    monsters: Monster[] = [];

    @Input() ids: string[];
    @ViewChild(MatPaginator) paginator: MatPaginator;
    @ViewChild(MatSort) sort: MatSort;

    constructor(private monsterService: MonsterService, private router: Router) { }

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
                this.createDatasorce()
            });
        }
        else {
            this.monsterService.getMonsters().subscribe(response => {
                if (this.ids === undefined){
                    this.monsters = response       
                    this.createDatasorce()
                }
            });
        }
    }
    
    createDatasorce() {
        this.dataSource = new MatTableDataSource(this.monsters)
        this.dataSource.sortingDataAccessor = (item, property) => {
            switch(property) {
                case 'cr': return item.challengeRating.description;
                case 'type': return item.race.monsterType;
                case 'hp': return item.hitDie.expectedRoll;
                default: return (item as any)[property];
            }
        };
        this.dataSource.paginator = this.paginator;
        this.dataSource.sort = this.sort;
        
    }

    applyFilter(event: Event) {
        const filterValue = (event.target as HTMLInputElement).value;
        this.dataSource.filter = filterValue.trim().toLowerCase();

        if (this.dataSource.paginator) {
            this.dataSource.paginator.firstPage();
        }
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
