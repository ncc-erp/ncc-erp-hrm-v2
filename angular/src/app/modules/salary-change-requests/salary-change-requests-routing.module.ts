import { NgModule } from "@angular/core";
import { RouterModule, Routes } from '@angular/router';
import { AppRouteGuard } from "@shared/auth/auth-route-guard";
import { AddEmployeeToSalaryChangeRequestComponent } from "./salary-change-request-detail/add-employee-to-salary-change-request/add-employee-to-salary-change-request.component";
import { SalaryChangeRequestDetailComponent } from "./salary-change-request-detail/salary-change-request-detail.component";
import { SalaryChangeRequestListComponent } from "./salary-change-request-list/salary-change-request-list.component";

export const routes: Routes = [
    {
        path: "list-request",
        component: SalaryChangeRequestListComponent,
        canActivate: [AppRouteGuard]
    },
    {
        path: "list-request/request-detail/request-employee",
        component: AddEmployeeToSalaryChangeRequestComponent,
        canActivate: [AppRouteGuard] 
    },
    {
        path: "list-request/request-detail",
        component: SalaryChangeRequestDetailComponent,
        canActivate: [AppRouteGuard]
    },
    {
        path:"",
        pathMatch: "full",
        canActivate: [AppRouteGuard],
        redirectTo: "list-request",
    },

]

@NgModule({
    imports: [RouterModule.forChild(routes),],
    exports: [RouterModule]
})
export class SalaryChangeRequestsRoutingModule{
}