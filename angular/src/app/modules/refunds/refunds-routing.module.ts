import { RefundDetailComponent } from './refund-detail/refund-detail.component';
import { ListRefundComponent } from './list-refund/list-refund.component';
import { AppRouteGuard } from './../../../shared/auth/auth-route-guard';
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

const routes: Routes = [
  {
    path: "list-refund",
    component: ListRefundComponent,
    canActivate: [AppRouteGuard],
  },
  {
    path: "refund-detail",
    component: RefundDetailComponent,
    canActivate: [AppRouteGuard],
  }

];
@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class RefundsRoutingModule { }
