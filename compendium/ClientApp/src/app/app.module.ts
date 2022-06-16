import { BrowserModule } from '@angular/platform-browser';
import { NgModule, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';

import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { HomeComponent } from './components/homeComponent/home.component';
import { MonsterListComponent, FilterPipe } from './components/monsterListComponent/monsterList.component';
import { MonsterDetailComponent } from './components/monsterDetailComponent/monsterDetail.component';
import { StatBlockComponent } from './components/statBlockComponent/statBlock.component';
import { SpellListComponent, FilterSpellsPipe } from './components/spellListComponent/spellList.component';
import { SpellDetailComponent } from './components/spellDetailComponent/spellDetail.component';
import { DataService } from './services/data.service';
import { DataTableModule } from "angular-6-datatable";
import { ProjectService } from './services/project.service';
import { ProjectListComponent } from './components/projectListComponent/projectList.component';
import { ProjectEditComponent } from './components/projectEditComponent/projectEdit.component';
import { ProjectDetailComponent } from './components/projectDetailComponent/projectDetail.component';

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
        SpellDetailComponent,
        SpellListComponent,
        StatBlockComponent,
        FilterPipe,
        FilterSpellsPipe
    ],
    imports: [
        BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
        HttpClientModule,
        FormsModule,
        DataTableModule,
        RouterModule.forRoot([
            { path: '', component: HomeComponent, pathMatch: 'full' },
            { path: 'projectList', component: ProjectListComponent },
            { path: 'projectDetail/:id', component: ProjectDetailComponent },
            { path: 'projectDetail/:id/edit', component: ProjectEditComponent },
            { path: 'monsterList', component: MonsterListComponent },
            { path: 'monsterDetail/:id', component: MonsterDetailComponent },
            { path: 'spellList', component: SpellListComponent },
            { path: 'spellDetail/:id', component: SpellDetailComponent },
        ])
    ],
    providers: [DataService, ProjectService],
    bootstrap: [AppComponent],
    schemas: [CUSTOM_ELEMENTS_SCHEMA]
})
export class AppModule { }
