import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";
import { AppRouteGuard } from "../../../shared/auth/auth-route-guard";
import { CreateEditViewDebtComponent } from "./create-edit-view-debt/create-edit-view-debt.component";
import { DebtListComponent } from "./debt-list/debt-list.component";
import { PERMISSIONS_CONSTANT } from "@app/permission/permission";


const routes: Routes = [
    {
        path: 'list-debt',
        component: DebtListComponent,
        data: {
            permission: PERMISSIONS_CONSTANT.Debt_View,
            preload: true
          },
        canActivate: [AppRouteGuard]
    },
    {
        path: 'list-debt/create',
        component: CreateEditViewDebtComponent,
        data: {
            permission: PERMISSIONS_CONSTANT.Debt_Create,
            preload: true
          },
        canActivate: [AppRouteGuard]
    },
    {
        path: 'list-debt/detail/:id',
        component: CreateEditViewDebtComponent,
        data: {
            permission: PERMISSIONS_CONSTANT.Debt_DebtDetail,
            preload: true
          },
        canActivate: [AppRouteGuard]
    },
]

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class DebtRoutingModule{}