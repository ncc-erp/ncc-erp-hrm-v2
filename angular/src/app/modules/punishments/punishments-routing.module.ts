import { PunishmentsComponent } from './punishmets/punishments.component';
import { Routes, RouterModule } from '@angular/router';
import { AppRouteGuard } from './../../../shared/auth/auth-route-guard';
import { PunishmentDetailComponent } from './punishments-detail/punishment-detail.component';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

const routes: Routes = [
  {
    path: "list-punishments",
    component: PunishmentsComponent,
    canActivate: [AppRouteGuard]
  },
  {
    path: "list-punishments/punishment-detail",
    component: PunishmentDetailComponent,
    canActivate: [AppRouteGuard],
  },

];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class PunishmentsRoutingModule { }
