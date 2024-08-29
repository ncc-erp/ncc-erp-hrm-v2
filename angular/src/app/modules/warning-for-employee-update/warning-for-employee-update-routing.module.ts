import { PlanToQuitComponent } from './plan-to-quit/plan-to-quit.component';
import { TempEmployeeTalentComponent } from './temp-employee-talent/temp-employee-talent.component';
import { Routes } from '@angular/router';
import { AppRouteGuard } from '../../../shared/auth/auth-route-guard';
import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { BackToWorkComponent} from './back-to-work/back-to-work.component'
import { UpdateContractComponent } from './update-contract/update-contract.component';
import { TempEmployeeTsComponent } from './temp-employee-ts/temp-employee-ts.component';
import { PERMISSIONS_CONSTANT } from '@app/permission/permission';
const routes: Routes = [
    {
      path: "back-to-work",
      component:  BackToWorkComponent,
      data:{
        permisson:PERMISSIONS_CONSTANT.WarningEmployee_BackToWork_View,
        preload:true
      },
      canActivate: [AppRouteGuard],
    },
    {
      path: "update-contract",
      component: UpdateContractComponent,
      data:{
        permisson:PERMISSIONS_CONSTANT.WarningEmployee_ContractExpired_View,
        preload:true
      },
      canActivate: [AppRouteGuard]
    },
    {
      path: "temp-employee-talent",
      component: TempEmployeeTalentComponent,
      data:{
        permission: PERMISSIONS_CONSTANT.WarningEmployee_TempEmployeeTS,
        preload:true
      },
      canActivate: [AppRouteGuard]
    },
    {
      path: "request-change-info",
      component: TempEmployeeTsComponent,
      data:{
        permission: PERMISSIONS_CONSTANT.WarningEmployee_RequestChangeInfo_DetailRequest_View,
        preload:true
      },
      canActivate: [AppRouteGuard]
    },
    {
      path: "plan-quit-employee",
      component: PlanToQuitComponent,
      data:{
        permission: PERMISSIONS_CONSTANT.WarningEmployee_PlanQuitEmployee_View,
        preload:true
      },
      canActivate: [AppRouteGuard]
    }
  ];

  @NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule],
  })
  export class BackToWorkRoutingModule { }
