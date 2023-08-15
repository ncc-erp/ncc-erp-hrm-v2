import { AppRouteGuard } from './../../../shared/auth/auth-route-guard';
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { EmployeeListComponent } from './employee-list/employee-list.component';
import { EmployeeDetailComponent } from './employee-detail/employee-detail.component';
import { PersonalInfoComponent } from './employee-detail/personal-info/personal-info.component';
import { EmployeeContractComponent } from './employee-detail/employee-contract/employee-contract.component';
import { EmployeeDebtComponent } from './employee-detail/employee-debt/employee-debt.component';
import { EmployeeBenefitComponent } from './employee-detail/employee-benefit/employee-benefit.component';
import { EmployeeBonusComponent } from './employee-detail/employee-bonus/employee-bonus.component';
import { EmployeePunishmentComponent } from './employee-detail/employee-punishment/employee-punishment.component';
import { SalaryChangesComponent } from './employee-detail/salary-changes/salary-changes.component';
import { WorkingHistoryComponent } from './employee-detail/working-history/working-history.component';
import { BranchHistoryComponent } from './employee-detail/branch-history/branch-history.component';
import { PayslipHistoryComponent } from './employee-detail/payslip-history/payslip-history.component';
const routes: Routes = [
    {
      path:"",
      pathMatch: "full",
      redirectTo: "list-employee"
    },
    {
      path: "list-employee",
      component: EmployeeListComponent,
      canActivate: [AppRouteGuard],
    },
    {
      path: 'create',
      component: EmployeeDetailComponent,
      canActivate: [AppRouteGuard],
      children:[
        {
          path: "",
          pathMatch: "full",
          component: PersonalInfoComponent,
          canActivate: [AppRouteGuard]
        },
        {
          path: '**',
          pathMatch: "full",
          redirectTo: ""
        }
      ]
    },
    {
      path: "list-employee/employee-detail",
      component: EmployeeDetailComponent,
      canActivate: [AppRouteGuard],
      children:[
        {
          path: "personal-info",
          component: PersonalInfoComponent,
          canActivate: [AppRouteGuard]
        },
        {
          path: "contract",
          component: EmployeeContractComponent,
          canActivate: [AppRouteGuard]
        },
        {
          path: "debt",
          component: EmployeeDebtComponent,
          canActivate: [AppRouteGuard]
        },
        {
          path: "benefit",
          component: EmployeeBenefitComponent,
          canActivate: [AppRouteGuard]
        },
        {
          path: "bonus",                                                                                              
          component: EmployeeBonusComponent,
          canActivate: [AppRouteGuard]
        },
        {
          path: "punishment",
          component: EmployeePunishmentComponent,
          canActivate: [AppRouteGuard]
        },
        {
          path: "salary-changes",
          component: SalaryChangesComponent                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                     ,
          canActivate: [AppRouteGuard]
        },
        {
          path: "working-history",
          component: WorkingHistoryComponent                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                     ,
          canActivate: [AppRouteGuard]
        },
        {
          path: "branch-history",
          component: BranchHistoryComponent                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                     ,
          canActivate: [AppRouteGuard]
        },
        {
          path: "payslip-history",
          component: PayslipHistoryComponent                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                     ,
          canActivate: [AppRouteGuard]
        }
      ]
    }
  ];
  
  @NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule],
  })
  export class EmployeeRoutingModule { }