import { AppRouteGuard } from './../../../shared/auth/auth-route-guard';
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { BonusListComponent } from './bonus-list/bonus-list.component';
import { BonusDetailComponent } from './bonus-detail/bonus-detail.component';
import { BonusEmployeeComponent } from './bonus-detail/bonus-employee/bonus-employee.component';
import { BonusInformationComponent } from './bonus-detail/bonus-information/bonus-information.component';
const routes: Routes = [
  {
    path: "list-bonus",
    component: BonusListComponent,
    canActivate: [AppRouteGuard],
  },
  {
    path: "list-bonus/bonus-detail",
    component: BonusDetailComponent,
    canActivate: [AppRouteGuard],
    children: [
      {
        path: "bonus-information",
        component: BonusInformationComponent,
        canActivate: [AppRouteGuard],
      },
      {
        path: "bonus-employee",
        component: BonusEmployeeComponent,
        canActivate: [AppRouteGuard],
      },
    ],
  }
];
@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class BonusesRoutingModule { }
