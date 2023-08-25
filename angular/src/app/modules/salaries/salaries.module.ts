import { popperVariation, TippyModule, tooltipVariation } from '@ngneat/helipopper';
import { SharedModule } from './../../../shared/shared.module';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SalariesRoutingModule } from './salaries-routing.module';
import { AddEditPayRollComponent } from './pay-roll/add-edit-pay-roll/add-edit-pay-roll.component';
import { PayslipComponent } from './payslip/payslip.component';
import { PayslipDetailComponent } from './payslip-detail/payslip-detail.component';
import { PayslipDetailSalaryComponent } from './payslip-detail/payslip-detail-salary/payslip-detail-salary.component';
import { PayslipDetailBenefitComponent } from './payslip-detail/payslip-detail-benefit/payslip-detail-benefit.component';
import { PayslipDetailBonusComponent } from './payslip-detail/payslip-detail-bonus/payslip-detail-bonus.component';
import { PayslipDetailDebtComponent } from './payslip-detail/payslip-detail-debt/payslip-detail-debt.component';
import { PayslipDetailPunishmentComponent } from './payslip-detail/payslip-detail-punishment/payslip-detail-punishment.component';
import { PayslipDetailPreviewComponent } from './payslip-detail/payslip-detail-preview/payslip-detail-preview.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { CalculateResultDialogComponent } from './payslip/calculate-result-dialog/calculate-result-dialog.component';
import { ConfirmMailDialogComponent } from './payslip/confirm-mail-dialog/confirm-mail-dialog.component';
import { UpdatePayslipDeadlineDialogComponent } from './payslip-detail/payslip-detail-preview/update-payslip-deadline-dialog/update-payslip-deadline-dialog.component';
import { TippyProps } from '@ngneat/helipopper/lib/tippy.types';
import { CalculateResultComponent } from './payslip-detail/calculate-result/calculate-result.component';
import { ImportEmployeeRemainLeaveDaysAfterCalculatingSalaryComponent } from './payslip/import-employee-remain-leave-days-after-calculating-salary/import-employee-remain-leave-days-after-calculating-salary.component';
import { EditPayslipDetailDialogComponent } from './payslip-detail/payslip-detail-salary/edit-payslip-detail-dialog/edit-payslip-detail-dialog.component';
import { PenaltyUserDialogComponent } from './payslip/penalty-user-dialog/penalty-user-dialog.component';

export const customeTooltipVariation: Partial<TippyProps> = {
  theme: 'light',
  arrow: true,
  animation: 'scale',
  trigger: 'mouseenter',
  offset: [0, 10],
};

@NgModule({
  declarations: [
    AddEditPayRollComponent,
    PayslipComponent,
    PayslipDetailComponent,
    PayslipDetailSalaryComponent,
    PayslipDetailBenefitComponent,
    PayslipDetailBonusComponent,
    PayslipDetailDebtComponent,
    PayslipDetailPunishmentComponent,
    PayslipDetailPreviewComponent,
    CalculateResultDialogComponent,
    ConfirmMailDialogComponent,
    UpdatePayslipDeadlineDialogComponent,
    CalculateResultComponent,
    ImportEmployeeRemainLeaveDaysAfterCalculatingSalaryComponent,
    EditPayslipDetailDialogComponent,
    PenaltyUserDialogComponent,
  ],
  imports: [
    CommonModule,
    SharedModule,
    SalariesRoutingModule,
    FormsModule,
    ReactiveFormsModule,
    TippyModule.forRoot({
      defaultVariation: 'tooltip',
      variations: {
        tooltip: customeTooltipVariation,
        popper: popperVariation,
      }
    })
  ]
})
export class SalariesModule { }
