import { ModuleWithProviders } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { HomeComponent } from "./components/home/home.component";

const appRoutes: Routes = [
    { path: 'home', component: HomeComponent },
    { path: '', component: HomeComponent },
    // otherwise redirect to home
    { path: '**', redirectTo: '', pathMatch:'full' }
];
 
export const routing: ModuleWithProviders = RouterModule.forRoot(appRoutes, { useHash: true});
