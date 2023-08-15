import { TestComponentComponent } from './modules/test-component/test-component.component';
import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { AppComponent } from './app.component';
import { AppRouteGuard } from '@shared/auth/auth-route-guard';
import { HomeComponent } from './home/home.component';
import { AboutComponent } from './about/about.component';

@NgModule({
    imports: [
        RouterModule.forChild([
            {
                path: '',
                component: AppComponent,
                children: [
                    { path: 'home', component: HomeComponent, canActivate: [AppRouteGuard] },
                    { path: 'test', component: TestComponentComponent, canActivate: [AppRouteGuard] },
                    {
                        path: 'admin',
                        loadChildren: () => import('app/modules/admin/admin.module').then(m => m.AdminModule),
                        data: {
                            permission: '',
                            preload: true
                        },
                        canActivate: [AppRouteGuard],
                    },
                    {
                        path: 'categories',
                        loadChildren: () => import('app/modules/categories/categories.module').then(m => m.CategoriesModule),
                        data: {
                            permission: '',
                            preload: true
                        },
                        canActivate: [AppRouteGuard],
                    },
                    {
                        path: 'employees',
                        loadChildren: () => import('app/modules/employees/employees.module').then(m => m.EmployeesModule),
                        data: {
                            permission: '',
                            preload: true
                        },
                        canActivate: [AppRouteGuard],
                    },

                    {
                        path: 'debt',
                        loadChildren: () => import('app/modules/debt/debt.module').then(m => m.DebtModule),
                        canActivate: [AppRouteGuard],
                        data: {
                            permission: '',
                            preload: true
                        },
                    },
                    {
                        path: 'refunds',
                        loadChildren: () => import('app/modules/refunds/refunds.module').then(m => m.RefundsModule),
                        canActivate: [AppRouteGuard],
                        data: {
                            permission: '',
                            preload: true
                        },
                    },
                    {
                        path: 'punishments',
                        loadChildren: () => import('app/modules/punishments/punishments.module').then(m => m.PunishmentsModule),
                        data: {
                            permission: '',
                            preload: true
                        },
                        canActivate: [AppRouteGuard],
                    },
                    {
                        path: 'benefits',
                        loadChildren: () => import('app/modules/benefits/benefits.module').then(m => m.BenefitsModule),
                        data: {
                            permission: '',
                            preload: true
                        },
                        canActivate: [AppRouteGuard],
                    },
                    {
                        path: 'bonuses',
                        loadChildren: () => import('app/modules/bonuses/bonuses.module').then(m => m.BonusesModule),
                        data: {
                            permission: '',
                            preload: true
                        },
                        canActivate: [AppRouteGuard],
                    },
                    {
                        path: 'payroll',
                        loadChildren: () => import('app/modules/salaries/salaries.module').then(m =>m.SalariesModule),
                        data: {
                            permission: '',
                            preload: true
                        },
                        canActivate: [AppRouteGuard],
                    },





                    {
                        path: "salary-change-requests",
                        loadChildren: () => import('app/modules/salary-change-requests/salary-change-requests.module').then(m => m.SalaryChangeRequestsModule),
                        data: {
                            permission: '',
                            preload: true
                        },
                        canActivate: [AppRouteGuard]
                    },
                    {
                        path: "warning-for-employee",
                        loadChildren: () => import('app/modules/warning-for-employee-update/warning-for-employee-update.module').then(m => m.BackToWorkModule),
                        data: {
                            permission: '',
                            preload: true
                        },
                        canActivate: [AppRouteGuard]
                    },
                    {
                        path: "punishment-funds",
                        loadChildren: ()=> import('app/modules/punishment-funds/punishment-funds.module').then(m=> m.PunishmentFundsModule),
                        data: {
                            permission: '',
                            preload: true
                        },
                        canActivate: [AppRouteGuard]
                    }
                ]
            },
        ])
    ],
    exports: [RouterModule]
})
export class AppRoutingModule { }
