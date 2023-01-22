import { Component, Input, OnDestroy, ViewChild } from '@angular/core';

import { Spell } from "../../models/spell";
import { Router } from '@angular/router';
import { Pipe, PipeTransform } from '@angular/core';
import { SpellService } from '../../services/spell.service';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';

@Component({
    selector: 'spellList',
    templateUrl: 'spellList.component.html'
})

export class SpellListComponent implements OnDestroy {
    displayedColumns: string[] = ['name', 'school', 'level', 'classes'];
    dataSource: MatTableDataSource<Spell>;

    @Input() ids: string[];
    @ViewChild(MatPaginator) paginator: MatPaginator;
    @ViewChild(MatSort) sort: MatSort;
    spells: Spell[] = [];

    constructor(private spellService: SpellService, private router: Router) {
        this.loadSpells();
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
                this.dataSource = new MatTableDataSource(this.spells)
                this.dataSource.paginator = this.paginator;
                this.dataSource.sort = this.sort;
            });
        }
        else {
            this.spellService.getSpells().subscribe(response => {
                if (this.ids === undefined){
                    this.spells = response;
                    this.dataSource = new MatTableDataSource(this.spells)
                    this.dataSource.paginator = this.paginator;
                    this.dataSource.sort = this.sort;                    
                }
            });
        }
    }
    applyFilter(event: Event) {
        const filterValue = (event.target as HTMLInputElement).value;
        this.dataSource.filter = filterValue.trim().toLowerCase();

        if (this.dataSource.paginator) {
            this.dataSource.paginator.firstPage();
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
