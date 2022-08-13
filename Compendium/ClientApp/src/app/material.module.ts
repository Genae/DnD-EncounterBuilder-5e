import { NgModule } from '@angular/core';

import { MatSidenavModule } from '@angular/material/sidenav';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatIconModule } from '@angular/material/icon';
import { MatSliderModule } from '@angular/material/slider';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatSelectModule } from '@angular/material/select';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatStepperModule } from '@angular/material/stepper';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';


@NgModule({
    exports: [
        MatSidenavModule,
        MatToolbarModule,
        MatIconModule,
        MatSliderModule,
        MatCheckboxModule,
        MatFormFieldModule,
        MatSelectModule,
        MatInputModule,
        MatStepperModule,
        MatSlideToggleModule
    ]
})
export class MaterialModule { }