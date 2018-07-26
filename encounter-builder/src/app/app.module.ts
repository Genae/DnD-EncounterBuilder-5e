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

@NgModule({
    imports: [
        BrowserModule,
        FormsModule,
        HttpModule,
        routing
    ],
    declarations: [
        HomeComponent,
        MonsterDetailComponent,
        AppComponent,
        StatBlockComponent
    ],
    providers: [DataService],
    bootstrap: [AppComponent],
    schemas: [CUSTOM_ELEMENTS_SCHEMA]
 })
export class AppModule { }
