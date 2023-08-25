import { SharedModule } from './../../../shared/shared.module';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BranchComponent } from './branch/branch.component';
import { CategoriesRoutingModule } from './categories-routing.module';
import { JobPositionsComponent } from './job-positions/job-positions.component';
import { SkillsComponent } from './skills/skills.component';
import { TeamsComponent } from './teams/teams.component';
import { LevelsComponent } from './levels/levels.component';
import { UserTypesComponent } from './user-types/user-types.component';
import { PunishmentTypesComponent } from './punishment-types/punishment-types.component';
import { BanksComponent } from './banks/banks.component';
import { CreateEditBranchDialogComponent } from './branch/create-edit-branch-dialog/create-edit-branch-dialog.component';
import { CreateEditUserTypeDialogComponent } from './user-types/create-edit-user-type-dialog/create-edit-user-type-dialog.component';
import { CreateEditJobPositionDialogComponent } from './job-positions/create-edit-job-position-dialog/create-edit-job-position-dialog.component';
import { CreateEditLevelDialogComponent } from './levels/create-edit-level-dialog/create-edit-level-dialog.component';
import { CreateEditTeamDialogComponent } from './teams/create-edit-team-dialog/create-edit-team-dialog.component';
import { CreateEditSkillDialogComponent } from './skills/create-edit-skill-dialog/create-edit-skill-dialog.component';
import { CreateEditPunishmentTypeDialogComponent } from './punishment-types/create-edit-punishment-type-dialog/create-edit-punishment-type-dialog.component';
import { CreateEditBankDialogComponent } from './banks/create-edit-bank-dialog/create-edit-bank-dialog.component';
import { IssuedByComponent } from './issued-by/issued-by.component';
import { CreateEditIssuedByDialogComponent } from './issued-by/create-edit-issued-by-dialog/create-edit-issued-by-dialog.component';



@NgModule({
  declarations: [
    BranchComponent,
    JobPositionsComponent,
    SkillsComponent,
    TeamsComponent,
    LevelsComponent,
    UserTypesComponent,
    PunishmentTypesComponent,
    BanksComponent,
    CreateEditBranchDialogComponent,
    CreateEditUserTypeDialogComponent,
    CreateEditJobPositionDialogComponent,
    CreateEditLevelDialogComponent,
    CreateEditTeamDialogComponent,
    CreateEditSkillDialogComponent,
    CreateEditPunishmentTypeDialogComponent,
    CreateEditBankDialogComponent,
    IssuedByComponent,
    CreateEditIssuedByDialogComponent
  ],
  imports: [
    CommonModule,
    CategoriesRoutingModule,
    SharedModule
  ]
})
export class CategoriesModule { }
