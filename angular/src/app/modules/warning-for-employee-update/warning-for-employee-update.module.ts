import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BackToWorkRoutingModule } from './warning-for-employee-update-routing.module';
import { SharedModule } from '@shared/shared.module';
import { UpdateContractComponent } from './update-contract/update-contract.component';
import { BackToWorkComponent } from './back-to-work/back-to-work.component';
import { UpdateEmployeeBackDateComponent } from './back-to-work/update-employee-back-date/update-employee-back-date.component';
import { TempEmployeeTalentComponent } from './temp-employee-talent/temp-employee-talent.component';
import { TempEmployeeTsComponent } from './temp-employee-ts/temp-employee-ts.component';
import { PlanToQuitComponent } from './plan-to-quit/plan-to-quit.component';
import { UpdateDateComponent } from './plan-to-quit/update-date/update-date.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatNativeDateModule } from '@angular/material/core';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatDialogModule } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatListModule } from '@angular/material/list';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatSelectModule } from '@angular/material/select';
import { EmployeeDetailDialogComponent } from './temp-employee-talent/employee-detail-dialog/employee-detail-dialog.component';
import { MultiCreateEmployeeFromTempComponent } from './temp-employee-talent/multi-create-employee-from-temp/multi-create-employee-from-temp.component';



@NgModule({
  declarations: [
    UpdateContractComponent,
    BackToWorkComponent,
    UpdateEmployeeBackDateComponent,
    TempEmployeeTalentComponent,
    TempEmployeeTsComponent,
    PlanToQuitComponent,
    UpdateDateComponent,
    MultiCreateEmployeeFromTempComponent,
    EmployeeDetailDialogComponent
  ],
  imports: [
    CommonModule,
    BackToWorkRoutingModule,
    SharedModule,
    MatProgressBarModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    FormsModule,
    ReactiveFormsModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatCardModule,
    MatButtonModule,
    MatListModule
  ]
})
export class BackToWorkModule { }
