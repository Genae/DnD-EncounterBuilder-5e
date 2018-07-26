import { Component, OnInit, Input } from '@angular/core';

import { DataService } from "../../core/services/data.service";
import { Monster } from "../../core/models/monster";
import { ActivatedRoute } from '@angular/router';

@Component({
    selector: 'monsterDetail',
    templateUrl: 'monsterDetail.component.html'
})

export class MonsterDetailComponent implements OnInit {

    @Input() monster: Monster;
    public index: number = 0;
    id: number;
    private sub: any;

    constructor(private route: ActivatedRoute, private dataService: DataService) {
        
    }

    ngOnInit() {
        this.sub = this.route.params.subscribe(params => {
            this.id = params['id'];

            // In a real app: dispatch action to load the details here.
        });
    }
}