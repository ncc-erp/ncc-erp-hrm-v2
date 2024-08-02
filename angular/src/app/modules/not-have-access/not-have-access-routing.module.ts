import { PERMISSIONS_CONSTANT } from './../../permission/permission';
import { NotHaveAccessComponent } from './not-have-access.component';
import { AppRouteGuard } from './../../../shared/auth/auth-route-guard';
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

const routes: Routes = [
    {
      path: "no-have-access",
      component: NotHaveAccessComponent,
      canActivate: [AppRouteGuard],
  
    }
    
  ];
  
  @NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule],
  })
  export class NotHaveAccessRoutingModule {
    constructor() {
    }
   }