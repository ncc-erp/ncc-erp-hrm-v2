import { LoginComponent } from './login/login.component';
import { TenantChangeComponent } from './tenant/tenant-change.component';
import { AccountFooterComponent } from './layout/account-footer.component';
import { AccountHeaderComponent } from './layout/account-header.component';
import { AccountLanguagesComponent } from './layout/account-languages.component';
import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientJsonpModule } from '@angular/common/http';
import { HttpClientModule } from '@angular/common/http';
import { AccountRoutingModule } from './account-routing.module';
import { SharedModule } from '@shared/shared.module';
import { ServiceProxyModule } from '@shared/service-proxies/service-proxy.module';
import { GoogleLoginProvider, SocialAuthServiceConfig, SocialLoginModule } from '@abacritt/angularx-social-login';
import { ModalModule } from 'ngx-bootstrap/modal';
import { AccountComponent } from './account.component';
import { RegisterComponent } from './register/register.component';
import { TenantChangeDialogComponent } from './tenant/tenant-change-dialog.component';
import { AppConsts } from '@shared/AppConsts';


@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        HttpClientModule,
        HttpClientJsonpModule,
        SharedModule,
        ServiceProxyModule,
        AccountRoutingModule,
        SocialLoginModule,
        ModalModule.forChild(),
    ],
    declarations: [
        AccountComponent,
        LoginComponent,
        RegisterComponent,
        AccountLanguagesComponent,
        AccountHeaderComponent,
        AccountFooterComponent,
        // tenant
        TenantChangeComponent,
        TenantChangeDialogComponent,
    ],
    providers: [
        {
          provide: 'SocialAuthServiceConfig',
          useValue: {
            autoLogin: true,
            providers: [
              {
                id: GoogleLoginProvider.PROVIDER_ID,
                provider: new GoogleLoginProvider(
                   AppConsts.googleClientId
                ),
              },
            ],
          } as SocialAuthServiceConfig,
        }
      ],
    entryComponents: [
        // tenant
        TenantChangeDialogComponent
    ]
})
export class AccountModule {

}
