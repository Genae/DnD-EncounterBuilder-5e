import { BrowserModule } from '@angular/platform-browser';
import { NgModule, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';

import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { HomeComponent } from './home/home.component';
import { MonsterListComponent, FilterPipe } from './components/monsterListComponent/monsterList.component';
import { MonsterDetailComponent } from './components/monsterDetailComponent/monsterDetail.component';
import { StatBlockComponent } from './components/statBlockComponent/statBlock.component';
import { SpellDetailComponent } from './components/spellDetailComponent/spellDetail.component';
import { DataTableModule } from "angular-6-datatable";

@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    HomeComponent,
    MonsterListComponent,
    MonsterDetailComponent,
    SpellDetailComponent,
    StatBlockComponent,
    FilterPipe
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule,
    DataTableModule,
    RouterModule.forRoot([
      { path: '', component: HomeComponent, pathMatch: 'full' },
      { path: 'monsterList', component: MonsterListComponent },
      { path: 'monsterDetail/:id', component: MonsterDetailComponent },
    ])
  ],
  providers: [],
  bootstrap: [AppComponent],
  schemas: [CUSTOM_ELEMENTS_SCHEMA]
})
export class AppModule { }
