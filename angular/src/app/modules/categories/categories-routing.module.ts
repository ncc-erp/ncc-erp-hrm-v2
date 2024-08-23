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
import { ChartsComponent } from './charts/charts.component';
import { ChartDetailsComponent } from './charts/chart-details/chart-details.component';
import { PERMISSIONS_CONSTANT } from '@app/permission/permission';
const routes: Routes = [
    {
      path: "branches",
      component: BranchComponent,
      data: {
        permission: PERMISSIONS_CONSTANT.Category_Branch_View,
        preload: true
      },
      canActivate: [AppRouteGuard],
    },
    {
      path: "job-positions",
      component: JobPositionsComponent,
      data: {
        permission: PERMISSIONS_CONSTANT.Category_JobPosition_View,
        preload: true
      },
      canActivate: [AppRouteGuard],
    },
    {
      path: "levels",
      component: LevelsComponent,
      data: {
        permission: PERMISSIONS_CONSTANT.Category_Level,
        preload: true
      },
      canActivate: [AppRouteGuard],
    },
    {
      path: "punishment-type",
      component: PunishmentTypesComponent,
      data: {
        permission: PERMISSIONS_CONSTANT.Category_PunishmentType,
        preload: true
      },
      canActivate: [AppRouteGuard],
    },
    {
      path: "skills",
      component: SkillsComponent,
      data: {
        permission: PERMISSIONS_CONSTANT.Category_Skill,
        preload: true
      },
      canActivate: [AppRouteGuard],
    },
    {
      path: "teams",
      component: TeamsComponent,
      data: {
        permission: PERMISSIONS_CONSTANT.Category_Team,
        preload: true
      },
      canActivate: [AppRouteGuard],
    },
    {
      path: "user-types",
      component: UserTypesComponent,
      data: {
        permission: PERMISSIONS_CONSTANT.Category_Usertype_View,
        preload: true
      },
      canActivate: [AppRouteGuard],
    },
    {
      path: "banks",
      component: BanksComponent,
      data: {
        permission: PERMISSIONS_CONSTANT.Category_Bank_View,
        preload: true
      },
      canActivate: [AppRouteGuard],
    },
    {
      path: "issued-by",
      component: IssuedByComponent,
      data: {
        permission: PERMISSIONS_CONSTANT.Category_IssuedBy_View,
        preload: true
      },
      canActivate: [AppRouteGuard],
    },
    {
      path: "charts",
      component: ChartsComponent,
      data: {
        permission: PERMISSIONS_CONSTANT.Category_Chart_View,
        preload: true
      },
      canActivate: [AppRouteGuard],
    },
    {
      path: "chart-details",
      component: ChartDetailsComponent,
      data: {
        permission: PERMISSIONS_CONSTANT.Category_Chart_ChartDetail_View,
        preload: true
      },
      canActivate: [AppRouteGuard]
    }
  ];
  
  @NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule],
  })
  export class CategoriesRoutingModule { }
  