import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SalaryChangeRequestDetailComponent } from './salary-change-request-detail/salary-change-request-detail.component';
import { SalaryChangeRequestListComponent } from './salary-change-request-list/salary-change-request-list.component';
import {SalaryChangeRequestsRoutingModule} from './salary-change-requests-routing.module'
import { SharedModule } from '@shared/shared.module';
import { CreateSalaryChangeRequestComponent } from './create-salary-change-request/create-salary-change-request.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { AddEmployeeToSalaryChangeRequestComponent } from './salary-change-request-detail/add-employee-to-salary-change-request/add-employee-to-salary-change-request.component';
import { UploadContractFileComponent } from './salary-change-request-detail/add-employee-to-salary-change-request/upload-contract-file/upload-contract-file.component';
import { ImportCheckpointComponent } from './salary-change-request-detail/import-checkpoint/import-checkpoint.component';

@NgModule({
  declarations: [
    SalaryChangeRequestDetailComponent,
    SalaryChangeRequestListComponent,
    CreateSalaryChangeRequestComponent,
    AddEmployeeToSalaryChangeRequestComponent,
    UploadContractFileComponent,
    ImportCheckpointComponent
  ],
  imports: [
    CommonModule,
    SharedModule,
    SalaryChangeRequestsRoutingModule,
    FormsModule,
    ReactiveFormsModule
  ],
})
export class SalaryChangeRequestsModule { }
