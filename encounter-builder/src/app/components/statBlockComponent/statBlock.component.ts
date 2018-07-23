import { Component, OnInit, Input } from '@angular/core';

import {Monster, SavingThrow, Skill, Spellcasting } from "../../core/models/monster";

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

    public getSpellLines(sc: Spellcasting) {
        var lines: any[] = [];
        for (let i = 0; i < sc.spells.length; i++) {
            if (sc.spells[i] !== null) {
                lines[lines.length] = {
                    level: this.getTextForSpellcastingHeader(sc, i),
                    spelllist: sc.spells[i].map(s => s.name + (s.marked ? '*' : '')).join(', ').toLowerCase()
                }
            }
        }
        return lines;
    }

    thIfy(number: number): string {
        if (number === 1)
            return '1st';
        if (number === 2)
            return '2nd';
        if (number === 3)
            return '3rd';
        return number + 'th';
    }

    getTextForSpellcastingHeader(sc: Spellcasting, i: number): any {
        if(sc.spellListClass !== 'warlock')
            return (i === 0
                ? 'Cantrips (at will)'
                : this.thIfy(i) + ' level (' + sc.spellslots[i - 1] + ' slot' + (sc.spellslots[i - 1] > 1 ? 's)' : ')'));
        return (i === 0
            ? 'Cantrips (at will)'
            : '1st-' + this.thIfy(i) + ' level (' + sc.spellslots[i - 1] + ' ' + this.thIfy(i) +'-level slots)');
    }
}