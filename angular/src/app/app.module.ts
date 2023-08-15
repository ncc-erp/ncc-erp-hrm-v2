import { RefundsModule } from './modules/refunds/refunds.module';
import { APP_INITIALIZER, ErrorHandler, NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientJsonpModule } from '@angular/common/http';
import { HttpClientModule } from '@angular/common/http';
import { ModalModule } from 'ngx-bootstrap/modal';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { CollapseModule } from 'ngx-bootstrap/collapse';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { ServiceProxyModule } from '@shared/service-proxies/service-proxy.module';
import { SharedModule } from '@shared/shared.module';
import { HomeComponent } from '@app/home/home.component';
import { AboutComponent } from '@app/about/about.component';

import { HeaderComponent } from './layout/header.component';
import { HeaderLeftNavbarComponent } from './layout/header-left-navbar.component';
import { HeaderLanguageMenuComponent } from './layout/header-language-menu.component';
import { HeaderUserMenuComponent } from './layout/header-user-menu.component';
import { FooterComponent } from './layout/footer.component';
import { SidebarComponent } from './layout/sidebar.component';
import { SidebarLogoComponent } from './layout/sidebar-logo.component';
import { SidebarUserPanelComponent } from './layout/sidebar-user-panel.component';
import { SidebarMenuComponent } from './layout/sidebar-menu.component';
import { TestComponentComponent } from './modules/test-component/test-component.component';
import { TestDialogComponent } from './modules/test-component/test-dialog/test-dialog.component';
import { NgxMatSelectSearchModule } from 'ngx-mat-select-search';
import { MAT_DATE_LOCALE } from '@angular/material/core';
import { GoogleLoginProvider, SocialAuthServiceConfig } from '@abacritt/angularx-social-login';
import { AppConsts } from '@shared/AppConsts';
import { DebtModule } from './modules/debt/debt.module';
import { AdminModule } from './modules/admin/admin.module';
import { CategoriesModule } from './modules/categories/categories.module';
import { PunishmentsModule } from './modules/punishments/punishments.module';
import { PayRollComponent } from './modules/salaries/pay-roll/pay-roll.component';
import { ListInfoComponent } from './home/listinfo/list-info/list-info.component';
import { HomeFilterComponent } from './home/home-filter/home-filter/home-filter.component';
@NgModule({
  declarations: [
    AppComponent,
    HomeComponent,
    AboutComponent,
    HeaderComponent,
    HeaderLeftNavbarComponent,
    HeaderLanguageMenuComponent,
    HeaderUserMenuComponent,
    FooterComponent,
    SidebarComponent,
    SidebarLogoComponent,
    SidebarUserPanelComponent,
    SidebarMenuComponent,
    TestComponentComponent,
    TestDialogComponent,
    PayRollComponent,
    ListInfoComponent,
    HomeFilterComponent,
  ],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    HttpClientModule,
    HttpClientJsonpModule,
    ModalModule.forChild(),
    BsDropdownModule,
    CollapseModule,
    AppRoutingModule,
    ServiceProxyModule,
    SharedModule,
    NgxMatSelectSearchModule,
    AdminModule,
    CategoriesModule,
    PunishmentsModule,
    DebtModule,
    RefundsModule
  ],
  providers: [
    {
      provide: MAT_DATE_LOCALE,
      useValue: 'en-GB',
    },
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
    },
  ],
  entryComponents: [
  ],
})
export class AppModule { }
