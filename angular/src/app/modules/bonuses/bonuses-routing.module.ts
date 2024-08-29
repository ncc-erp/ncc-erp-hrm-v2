import { AppRouteGuard } from './../../../shared/auth/auth-route-guard';
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { BonusListComponent } from './bonus-list/bonus-list.component';
import { BonusDetailComponent } from './bonus-detail/bonus-detail.component';
import { BonusEmployeeComponent } from './bonus-detail/bonus-employee/bonus-employee.component';
import { BonusInformationComponent } from './bonus-detail/bonus-information/bonus-information.component';
import { PERMISSIONS_CONSTANT } from '@app/permission/permission';
const routes: Routes = [
  {
    path: "list-bonus",
    component: BonusListComponent,
    data: {
      permission: PERMISSIONS_CONSTANT.Bonus_View,
      preload: true
    },
    canActivate: [AppRouteGuard],
  },
  {
    path: "list-bonus/bonus-detail",
    component: BonusDetailComponent,
    data: {
      permission: PERMISSIONS_CONSTANT.Bonus_BonusDetail,
      preload: true
    },
    canActivate: [AppRouteGuard],
    children: [
      {
        path: "bonus-information",
        component: BonusInformationComponent,
        data: {
          permission: PERMISSIONS_CONSTANT.Bonus_BonusDetail_TabInformation,
          preload: true
        },
        canActivate: [AppRouteGuard],
      },
      {
        path: "bonus-employee",
        component: BonusEmployeeComponent,
        data: {
          permission: PERMISSIONS_CONSTANT.Bonus_BonusDetail_TabEmployee,
          preload: true
        },
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
