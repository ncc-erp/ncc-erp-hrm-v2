import { SharedModule } from './../../../shared/shared.module';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ListBenefitComponent } from './list-benefit/list-benefit.component';
import { BenefitRoutingModule } from './benefit-routing.module';
import { BenefitDetailComponent } from './benefit-detail/benefit-detail.component';
import { CreateEditBenefitDialogComponent } from './create-edit-benefit-dialog/create-edit-benefit-dialog.component';
import { BenefitInfomationComponent } from './benefit-detail/benefit-infomation/benefit-infomation.component';
import { BenefitEmployeeComponent } from './benefit-detail/benefit-employee/benefit-employee.component';
import { DatePipe } from '@angular/common';
import { AddBenefitEmployeeDialogComponent } from './benefit-detail/benefit-employee/add-benefit-employee-dialog/add-benefit-employee-dialog.component';
import { UpdateBenefitDateDialogComponent } from './benefit-detail/benefit-employee/update-benefit-date-dialog/update-benefit-date-dialog.component';
import { IConfig, NgxMaskModule } from 'ngx-mask';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

export const options: Partial<IConfig> | (() => Partial<IConfig>) = null;

@NgModule({
  declarations: [
    ListBenefitComponent,
    BenefitDetailComponent,
    CreateEditBenefitDialogComponent,
    BenefitInfomationComponent,
    BenefitEmployeeComponent,
    AddBenefitEmployeeDialogComponent,
    UpdateBenefitDateDialogComponent,
  ],
  imports: [
    CommonModule,
    SharedModule,
    BenefitRoutingModule,
    FormsModule,
    ReactiveFormsModule,
    NgxMaskModule.forRoot(options),
  ],
  providers: [
    DatePipe,
  ]
})
export class BenefitsModule { }
