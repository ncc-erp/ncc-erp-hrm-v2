import { AppRouteGuard } from './../../../shared/auth/auth-route-guard';
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ListBenefitComponent } from './list-benefit/list-benefit.component';
import { BenefitInfomationComponent } from './benefit-detail/benefit-infomation/benefit-infomation.component';
import { BenefitEmployeeComponent } from './benefit-detail/benefit-employee/benefit-employee.component';
import { BenefitDetailComponent } from './benefit-detail/benefit-detail.component';
const routes: Routes = [
  {
    path: "list-benefit",
    component: ListBenefitComponent,
    canActivate: [AppRouteGuard],
  },
  {
    path: "list-benefit/benefit-detail",
    component: BenefitDetailComponent,
    canActivate: [AppRouteGuard],
    children: [
      {
        path: "benefit-employee",
        component: BenefitEmployeeComponent,
        canActivate: [AppRouteGuard],
      },
      {
        path: "benefit-infomation",
        component: BenefitInfomationComponent,
        canActivate: [AppRouteGuard],
      },
    ]
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class BenefitRoutingModule { }