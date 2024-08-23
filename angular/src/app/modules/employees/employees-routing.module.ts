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
import { PERMISSIONS_CONSTANT } from '@app/permission/permission';
const routes: Routes = [
    {
      path:"",
      pathMatch: "full",
      redirectTo: "list-employee"
    },
    {
      path: "list-employee",
      component: EmployeeListComponent,
      data: {
        permission: PERMISSIONS_CONSTANT.Employee_View,
        preload: true
      },
      canActivate: [AppRouteGuard],
    },
    {
      path: 'create',
      component: EmployeeDetailComponent,
      data: {
        permission: PERMISSIONS_CONSTANT.Employee_Create,
        preload: true
      },
      canActivate: [AppRouteGuard],
      children:[
        {
          path: "",
          pathMatch: "full",
          component: PersonalInfoComponent,
          data: {
            permission: PERMISSIONS_CONSTANT.Employee_EmployeeDetail_TabPersonalInfo_View,
            preload: true
          },
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
      data: {
        permission: PERMISSIONS_CONSTANT.Employee_EmployeeDetail,
        preload: true
      },
      canActivate: [AppRouteGuard],
      children:[
        {
          path: "personal-info",
          component: PersonalInfoComponent,
          data: {
            permission: PERMISSIONS_CONSTANT.Employee_EmployeeDetail_TabPersonalInfo_View,
            preload: true
          },
          canActivate: [AppRouteGuard]
        },
        {
          path: "contract",
          component: EmployeeContractComponent,
          data: {
            permission: PERMISSIONS_CONSTANT.Employee_EmployeeDetail_TabContract_View,
            preload: true
          },
          canActivate: [AppRouteGuard]
        },
        {
          path: "debt",
          component: EmployeeDebtComponent,
          data: {
            permission: PERMISSIONS_CONSTANT.Employee_EmployeeDetail_TabDebt_View,
            preload: true
          },
          canActivate: [AppRouteGuard]
        },
        {
          path: "benefit",
          component: EmployeeBenefitComponent,
          data: {
            permission: PERMISSIONS_CONSTANT.Employee_EmployeeDetail_TabBenefit_View,
            preload: true
          },
          canActivate: [AppRouteGuard]
        },
        {
          path: "bonus",                                                                                              
          component: EmployeeBonusComponent,
          data: {
            permission: PERMISSIONS_CONSTANT.Employee_EmployeeDetail_TabBonus_View,
            preload: true
          },
          canActivate: [AppRouteGuard]
        },
        {
          path: "punishment",
          component: EmployeePunishmentComponent,
          data: {
            permission: PERMISSIONS_CONSTANT.Employee_EmployeeDetail_TabPunishment_View,
            preload: true
          },
          canActivate: [AppRouteGuard]
        },
        {
          path: "salary-changes",
          component: SalaryChangesComponent,
          data: {
            permission: PERMISSIONS_CONSTANT.Employee_EmployeeDetail_TabSalaryHistory,
            preload: true
          }                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                ,
          canActivate: [AppRouteGuard]
        },
        {
          path: "working-history",
          component: WorkingHistoryComponent,
          data: {
            permission: PERMISSIONS_CONSTANT.Employee_EmployeeDetail_TabWorkingHistory_View,
            preload: true
          }                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                 ,
          canActivate: [AppRouteGuard]
        },
        {
          path: "branch-history",
          component: BranchHistoryComponent,
          data: {
            permission: PERMISSIONS_CONSTANT.Employee_EmployeeDetail_TabBranchHistory_View,
            preload: true
          }                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                   ,
          canActivate: [AppRouteGuard]
        },
        {
          path: "payslip-history",
          component: PayslipHistoryComponent,
          data: {
            permission: PERMISSIONS_CONSTANT.Employee_EmployeeDetail_TabPayslipHistory_View,
            preload: true
          }                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                 ,
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