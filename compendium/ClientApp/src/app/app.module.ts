import { BrowserModule } from '@angular/platform-browser';
import { CUSTOM_ELEMENTS_SCHEMA, NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';

import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { HomeComponent } from './pages/home/home.component';
import { MonsterListComponent, FilterPipe } from './pages/monster/monsterListComponent/monsterList.component';
import { MonsterDetailComponent } from './pages/monster/monsterDetailComponent/monsterDetail.component';
import { SpellListComponent, FilterSpellsPipe } from './pages/spell/spellListComponent/spellList.component';
import { SpellDetailComponent } from './pages/spell/spellDetailComponent/spellDetail.component';
import { ProjectService } from './services/project.service';
import { ProjectListComponent } from './pages/project/projectListComponent/projectList.component';
import { ProjectEditComponent } from './pages/project/projectEditComponent/projectEdit.component';
import { ProjectDetailComponent } from './pages/project/projectDetailComponent/projectDetail.component';
import { MonsterEditComponent } from './pages/monster/monsterEditComponent/monsterEdit.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { MaterialModule } from './material.module';
import { TextgenService } from './services/textgen.service';
import { MonsterService } from './services/monster.service';
import { SpellService } from './services/spell.service';
import { WeaponTypeService } from './services/weaponType.service';
import { ProjectSelectionComponent } from './components/projectSelectionComponent/projectSelection.component';
import {NgxMatSelectSearchModule} from "ngx-mat-select-search";
import { AbilityScoreComponent } from './pages/monster/monsterEditComponent/ability-score/ability-score.component';
import { MonsterDefenseComponent } from './pages/monster/monsterEditComponent/monster-defense/monster-defense.component';
import { MonsterBasicInfoComponent } from './pages/monster/monsterEditComponent/monster-basic-info/monster-basic-info.component';

@NgModule({
    declarations: [
        AppComponent,
        NavMenuComponent,
        HomeComponent,
        ProjectListComponent,
        ProjectDetailComponent,
        ProjectEditComponent,
        MonsterListComponent,
        MonsterDetailComponent,
        MonsterEditComponent,
        SpellDetailComponent,
        SpellListComponent,
        FilterPipe,
        FilterSpellsPipe,
        ProjectSelectionComponent,
        AbilityScoreComponent,
        MonsterDefenseComponent,
        MonsterBasicInfoComponent
    ],
    imports: [
        MaterialModule,
        BrowserModule.withServerTransition({appId: 'ng-cli-universal'}),
        HttpClientModule,
        FormsModule,
        BrowserAnimationsModule,
        ReactiveFormsModule,
        RouterModule.forRoot([
            {path: '', component: HomeComponent, pathMatch: 'full'},
            {path: 'projectList', component: ProjectListComponent},
            {path: 'projectDetail/:id', component: ProjectDetailComponent},
            {path: 'projectDetail/:id/edit', component: ProjectEditComponent},
            {path: 'monsterList', component: MonsterListComponent},
            {path: 'monsterDetail/:id', component: MonsterDetailComponent},
            {path: 'monsterDetail/:id/edit', component: MonsterEditComponent},
            {path: 'spellList', component: SpellListComponent},
            {path: 'spellDetail/:id', component: SpellDetailComponent},
        ]),
        NgxMatSelectSearchModule
    ],
    schemas: [CUSTOM_ELEMENTS_SCHEMA],
    providers: [MonsterService, ProjectService, TextgenService, SpellService, WeaponTypeService],
    bootstrap: [AppComponent]
})
export class AppModule { }
