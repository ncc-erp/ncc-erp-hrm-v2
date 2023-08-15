import { Component, Injector } from '@angular/core';
import { AbpSessionService } from 'abp-ng2-module';
import { AppComponentBase } from '@shared/app-component-base';
import { accountModuleAnimation } from '@shared/animations/routerTransition';
import { AppAuthService } from '@shared/auth/app-auth.service';
import { LoginService } from './login.service';
import { GoogleLoginProvider, SocialAuthService, SocialUser } from '@abacritt/angularx-social-login';
import { AppConsts } from '@shared/AppConsts';
@Component({
  templateUrl: './login.component.html',
  animations: [accountModuleAnimation()]
})
export class LoginComponent extends AppComponentBase {
  submitting = false;
  user: SocialUser
  tenancyName: string
  loggedIn: boolean;
  enableNormalLogin:boolean = false
  constructor(
    injector: Injector,
    public authService: AppAuthService,
    private _sessionService: AbpSessionService,
    private googleAuthService: SocialAuthService,
    private loginService: LoginService
  ) {
    super(injector);
  }
  ngOnInit(): void {
    this.enableNormalLogin = AppConsts.enableNormalLogin
  }
  get multiTenancySideIsTeanant(): boolean {
    return this._sessionService.tenantId > 0;
  }

  get isSelfRegistrationAllowed(): boolean {
    if (!this._sessionService.tenantId) {
      return false;
    }

    return true;
  }
  login(): void {
    this.submitting = true;
    this.authService.authenticate(() => (this.submitting = false));
  }
  signInWithGoogle() {
    this.googleAuthService.signIn(GoogleLoginProvider.PROVIDER_ID).then((rs: any) =>{
      this.loginService.authenticateGoogle(rs.idToken)
    })
  }
}
