import { Component, OnInit, Input } from '@angular/core';

import {Monster, SavingThrow, Skill } from "../../core/models/monster";

@Component({
    selector: 'statBlock',
    templateUrl: 'statBlock.component.html'
})

export class StatBlockComponent implements OnInit {

    @Input() monster: Monster; 

    ngOnInit(): void {
    }

    public describeSavingThrows(saves: SavingThrow[]) {
        var str = "";
        for (var save of saves) {
            str += save.ability.substr(0, 3) + " " + (save.modifier > 0 ? "+" + save.modifier : save.modifier) + ", ";
        }
        return str.substr(0, str.length - 2);
    }

    public describeSkills(skills: Skill[]) {
        var str = "";
        for (var skill of skills) {
            str += skill.skill + " " + (skill.modifier > 0 ? "+" + skill.modifier : skill.modifier) + ", ";
        }
        return str.substr(0, str.length - 2);
    }
}