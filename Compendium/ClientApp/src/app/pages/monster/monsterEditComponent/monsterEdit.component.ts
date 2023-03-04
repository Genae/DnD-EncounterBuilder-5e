import {Component, OnInit} from '@angular/core';

import {
    Monster,
    PreparedSpell,
    Ability,
    DamageType,
    ArmorInfo,
    Multiattack,
    Action,
    Attack,
    AttackType,
    Senses,
    HitEffect,
    SavingThrow
} from "../../../models/monster";
import { Spell } from "../../../models/spell";
import { Router, ActivatedRoute } from '@angular/router';
import { TextgenService } from '../../../services/textgen.service';
import { MonsterService } from '../../../services/monster.service';
import { SpellService } from '../../../services/spell.service';
import { Project } from '../../../models/project';
import {FormControl} from "@angular/forms";
import {HitDieSize, HitDieSizeList} from "../../../models/lists/hitDieSizeList";
import {MonsterBasicInfoComponent} from "./monster-basic-info/monster-basic-info.component";
import {MonsterDefenseComponent} from "./monster-defense/monster-defense.component";
import {AbilityScoreComponent} from "./ability-score/ability-score.component";

@Component({
    selector: 'monsterEdit',
    templateUrl: 'monsterEdit.component.html'
})

export class MonsterEditComponent implements OnInit {

    constructor(private monsterService: MonsterService, private spellService: SpellService, private textGen: TextgenService, private route: ActivatedRoute, private router: Router) {

        this.route.params.subscribe(params => {
            if (params['id'])
                this.monsterService.getMonsterById(params['id']).subscribe(response => this.monsterUpdated(response as Monster));
        });
    }

    public formGroups: { [id: string]: { [id: string]: FormControl; }; } = {
        'basic': MonsterBasicInfoComponent.initForm({}),
        'abilities': AbilityScoreComponent.initForm({}),
        'defence': MonsterDefenseComponent.initForm({})
    }

    ngOnInit(): void {
    }

    public view() {
        if (this.monster === undefined)
            return;
        this.router.navigateByUrl('/monsterDetail/' + this.monster.id);
    }

    monster: Monster;
    save: { [id: string]: boolean; } = {}

    monsterSpells: Spell[];


    public updateProjectTags(projects: Project[]) {
        this.monster.projectTags = projects.map(p => p.id);
    }
    
    public submit() {
        this.monsterService.saveMonster(this.monster).subscribe(m => {
            this.monster = m;
            this.view();
        });
    }

    public monsterUpdated(monster: Monster) {
        this.monster = monster;
        this.monsterSpells = [];
        if (!this.monster.armorInfo)
            this.monster.armorInfo = new ArmorInfo();


        if (this.monster.spellcasting !== undefined && this.monster.spellcasting.spells.length > 0) {
            var flattened = this.monster.spellcasting.spells.flat().filter((a: PreparedSpell) => a !== null);
            this.spellService.getSpellsFromIds(flattened.map((s: PreparedSpell) => s.spellId)).subscribe((data) => {
                this.monsterSpells = data;
            });
        }

        this.route.queryParams.subscribe((params) => {
            if (params.pId && this.monster.projectTags.indexOf(params.pId) === -1) {
                this.monster.projectTags.push(params.pId);
            }
        });

    }
}
