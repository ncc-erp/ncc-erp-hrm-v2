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
import { PERMISSIONS_CONSTANT } from '@app/permission/permission';

const routes: Routes = [
  {
    path: "users",
    component: UsersComponent,
    data: {
      permission: PERMISSIONS_CONSTANT.Admin_User_View,
      preload: true
    },
    canActivate: [AppRouteGuard],
  },
  {
    path: "update-password",
    component: ChangePasswordComponent,
    data: {
      permission: PERMISSIONS_CONSTANT.Admin_User_ResetPassword,
      preload: true
    },
    canActivate: [AppRouteGuard],
  },
  {
    path: "roles",
    component: RolesComponent,
    data: {
      permission: PERMISSIONS_CONSTANT.Admin_Role,
      preload: true
    },
    canActivate: [AppRouteGuard],
  },
  {
    path: "tenants",
    component: TenantsComponent,
    data: {
      permission: PERMISSIONS_CONSTANT.Admin_Tenant_View,
      preload: true
    },
    canActivate: [AppRouteGuard],
  },
  {
    path: "roles/edit-role",
    component: EditRoleDialogComponent,
    data: {
      permission: PERMISSIONS_CONSTANT.Admin_Role_Edit,
      preload: true
    },
    canActivate: [AppRouteGuard]
  },
  {
    path: "configurations",
    component: ConfigurationComponent,
    data: {
      permission: PERMISSIONS_CONSTANT.Admin_Configuration,
      preload: true
    },
    canActivate: [AppRouteGuard],
  },
  {
    path: "email-templates",
    component: EmailTemplatesComponent,
    data: {
      permission: PERMISSIONS_CONSTANT.Admin_EmailTemplate,
      preload: true
    },
    canActivate: [AppRouteGuard],
  },
  {
    path: "background-jobs",
    component: BackgroundJobsComponent,
    data: {
      permission: PERMISSIONS_CONSTANT.Admin_BackgroundJob,
      preload: true
    },
    canActivate: [AppRouteGuard],
  },
  {
    path: "audit-logs",
    component: AuditLogsComponent,
    data: {
      permission: PERMISSIONS_CONSTANT.Admin_AuditLog_View,
      preload: true
    },
    canActivate: [AppRouteGuard],
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class AdminRoutingModule { }
