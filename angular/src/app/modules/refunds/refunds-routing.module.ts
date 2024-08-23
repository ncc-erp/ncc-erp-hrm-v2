import { RefundDetailComponent } from './refund-detail/refund-detail.component';
import { ListRefundComponent } from './list-refund/list-refund.component';
import { AppRouteGuard } from './../../../shared/auth/auth-route-guard';
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { PERMISSIONS_CONSTANT } from '@app/permission/permission';

const routes: Routes = [
  {
    path: "list-refund",
    component: ListRefundComponent,
    data:{
      permission:PERMISSIONS_CONSTANT.Refund_View,
      preload:true
    },
    canActivate: [AppRouteGuard],
  },
  {
    path: "refund-detail",
    component: RefundDetailComponent,
    data:{
      permisson: PERMISSIONS_CONSTANT.Refund_RefundDetail_View,
      preload:true
    },
    canActivate: [AppRouteGuard],
  }

];
@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class RefundsRoutingModule { }
