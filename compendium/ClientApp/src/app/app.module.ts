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
import { DataTablesModule } from "angular-datatables";
import { ProjectService } from './services/project.service';
import { ProjectListComponent } from './components/projectListComponent/projectList.component';
import { ProjectEditComponent } from './components/projectEditComponent/projectEdit.component';
import { ProjectDetailComponent } from './components/projectDetailComponent/projectDetail.component';
import { MonsterEditComponent } from './components/monsterEditComponent/monsterEdit.component';

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
        StatBlockComponent,
        FilterPipe,
        FilterSpellsPipe
    ],
    imports: [
        BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
        HttpClientModule,
        FormsModule,
        DataTablesModule,
        RouterModule.forRoot([
            { path: '', component: HomeComponent, pathMatch: 'full' },
            { path: 'projectList', component: ProjectListComponent },
            { path: 'projectDetail/:id', component: ProjectDetailComponent },
            { path: 'projectDetail/:id/edit', component: ProjectEditComponent },
            { path: 'monsterList', component: MonsterListComponent },
            { path: 'monsterDetail/:id', component: MonsterDetailComponent },
            { path: 'monsterDetail/:id/edit', component: MonsterEditComponent },
            { path: 'spellList', component: SpellListComponent },
            { path: 'spellDetail/:id', component: SpellDetailComponent },
        ])
    ],
    providers: [DataService, ProjectService],
    bootstrap: [AppComponent],
    schemas: [CUSTOM_ELEMENTS_SCHEMA]
})
export class AppModule { }
