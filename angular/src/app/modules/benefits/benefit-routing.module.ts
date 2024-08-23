import { AppRouteGuard } from './../../../shared/auth/auth-route-guard';
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ListBenefitComponent } from './list-benefit/list-benefit.component';
import { BenefitInfomationComponent } from './benefit-detail/benefit-infomation/benefit-infomation.component';
import { BenefitEmployeeComponent } from './benefit-detail/benefit-employee/benefit-employee.component';
import { BenefitDetailComponent } from './benefit-detail/benefit-detail.component';
import { PERMISSIONS_CONSTANT } from '@app/permission/permission';
const routes: Routes = [
  {
    path: "list-benefit",
    component: ListBenefitComponent,
    data: {
      permission: PERMISSIONS_CONSTANT.Benefit_View,
      preload: true
    },
    canActivate: [AppRouteGuard],
  },
  {
    path: "list-benefit/benefit-detail",
    component: BenefitDetailComponent,
    data: {
      permission: PERMISSIONS_CONSTANT.Benefit_BenefitDetail,
      preload: true
    },
    canActivate: [AppRouteGuard],
    children: [
      {
        path: "benefit-employee",
        component: BenefitEmployeeComponent,
        data: {
          permission: PERMISSIONS_CONSTANT.Benefit_BenefitDetail_TabEmployee_View,
          preload: true
        },
        canActivate: [AppRouteGuard],
      },
      {
        path: "benefit-infomation",
        component: BenefitInfomationComponent,
        data: {
          permission: PERMISSIONS_CONSTANT.Benefit_BenefitDetail_TabInformation_View,
          preload: true
        },
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