import { Component, OnInit } from '@angular/core';

import { Monster } from "../../core/models/monster";
import { DataService } from "../../core/services/data.service";

@Component({
    selector: 'home',
    templateUrl: 'home.component.html'
})

export class HomeComponent implements OnInit {

    monsters: Monster[] = [];
    public index: number = 0;

    constructor(private dataService: DataService) {
        (Object.prototype as any).isEmpty = function () {
            for (var key in this) {
                if (this.hasOwnProperty(key))
                    return false;
            }
            return true;
        }

        this.dataService.getMonsters().subscribe(response => this.monsters = response);
    }

    updateIndex(delta: number) {
        this.index += delta;
        if (this.index < 0) {
            this.index = this.monsters.length + this.index;
        }
        if (this.index >= this.monsters.length) {
            this.index = this.index - this.monsters.length;
        }
    }

    ngOnInit(): void {
    }

}