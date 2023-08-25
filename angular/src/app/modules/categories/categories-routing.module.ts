import { Routes } from '@angular/router';
import { AppRouteGuard } from './../../../shared/auth/auth-route-guard';
import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { BranchComponent } from './branch/branch.component';
import { JobPositionsComponent } from './job-positions/job-positions.component';
import { LevelsComponent } from './levels/levels.component';
import { PunishmentTypesComponent } from './punishment-types/punishment-types.component';
import { SkillsComponent } from './skills/skills.component';
import { TeamsComponent } from './teams/teams.component';
import { UserTypesComponent } from './user-types/user-types.component';
import { BanksComponent } from './banks/banks.component';
import { IssuedByComponent } from './issued-by/issued-by.component';
const routes: Routes = [
    {
      path: "branches",
      component: BranchComponent,
      canActivate: [AppRouteGuard],
    },
    {
      path: "job-positions",
      component: JobPositionsComponent,
      canActivate: [AppRouteGuard],
    },
    {
      path: "levels",
      component: LevelsComponent,
      canActivate: [AppRouteGuard],
    },
    {
      path: "punishment-type",
      component: PunishmentTypesComponent,
      canActivate: [AppRouteGuard],
    },
    {
      path: "skills",
      component: SkillsComponent,
      canActivate: [AppRouteGuard],
    },
    {
      path: "teams",
      component: TeamsComponent,
      canActivate: [AppRouteGuard],
    },
    {
      path: "user-types",
      component: UserTypesComponent,
      canActivate: [AppRouteGuard],
    },
    {
      path: "banks",
      component: BanksComponent,
      canActivate: [AppRouteGuard],
    },
    {
      path: "issued-by",
      component: IssuedByComponent,
      canActivate: [AppRouteGuard],
    },
  ];
  
  @NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule],
  })
  export class CategoriesRoutingModule { }
  