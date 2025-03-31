

import { AppRouteGuard } from './../../../shared/auth/auth-route-guard';
import { RouterModule, Routes } from '@angular/router';
import { NgModule } from "@angular/core";
import { PERMISSIONS_CONSTANT } from '@app/permission/permission';
import { PayrollTokenComponent } from './payroll-token/payroll-token.component';
import { PayslipTokenComponent } from './payslip-token/payslip-token.component';

const routes: Routes = [
    {
        path: "list-payroll-token",
        component: PayrollTokenComponent,
        data: {
            permission: PERMISSIONS_CONSTANT.Payrol_View,
            preload: true
        },
        canActivate: [AppRouteGuard],
    },
    {
      path:"list-payroll-token/list-detail",
      component:PayslipTokenComponent,
      data: {   
        permission: PERMISSIONS_CONSTANT.Payrol_View,
        preload: true
      },

    }
];
@NgModule({ 
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class PayrollTokenRoutingModule { }