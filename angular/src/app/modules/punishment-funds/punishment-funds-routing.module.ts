import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Route, RouterModule, Routes } from '@angular/router';
import { ListPunishmentRefundsComponent } from './list-punishment-refunds/list-punishment-refunds.component';
import { AppRouteGuard } from '@shared/auth/auth-route-guard';

const routes:Routes = [
  {
    path: "list-punishment-funds",
    component: ListPunishmentRefundsComponent,
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
