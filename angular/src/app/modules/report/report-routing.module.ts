
import { AppRouteGuard } from './../../../shared/auth/auth-route-guard';
import { RouterModule, Routes } from '@angular/router';
import { NgModule } from "@angular/core";
import { PERMISSIONS_CONSTANT } from '@app/permission/permission';
import { ReportSalaryComponent } from './report-salary/report-salary.component';
const routes: Routes = [
    {
        path: "list-salary",
        component: ReportSalaryComponent,
        data: {
            permission: PERMISSIONS_CONSTANT.SalaryChangeRequest_View,
            preload: true
        },
        canActivate: [AppRouteGuard],
    }
];
@NgModule({ 
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class ReportRoutingModule { }