import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";
import { AppRouteGuard } from "../../../shared/auth/auth-route-guard";
import { CreateEditViewDebtComponent } from "./create-edit-view-debt/create-edit-view-debt.component";
import { DebtListComponent } from "./debt-list/debt-list.component";


const routes: Routes = [
    {
        path: 'list-debt',
        component: DebtListComponent,
        canActivate: [AppRouteGuard]
    },
    {
        path: 'list-debt/create',
        component: CreateEditViewDebtComponent,
        canActivate: [AppRouteGuard]
    },
    {
        path: 'list-debt/detail/:id',
        component: CreateEditViewDebtComponent,
        canActivate: [AppRouteGuard]
    },
]

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class DebtRoutingModule{}