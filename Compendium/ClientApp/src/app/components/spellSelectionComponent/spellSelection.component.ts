import { COMMA, ENTER } from '@angular/cdk/keycodes';
import {Component, ElementRef, EventEmitter, Input, OnInit, Output, SimpleChanges, ViewChild} from '@angular/core';
import { FormControl } from '@angular/forms';
import { MatAutocompleteSelectedEvent } from '@angular/material/autocomplete';
import { MatChipInputEvent } from '@angular/material/chips';
import { first, map, Observable, startWith } from 'rxjs';
import { Spell } from '../../models/spell';
import { SpellService } from '../../services/spell.service';


@Component({
    selector: 'spellSelection',
    templateUrl: 'spellSelection.component.html'
})

export class SpellSelectionComponent implements OnInit {

    separatorKeysCodes: number[] = [ENTER, COMMA];
    spellCtrl = new FormControl('');
    filteredSpells: Observable<Spell[]>;
    allSpells: Spell[];
    label: string;
    spells: Spell[] = [];

    @Input() spellTags: string[]
    @Input() spellLabel: string
    @Input() level: number;
    @Input() listClass: string;
    @Output() updateList = new EventEmitter<Spell[]>();

    @ViewChild('spellInput') spellInput: ElementRef<HTMLInputElement>;

    constructor(private spellService: SpellService) {
        
    }

    ngOnInit(): void {
        this.spellService.getSpellsWithParams({listClass: this.listClass, level: this.level}).pipe(first()).subscribe(res => {
            this.allSpells = res;
            this.convertTagsToSpell();
            this.setLabel();
            this.filteredSpells = this.spellCtrl.valueChanges.pipe(startWith(null), map(proj => this._filter(proj)));
        })
    }

    ngOnChanges(changes: SimpleChanges) {
        this.setLabel();
        this.convertTagsToSpell();
    }
    convertTagsToSpell() {
        if (this.allSpells)
            this.spells = this.allSpells.filter(p => this.spellTags.indexOf(p.id) !== -1)
    }

    addSpell(event: MatChipInputEvent): void {
        const value = (event.value || '').trim();

        // Add our spell
        if (value) {
            let spell = this.allSpells.find(p => p.name == value)
            this.spells.push(spell!);
            this.updateList.emit(this.spells);
        }

        // Clear the input value
        event.chipInput!.clear();

        this.spellCtrl.setValue(null);
        this.setLabel();
    }

    removeSpell(spell: Spell): void {
        const index = this.spells.indexOf(spell);

        if (index >= 0) {
            this.spells.splice(index, 1);
            this.updateList.emit(this.spells);
        }
        this.spellCtrl.setValue(null);
        this.setLabel();
    }

    selectedSpell(event: MatAutocompleteSelectedEvent): void {
        let spell = this.allSpells.find(p => p.name == event.option.viewValue)
        this.spells.push(spell!);
        this.updateList.emit(this.spells);
        this.spellInput.nativeElement.value = '';
        this.spellCtrl.setValue(null);
    }

    private setLabel() {
        if (this.spells.length == 0)
            this.label = "Add " + this.spellLabel
        else
            this.label = this.spellLabel
    }
    private _filter(search: any): Spell[] {
        let searchLower = "";
        if (search?.name)
            searchLower = "";
        else
            searchLower = search?.toLowerCase() ?? "";
        return this.allSpells.filter(spell => this.spells.indexOf(spell) === -1 && spell.name.toLowerCase().includes(searchLower));
    }
}
