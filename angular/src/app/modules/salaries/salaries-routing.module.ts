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
import { PERMISSIONS_CONSTANT } from '@app/permission/permission';
const routes: Routes = [
    {
      path: "list-payroll",
      component: PayRollComponent,
      data:{
        permission:PERMISSIONS_CONSTANT.Payrol_View,
        preload:true
      },
      canActivate: [AppRouteGuard],
    },
    {
      path:"list-payroll/payroll-detail",
      component: PayslipComponent,
      data:{
        permission: PERMISSIONS_CONSTANT.Payroll_Payslip_View,
        preload:true
      },
      canActivate: [AppRouteGuard],
    },
    {
      path:"list-payroll/payroll-detail/payslip-detail",
      component: PayslipDetailComponent,
      data:{
        permisson:PERMISSIONS_CONSTANT.Payroll_Payslip_PayslipDetail,
        preload:true
      },
      canActivate: [AppRouteGuard],
      children:[
        {
          path: "salary",
          component: PayslipDetailSalaryComponent,
          data:{
            permission:PERMISSIONS_CONSTANT.Payroll_Payslip_PayslipDetail_TabSalary_View,
            preload:true
          },
          canActivate: [AppRouteGuard],

        },
        {
          path: "debt",
          component: PayslipDetailDebtComponent,
          data:{
            permission:PERMISSIONS_CONSTANT.Payroll_Payslip_PayslipDetail_TabDebt_View,
            preload:true
          },
          canActivate: [AppRouteGuard]
        },
        {
          path: "bonus",
          component: PayslipDetailBonusComponent,
          data:{
            permission: PERMISSIONS_CONSTANT.Payroll_Payslip_PayslipDetail_TabBonus_View,
            preload:true
          },
          canActivate: [AppRouteGuard]
        },
        {
          path: "punishment",
          component: PayslipDetailPunishmentComponent,
          data:{
            permission:PERMISSIONS_CONSTANT.Payroll_Payslip_PayslipDetail_TabPunishment_View,
            preload:true
          },
          canActivate: [AppRouteGuard]
        },
        {
          path: 'benefit',
          component: PayslipDetailBenefitComponent,
          data:{
            permission:PERMISSIONS_CONSTANT.Payroll_Payslip_PayslipDetail_TabBenefit_View,
            preload:true
          },
          canActivate: [AppRouteGuard]
        },
        {
          path: 'payslip-preview',
          component: PayslipDetailPreviewComponent,
          data:{
            permission:PERMISSIONS_CONSTANT.Payroll_Payslip_PayslipDetail_TabPayslipPreview_View,
            preload:true
          },
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