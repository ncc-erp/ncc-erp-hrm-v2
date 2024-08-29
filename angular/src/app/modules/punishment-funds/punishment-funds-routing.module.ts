import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Route, RouterModule, Routes } from '@angular/router';
import { ListPunishmentRefundsComponent } from './list-punishment-refunds/list-punishment-refunds.component';
import { AppRouteGuard } from '@shared/auth/auth-route-guard';
import { PERMISSIONS_CONSTANT } from '@app/permission/permission';

const routes:Routes = [
  {
    path: "list-punishment-funds",
    component: ListPunishmentRefundsComponent,
    data:{
      permission: PERMISSIONS_CONSTANT.PunishmentFund_View,
      preload: true
    },
    canActivate: [AppRouteGuard]
  }
]
@NgModule({
  imports: [
    RouterModule.forChild(routes)
  ],
  exports: [RouterModule]
})
export class PunishmentFundsRoutingModule { }
