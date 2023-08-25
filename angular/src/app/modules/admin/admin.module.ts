import { AdminRoutingModule } from './admin-routing.module';
import { SharedModule } from './../../../shared/shared.module';
import { ChangePasswordComponent } from './users/change-password/change-password.component';
import { UsersComponent } from './users/users.component';
import { EditRoleDialogComponent } from './roles/edit-role/edit-role-dialog.component';
import { RolesComponent } from './roles/roles.component';
import { TenantsComponent } from './tenants/tenants.component';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CreateTenantDialogComponent } from './tenants/create-tenant/create-tenant-dialog.component';
import { EditTenantDialogComponent } from './tenants/edit-tenant/edit-tenant-dialog.component';
import { CreateRoleDialogComponent } from './roles/create-role/create-role-dialog.component';
import { CreateUserDialogComponent } from './users/create-user/create-user-dialog.component';
import { EditUserDialogComponent } from './users/edit-user/edit-user-dialog.component';
import { ResetPasswordDialogComponent } from './users/reset-password/reset-password.component';
import { AddUserInRoleComponent } from './roles/edit-role/add-user-in-role/add-user-in-role.component';
import { ConfigurationComponent } from './configuration/configuration.component';
import { EmailTemplatesComponent } from './email-templates/email-templates.component';
import { MailDialogComponent } from './email-templates/mail-dialog/mail-dialog.component';
import { EditEmailDialogComponent } from './email-templates/edit-email-dialog/edit-email-dialog.component';
import { EditUserRoleComponent } from './users/edit-user-role/edit-user-role.component';
import { BackgroundJobsComponent } from './background-jobs/background-jobs.component';
import { AuditLogsComponent } from './audit-logs/audit-logs.component';
import { RetryBackgroundJobComponent } from './background-jobs/retry-background-job/retry-background-job.component';


@NgModule({
  declarations: [
    TenantsComponent,
    CreateTenantDialogComponent,
    EditTenantDialogComponent,
    // roles
    RolesComponent,
    CreateRoleDialogComponent,
    EditRoleDialogComponent,
    // users
    UsersComponent,
    CreateUserDialogComponent,
    EditUserDialogComponent,
    ChangePasswordComponent,
    ResetPasswordDialogComponent,
    AddUserInRoleComponent,
    ConfigurationComponent,
    EmailTemplatesComponent,
    MailDialogComponent,
    EditEmailDialogComponent,
    EditUserRoleComponent,
    BackgroundJobsComponent,
    AuditLogsComponent,
    RetryBackgroundJobComponent
  ],
  imports: [
    CommonModule,
    SharedModule,
    AdminRoutingModule
  ]
})
export class AdminModule { }
