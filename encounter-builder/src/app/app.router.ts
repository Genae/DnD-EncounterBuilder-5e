import { ModuleWithProviders } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { HomeComponent } from "./components/home/home.component";
import { MonsterDetailComponent } from "./components/monsterDetailComponent/monsterDetail.component";

const appRoutes: Routes = [
    { path: 'home', component: HomeComponent },
    { path: '', component: HomeComponent },
    { path: 'monsterDetails/{id}', component: MonsterDetailComponent },
    // otherwise redirect to home
    { path: '**', redirectTo: '', pathMatch:'full' }
];
 
export const routing: ModuleWithProviders = RouterModule.forRoot(appRoutes, { useHash: true});
