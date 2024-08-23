import { PunishmentsComponent } from './punishmets/punishments.component';
import { Routes, RouterModule } from '@angular/router';
import { AppRouteGuard } from './../../../shared/auth/auth-route-guard';
import { PunishmentDetailComponent } from './punishments-detail/punishment-detail.component';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PERMISSIONS_CONSTANT } from '@app/permission/permission';

const routes: Routes = [
  {
    path: "list-punishments",
    component: PunishmentsComponent,
    data:{
      permission:PERMISSIONS_CONSTANT.Punishment_View,
      preload:true
    },
    canActivate: [AppRouteGuard]
  },
  {
    path: "list-punishments/punishment-detail",
    component: PunishmentDetailComponent,
    data:{
      permission: PERMISSIONS_CONSTANT.Punishment_PunishmentDetail_View,
      preload:true
    },
    canActivate: [AppRouteGuard],
  },

];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class PunishmentsRoutingModule { }
