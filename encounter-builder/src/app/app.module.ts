import { NgModule, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule } from '@angular/forms';
import { HttpModule } from '@angular/http';
import { AppComponent } from './app.component';
import { HomeComponent } from './components/home/home.component';
import { routing } from './app.router';

import { StatBlockComponent } from "./components/statBlockComponent/statBlock.component";
import { DataService } from "./core/services/data.service";
import { MonsterDetailComponent } from "./components/monsterDetailComponent/monsterDetail.component";
import { SpellDetailComponent } from "./components/spellDetailComponent/spellDetail.component";
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { MonsterListComponent, FilterPipe } from "./components/monsterListComponent/monsterList.component";
import { DataTableModule } from "angular-6-datatable";


@NgModule({
    imports: [
        BrowserModule,
        FormsModule,
        HttpModule,
        BrowserAnimationsModule,
        DataTableModule,
        routing
    ],
    declarations: [
        HomeComponent,
        MonsterDetailComponent,
        AppComponent,
        StatBlockComponent,
        SpellDetailComponent,
        MonsterListComponent,
        FilterPipe
    ],
    providers: [DataService],
    bootstrap: [AppComponent],
    schemas: [CUSTOM_ELEMENTS_SCHEMA]
 })
export class AppModule { }
