import { PlanToQuitComponent } from './plan-to-quit/plan-to-quit.component';
import { TempEmployeeTalentComponent } from './temp-employee-talent/temp-employee-talent.component';
import { Routes } from '@angular/router';
import { AppRouteGuard } from '../../../shared/auth/auth-route-guard';
import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { BackToWorkComponent} from './back-to-work/back-to-work.component'
import { UpdateContractComponent } from './update-contract/update-contract.component';
import { TempEmployeeTsComponent } from './temp-employee-ts/temp-employee-ts.component';
const routes: Routes = [
    {
      path: "back-to-work",
      component:  BackToWorkComponent,
      canActivate: [AppRouteGuard],
    },
    {
      path: "update-contract",
      component: UpdateContractComponent,
      canActivate: [AppRouteGuard]
    },
    {
      path: "temp-employee-talent",
      component: TempEmployeeTalentComponent,
      canActivate: [AppRouteGuard]
    },
    {
      path: "request-change-info",
      component: TempEmployeeTsComponent,
      canActivate: [AppRouteGuard]
    },
    {
      path: "plan-quit-employee",
      component: PlanToQuitComponent,
      canActivate: [AppRouteGuard]
    }
  ];

  @NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule],
  })
  export class BackToWorkRoutingModule { }
