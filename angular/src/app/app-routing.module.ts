import { TestComponentComponent } from './modules/test-component/test-component.component';
import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { AppComponent } from './app.component';
import { AppRouteGuard } from '@shared/auth/auth-route-guard';
import { HomeComponent } from './home/home.component';
import { AboutComponent } from './about/about.component';
import { PayslipDetailPreviewLinkComponent } from './modules/payslip-detail-preview-link/payslip-detail-preview-link.component';
import { NotHaveAccessComponent } from './modules/not-have-access/not-have-access.component';
import { PERMISSIONS_CONSTANT } from './permission/permission';
import { PayslipToConfirmMailOrComplainMailComponent } from './modules/payslip-to-confirm-mail-or-complain-mail/payslip-to-confirm-mail-or-complain-mail.component';

@NgModule({
    imports: [
        RouterModule.forChild([
            {
                path: '',
                component: AppComponent,
                children: [
                    { path: 'home', component: HomeComponent, 
                        data: {
                        permission: PERMISSIONS_CONSTANT.Home,
                        preload: true,
                    }, canActivate: [AppRouteGuard] },
                    { path: 'test', component: TestComponentComponent, canActivate: [AppRouteGuard] },
                    {
                        path: 'admin',
                        loadChildren: () => import('app/modules/admin/admin.module').then(m => m.AdminModule),
                        data: {
                            permission: PERMISSIONS_CONSTANT.Admin,
                            preload: true
                        },
                        canActivate: [AppRouteGuard],
                    },
                    {
                        path: 'categories',
                        loadChildren: () => import('app/modules/categories/categories.module').then(m => m.CategoriesModule),
                        data: {
                            permission: PERMISSIONS_CONSTANT.Category,
                            preload: true
                        },
                        canActivate: [AppRouteGuard],
                    },
                    {
                        path: 'employees',
                        loadChildren: () => import('app/modules/employees/employees.module').then(m => m.EmployeesModule),
                        data: {
                            permission: PERMISSIONS_CONSTANT.Employee,
                            preload: true
                        },
                        canActivate: [AppRouteGuard],
                    },

                    {
                        path: 'debt',
                        loadChildren: () => import('app/modules/debt/debt.module').then(m => m.DebtModule),
                        canActivate: [AppRouteGuard],
                        data: {
                            permission: PERMISSIONS_CONSTANT.Debt,
                            preload: true
                        },
                    },
                    {
                        path: 'refunds',
                        loadChildren: () => import('app/modules/refunds/refunds.module').then(m => m.RefundsModule),
                        canActivate: [AppRouteGuard],
                        data: {
                            permission: PERMISSIONS_CONSTANT.Refund,
                            preload: true
                        },
                    },
                    {
                        path: 'punishments',
                        loadChildren: () => import('app/modules/punishments/punishments.module').then(m => m.PunishmentsModule),
                        data: {
                            permission: PERMISSIONS_CONSTANT.Punishment,
                            preload: true
                        },
                        canActivate: [AppRouteGuard],
                    },
                    {
                        path: 'benefits',
                        loadChildren: () => import('app/modules/benefits/benefits.module').then(m => m.BenefitsModule),
                        data: {
                            permission: PERMISSIONS_CONSTANT.Benefit,
                            preload: true
                        },
                        canActivate: [AppRouteGuard],
                    },
                    {
                        path: 'bonuses',
                        loadChildren: () => import('app/modules/bonuses/bonuses.module').then(m => m.BonusesModule),
                        data: {
                            permission: PERMISSIONS_CONSTANT.Bonus,
                            preload: true
                        },
                        canActivate: [AppRouteGuard],
                    },
                    {
                        path: 'payroll',
                        loadChildren: () => import('app/modules/salaries/salaries.module').then(m =>m.SalariesModule),
                        data: {
                            permission: PERMISSIONS_CONSTANT.Payroll,
                            preload: true
                        },
                        canActivate: [AppRouteGuard],
                    },
                    {
                        path: "salary-change-requests",
                        loadChildren: () => import('app/modules/salary-change-requests/salary-change-requests.module').then(m => m.SalaryChangeRequestsModule),
                        data: {
                            permission: PERMISSIONS_CONSTANT.SalaryChangeRequest,
                            preload: true
                        },
                        canActivate: [AppRouteGuard]
                    },
                    {
                        path: "warning-for-employee",
                        loadChildren: () => import('app/modules/warning-for-employee-update/warning-for-employee-update.module').then(m => m.BackToWorkModule),
                        data: {
                            permission: PERMISSIONS_CONSTANT.WarningEmployee,
                            preload: true
                        },
                        canActivate: [AppRouteGuard]
                    },
                    {
                        path: "punishment-funds",
                        loadChildren: ()=> import('app/modules/punishment-funds/punishment-funds.module').then(m=> m.PunishmentFundsModule),
                        data: {
                            permission: PERMISSIONS_CONSTANT.Punishment,
                            preload: true
                        },
                        canActivate: [AppRouteGuard]
                    },
                    {
                        path: "not-have-access",
                        component : NotHaveAccessComponent,
                        canActivate: [AppRouteGuard]
                    }, 
                    { path: 'payslip-confirm', component: PayslipDetailPreviewLinkComponent,
                        data: {
                            permission: PERMISSIONS_CONSTANT.ViewMyPayslipLink,
                            preload: true,
                        
                        },
                         canActivate: [AppRouteGuard] 
                    },
                    { path: 'confirm-mail', component: PayslipToConfirmMailOrComplainMailComponent,
                        data: {
                            permission: PERMISSIONS_CONSTANT.ViewMyPayslipLink,
                            preload: true,
                        
                        },
                         canActivate: [AppRouteGuard] 
                    },
                    { path: 'complain-mail', component: PayslipToConfirmMailOrComplainMailComponent,
                        data: {
                            permission: PERMISSIONS_CONSTANT.ViewMyPayslipLink,
                            preload: true,
                        
                        },
                         canActivate: [AppRouteGuard] 
                    },
                ]
            },
        ])
    ],
    exports: [RouterModule]
})
export class AppRoutingModule { }
