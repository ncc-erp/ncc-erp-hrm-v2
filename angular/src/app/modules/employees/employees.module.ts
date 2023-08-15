import { UploadAvatarComponent } from './upload-avatar/upload-avatar.component';
import { EmployeeRoutingModule } from './employees-routing.module';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { EmployeeListComponent } from './employee-list/employee-list.component';
import { SharedModule } from '@shared/shared.module';
import {EmployeeDetailComponent} from './employee-detail/employee-detail.component';
import { PersonalInfoComponent } from './employee-detail/personal-info/personal-info.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { EmployeeContractComponent } from './employee-detail/employee-contract/employee-contract.component';
import { EmployeeDebtComponent } from './employee-detail/employee-debt/employee-debt.component';
import { EmployeeBenefitComponent } from './employee-detail/employee-benefit/employee-benefit.component';
import { EmployeeBonusComponent } from './employee-detail/employee-bonus/employee-bonus.component';
import { EmployeePunishmentComponent } from './employee-detail/employee-punishment/employee-punishment.component';
import { ImageCropperModule } from 'ngx-image-cropper';
import { EmployeeContractEditNoteComponent } from './employee-detail/employee-contract/employee-contract-edit-note/employee-contract-edit-note.component';
import { EmployeeHistoryComponent } from './employee-detail/employee-history/employee-history.component';
import { UploadContractFileComponent } from './employee-detail/employee-contract/upload-contract-file/upload-contract-file.component';
import { TooltipModule } from 'ngx-bootstrap/tooltip';
import { SalaryChangesComponent } from './employee-detail/salary-changes/salary-changes.component';
import { WorkingHistoryComponent } from './employee-detail/working-history/working-history.component';
import { BranchHistoryComponent } from './employee-detail/branch-history/branch-history.component';
import { PayslipHistoryComponent } from './employee-detail/payslip-history/payslip-history.component';
import { ChangeBranchComponent } from './employee-detail/personal-info/change-branch/change-branch.component';
import { ChangeStatusToQuitComponent } from './employee-detail/personal-info/change-status/change-status-to-quit/change-status-to-quit.component';
import { ChangeStatusToPauseComponent } from './employee-detail/personal-info/change-status/change-status-to-pause/change-status-to-pause.component';
import { ChangeStatusToWorkingComponent } from './employee-detail/personal-info/change-status/change-status-to-working/change-status-to-working.component';
import { ChangeStatusToMaternityLeaveComponent } from './employee-detail/personal-info/change-status/change-status-to-maternity-leave/change-status-to-maternity-leave.component';
import { ExtendMaternityLeaveComponent } from './employee-detail/personal-info/change-status/extend-maternity-leave/extend-maternity-leave.component';
import { ExtendPausingComponent } from './employee-detail/personal-info/change-status/extend-pausing/extend-pausing.component';
import { WorkingHistoryEditNote } from './employee-detail/working-history/working-history-edit-note/working-history-edit-note.component';
import { BranchHistoryEditNoteComponent } from './employee-detail/branch-history/branch-history-edit-note/branch-history-edit-note.component';
import { SalaryChangeEditNoteComponent } from './employee-detail/salary-changes/salary-change-edit-note/salary-change-edit-note.component';
import { SalaryChangesEditDialogComponent } from './employee-detail/salary-changes/salary-changes-edit-dialog/salary-changes-edit-dialog.component';
import { CreateEmployeeFromFileComponent } from './employee-list/create-employee-from-file/create-employee-from-file.component';
import { UpdateEmployeeFromFileComponent } from './employee-list/update-employee-from-file/update-employee-from-file.component';
import { EditEmployeeContractComponent } from './employee-detail/employee-contract/edit-employee-contract/edit-employee-contract.component';
import { ExportEmployeeDialogComponent } from './employee-list/export-employee-dialog/export-employee-dialog.component';
import { ExportContractComponent } from './employee-detail/employee-contract/export-contract/export-contract.component';
import { ConfirmChangeStatusDialogComponent } from './employee-detail/personal-info/change-status/confirm-change-status-dialog/confirm-change-status-dialog.component';
import { WorkingHistoryEditDateComponent } from './employee-detail/working-history/working-history-edit-date/working-history-edit-date.component';
@NgModule({
  declarations: [
    EmployeeListComponent,
    EmployeeDetailComponent,
    PersonalInfoComponent,
    EmployeeContractComponent,
    EmployeeDebtComponent,
    EmployeeBenefitComponent,
    EmployeeBonusComponent,
    EmployeePunishmentComponent,
    UploadAvatarComponent,
    EmployeeContractEditNoteComponent,
    EmployeeHistoryComponent,
    UploadContractFileComponent,
    ChangeBranchComponent,
    ChangeStatusToQuitComponent,
    ChangeStatusToPauseComponent,
    SalaryChangesComponent,
    WorkingHistoryComponent,
    BranchHistoryComponent,
    PayslipHistoryComponent,
    ChangeStatusToWorkingComponent,
    ChangeStatusToMaternityLeaveComponent,
    ExtendMaternityLeaveComponent,
    ExtendPausingComponent,
    WorkingHistoryEditNote,
    BranchHistoryEditNoteComponent,
    SalaryChangeEditNoteComponent,
    SalaryChangesEditDialogComponent,
    CreateEmployeeFromFileComponent,
    UpdateEmployeeFromFileComponent,
    EditEmployeeContractComponent,
    ExportEmployeeDialogComponent,
    ExportContractComponent,
    ConfirmChangeStatusDialogComponent,
    WorkingHistoryEditDateComponent
  ],
  imports: [
    FormsModule,
    ReactiveFormsModule,
    CommonModule,
    SharedModule,
    EmployeeRoutingModule,
    ImageCropperModule,
    TooltipModule.forRoot()
  ],
})
export class EmployeesModule { }
