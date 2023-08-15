import { ChangePasswordComponent } from './users/change-password/change-password.component';
import { ConfigurationComponent } from './configuration/configuration.component';
import { AppRouteGuard } from "./../../../shared/auth/auth-route-guard";
import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";
import { RolesComponent } from "./roles/roles.component";
import { TenantsComponent } from "./tenants/tenants.component";
import { UsersComponent } from "./users/users.component";
import { EditRoleDialogComponent } from "./roles/edit-role/edit-role-dialog.component";
import { EmailTemplatesComponent } from './email-templates/email-templates.component';
import { BackgroundJobsComponent } from './background-jobs/background-jobs.component';
import { AuditLogsComponent } from './audit-logs/audit-logs.component';

const routes: Routes = [
  {
    path: "users",
    component: UsersComponent,
    canActivate: [AppRouteGuard],
  },
  {
    path: "update-password",
    component: ChangePasswordComponent,
    canActivate: [AppRouteGuard],
  },
  {
    path: "roles",
    component: RolesComponent,
    canActivate: [AppRouteGuard],
  },
  {
    path: "tenants",
    component: TenantsComponent,
    canActivate: [AppRouteGuard],
  },
  {
    path: "roles/edit-role",
    component: EditRoleDialogComponent,
    canActivate: [AppRouteGuard]
  },
  {
    path: "configurations",
    component: ConfigurationComponent,
    canActivate: [AppRouteGuard],
  },
  {
    path: "email-templates",
    component: EmailTemplatesComponent,
    canActivate: [AppRouteGuard],
  },
  {
    path: "background-jobs",
    component: BackgroundJobsComponent,
    canActivate: [AppRouteGuard],
  },
  {
    path: "audit-logs",
    component: AuditLogsComponent,
    canActivate: [AppRouteGuard],
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class AdminRoutingModule { }
