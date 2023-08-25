import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { BonusesRoutingModule } from './bonuses-routing.module';
import { BonusListComponent } from './bonus-list/bonus-list.component';
import { SharedModule } from './../../../shared/shared.module';
import { CreateEditBonusDialogComponent } from './create-edit-bonus-dialog/create-edit-bonus-dialog.component';
import { BonusDetailComponent } from './bonus-detail/bonus-detail.component';
import { BonusInformationComponent } from './bonus-detail/bonus-information/bonus-information.component';
import { BonusEmployeeComponent } from './bonus-detail/bonus-employee/bonus-employee.component';
import { AddEmployeeBonusDialogComponent } from './bonus-detail/bonus-employee/add-employee-bonus-dialog/add-employee-bonus-dialog.component';
import { NgxMaskModule, IConfig } from 'ngx-mask';
import { ImportEmployeeToBonusComponent } from './bonus-detail/bonus-employee/import-employee-to-bonus/import-employee-to-bonus.component'
export const options: Partial<IConfig> | (() => Partial<IConfig>) = null;

@NgModule({
  declarations: [
    BonusListComponent,
    CreateEditBonusDialogComponent,
    BonusDetailComponent,
    BonusInformationComponent,
    BonusEmployeeComponent,
    AddEmployeeBonusDialogComponent,
    ImportEmployeeToBonusComponent
  ],
  imports: [
    CommonModule,
    SharedModule,
    BonusesRoutingModule,
    NgxMaskModule.forRoot(),
  ]
})
export class BonusesModule { }
