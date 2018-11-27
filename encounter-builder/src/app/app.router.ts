import { ModuleWithProviders } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { HomeComponent } from "./components/home/home.component";
import { MonsterDetailComponent } from "./components/monsterDetailComponent/monsterDetail.component";
import { MonsterListComponent } from "./components/monsterListComponent/monsterList.component";

const appRoutes: Routes = [
    { path: 'home', component: HomeComponent },
    { path: '', component: HomeComponent },
    { path: 'monsterList', component: MonsterListComponent },
    { path: 'monsterDetails/:id', component: MonsterDetailComponent },
    // otherwise redirect to home
    //{ path: '**', redirectTo: '', pathMatch:'full' }
];
 
export const routing: ModuleWithProviders = RouterModule.forRoot(appRoutes, { useHash: true});
