

import { AppRouteGuard } from '../../../shared/auth/auth-route-guard';
import { RouterModule, Routes } from '@angular/router';
import { NgModule } from "@angular/core";
import { PERMISSIONS_CONSTANT } from '@app/permission/permission';
import { MezonTokenComponent } from './mezon-tokens/mezon-token.component';

const routes: Routes = [

    {
      path:"list-mezon-token",
      component:MezonTokenComponent,
      data: {   
        permission: PERMISSIONS_CONSTANT.Mezon_Token_View,
        preload: true
      },

    }
];
@NgModule({ 
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class PayrollTokenRoutingModule { }