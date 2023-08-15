import { PayslipComponent } from './payslip/payslip.component';
import { AppRouteGuard } from './../../../shared/auth/auth-route-guard';
import { PayRollComponent } from './pay-roll/pay-roll.component';
import { RouterModule, Routes } from '@angular/router';
import { NgModule } from "@angular/core";
import { PayslipDetailComponent } from './payslip-detail/payslip-detail.component';
import { PayslipDetailSalaryComponent } from './payslip-detail/payslip-detail-salary/payslip-detail-salary.component';
import { PayslipDetailBenefitComponent } from './payslip-detail/payslip-detail-benefit/payslip-detail-benefit.component';
import { PayslipDetailBonusComponent } from './payslip-detail/payslip-detail-bonus/payslip-detail-bonus.component';
import { PayslipDetailDebtComponent } from './payslip-detail/payslip-detail-debt/payslip-detail-debt.component';
import { PayslipDetailPunishmentComponent } from './payslip-detail/payslip-detail-punishment/payslip-detail-punishment.component';
import { PayslipDetailPreviewComponent } from './payslip-detail/payslip-detail-preview/payslip-detail-preview.component';
const routes: Routes = [
    {
      path: "list-payroll",
      component: PayRollComponent,
      canActivate: [AppRouteGuard],
    },
    {
      path:"list-payroll/payroll-detail",
      component: PayslipComponent,
      canActivate: [AppRouteGuard],
    },
    {
      path:"list-payroll/payroll-detail/payslip-detail",
      component: PayslipDetailComponent,
      canActivate: [AppRouteGuard],
      children:[
        {
          path: "salary",
          component: PayslipDetailSalaryComponent,
          canActivate: [AppRouteGuard],

        },
        {
          path: "debt",
          component: PayslipDetailDebtComponent,
          canActivate: [AppRouteGuard]
        },
        {
          path: "bonus",
          component: PayslipDetailBonusComponent,
          canActivate: [AppRouteGuard]
        },
        {
          path: "punishment",
          component: PayslipDetailPunishmentComponent,
          canActivate: [AppRouteGuard]
        },
        {
          path: 'benefit',
          component: PayslipDetailBenefitComponent,
          canActivate: [AppRouteGuard]
        },
        {
          path: 'payslip-preview',
          component: PayslipDetailPreviewComponent,
          canActivate: [AppRouteGuard]
        }
      ]
    },
    
   
  
  ];
@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule],
  })
  export class SalariesRoutingModule { }