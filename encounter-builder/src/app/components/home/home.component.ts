import { Component, OnInit } from '@angular/core';

import { HomeService } from "../../core/services/home.service";
import {Monster} from "../../core/models/monster";

@Component({
    selector: 'home',
    templateUrl: 'home.component.html'
})

export class HomeComponent implements OnInit {

    monsters: Monster[] = [];
    public index: number = 0;

    constructor(private hserv: HomeService) {
        this.hserv.GetHomeMessage().subscribe(response => this.monsters = response);
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