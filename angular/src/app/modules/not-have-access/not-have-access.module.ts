import { SharedModule } from './../../../shared/shared.module';

import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';


import { TooltipModule } from 'ngx-bootstrap/tooltip';
import { NotHaveAccessRoutingModule } from './not-have-access-routing.module';
import { NotHaveAccessComponent } from './not-have-access.component';

@NgModule({
  declarations: [
    NotHaveAccessComponent,
  
  ],
  imports: [
    CommonModule,
    SharedModule,
    NotHaveAccessRoutingModule,
  ],
})
export class NotHaveAccessModule {
  constructor() {
  }
}
