import { PERMISSIONS_CONSTANT } from './../../permission/permission';
import { PayslipDetailPreviewLinkComponent } from './payslip-detail-preview-link.component';
import { AppRouteGuard } from './../../../shared/auth/auth-route-guard';
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

const routes: Routes = [
    {
      path: "payslip-detail/:id",
      component: PayslipDetailPreviewLinkComponent,
      data: {
        permission: [PERMISSIONS_CONSTANT.ViewMyPayslipLink,PERMISSIONS_CONSTANT.ViewAllPayslipLink],
        preload: true
      },
      canActivate: [AppRouteGuard],
  
    }
    
  ];
  
  @NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule],
  })
  export class PayslipDetailPreviewLinkRoutingModule {
    constructor() {
     
    }
   }