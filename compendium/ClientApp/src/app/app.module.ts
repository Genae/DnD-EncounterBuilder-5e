import { BrowserModule } from '@angular/platform-browser';
import { CUSTOM_ELEMENTS_SCHEMA, NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';

import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { HomeComponent } from './components/homeComponent/home.component';
import { MonsterListComponent, FilterPipe } from './components/monsterListComponent/monsterList.component';
import { MonsterDetailComponent } from './components/monsterDetailComponent/monsterDetail.component';
import { SpellListComponent, FilterSpellsPipe } from './components/spellListComponent/spellList.component';
import { SpellDetailComponent } from './components/spellDetailComponent/spellDetail.component';
import { DataService } from './services/data.service';
import { ProjectService } from './services/project.service';
import { ProjectListComponent } from './components/projectListComponent/projectList.component';
import { ProjectEditComponent } from './components/projectEditComponent/projectEdit.component';
import { ProjectDetailComponent } from './components/projectDetailComponent/projectDetail.component';
import { MonsterEditComponent } from './components/monsterEditComponent/monsterEdit.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { MaterialModule } from './material.module';

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
        FilterSpellsPipe
    ],
    imports: [
        MaterialModule,
        BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
        HttpClientModule,
        FormsModule,
        BrowserAnimationsModule,
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
    schemas: [CUSTOM_ELEMENTS_SCHEMA],
    providers: [DataService, ProjectService],
    bootstrap: [AppComponent]
})
export class AppModule { }
