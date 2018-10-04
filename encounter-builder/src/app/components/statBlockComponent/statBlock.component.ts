import { Component, AfterViewInit, Input, ViewChild, ElementRef } from '@angular/core';

import {Monster, SavingThrow, Skill, Spellcasting } from "../../core/models/monster";

var ResizeObserver: any = (window as any).ResizeObserver;
@Component({
    selector: 'statBlock',
    templateUrl: 'statBlock.component.html'
})

export class StatBlockComponent implements AfterViewInit {

    @Input() monster: Monster;

    ngAfterViewInit(): void {
        var element = document.getElementById('statBlockId');
        new ResizeObserver(function() {
            var size = element.getBoundingClientRect();
            var warp = element.shadowRoot.getElementById('content-wrap');
            if (size.height > 800) {
                warp.style.width = (warp.getBoundingClientRect().width - 18.19 + 450) + 'px';
            }
            else if (size.height < 400)
            {
                warp.style.width = size.width < 500 ? '400px' : (warp.getBoundingClientRect().width - 18.19 - 450) + 'px';
            }
        }).observe(element);
    }

    public describeSavingThrows(saves: { [id: string]: number; }) {
        var str = "";
        for (var save in saves) {
            str += save.substr(0, 3) + " " + (saves[save] > 0 ? "+" + saves[save] : saves[save]) + ", ";
        }
        return str.substr(0, str.length - 2);
    }

    public describeSkills(skills: { [id: string]: number; }) {
        var str = "";
        for (var skill in skills) {
            str += skill + " " + (skills[skill] > 0 ? "+" + skills[skill] : skills[skill]) + ", ";
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