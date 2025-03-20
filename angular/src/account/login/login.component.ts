import { Oauth2Mezon } from './../../shared/AppConsts';
import { Component, Injector } from '@angular/core';
import { AbpSessionService } from 'abp-ng2-module';
import { AppComponentBase } from '@shared/app-component-base';
import { accountModuleAnimation } from '@shared/animations/routerTransition';
import { AppAuthService } from '@shared/auth/app-auth.service';
import { LoginService } from './login.service';
import { GoogleLoginProvider, SocialAuthService, SocialUser } from '@abacritt/angularx-social-login';
import { AppConsts } from '@shared/AppConsts';
import { ActivatedRoute, Router } from '@angular/router';
import { IHashMezonAuthModel,  IUserMezonDto } from '@app/service/model/employee/MezonUser.dto';
import {Base64} from 'js-base64'
@Component({
  templateUrl: './login.component.html',
  animations: [accountModuleAnimation()]
})
export class LoginComponent extends AppComponentBase {
  submitting = false;
  user: SocialUser
  tenancyName: string
  loggedIn: boolean;
  hashData: string;
  isAuthenticating: boolean = false;
  isAuthenFailed: boolean = false;
  isMezonApp: boolean = false;
  enableNormalLogin:boolean = AppConsts.enableNormalLogin
  enableLoginMezon:boolean = AppConsts.enableLoginMezon
  enableLoginGoogle:boolean = AppConsts.enableLoginGoogle

  private googleAuthService: SocialAuthService
  constructor(
    public injector: Injector,
    public authService: AppAuthService,
    private _sessionService: AbpSessionService,
    
    private loginService: LoginService,
    private route : ActivatedRoute,

  ) {
    super(injector);
  }
  ngOnInit(): void {
    this.googleAuthService = this.injector.get(SocialAuthService);
    this.enableNormalLogin = AppConsts.enableNormalLogin
    this.route.queryParams.subscribe(params => {
      const authorizationCode = params['code'];
      if(authorizationCode != null ){
        this.loginService.authenticateMezon(authorizationCode);
      }
    })

    this.authService.isInMezon$.subscribe((status) => {
      this.isMezonApp = status;
    })

    this.authService.userHashData$.subscribe((userHashData) => {
       this.hashData = userHashData
       this.loginWithHash(this.hashData);
    })
  }
ngOnDestroy(): void {
  this.authService.removeEventListeners();
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

  loginWithHash(hashData: string): void {
    if(hashData){
      this.isAuthenticating = true;
      const hashAuthData : IHashMezonAuthModel = {
         hashData : Base64.encode(hashData),
         tenancyName : this.tenancyName
      }
   
    this.loginService.authenticateMezonHash(hashAuthData, (error) => {
      this.isAuthenFailed = false;
      this.message.error('Login failed');
   
  })}
}

retryHashLogin(){
  this.isAuthenticating = false;
  this.isAuthenFailed = false;
  this.loginWithHash(this.hashData);
  }
  signInWithMezon() {
    const OAUTH2_AUTHORIZE_URL = Oauth2Mezon.OAUTH2_AUTHORIZE_URL;
    const CLIENT_ID = AppConsts.mezonClientId;
    const REDIRECT_URI = AppConsts.appBaseUrl+"/account/login";
     const RESPONSE_TYPE = 'code';
     const SCOPE = 'openid+offline';
     const STATE = 'hkjadkjashdkjsah'; 

    const authUrl = `${OAUTH2_AUTHORIZE_URL}?client_id=${CLIENT_ID}&redirect_uri=${REDIRECT_URI}&response_type=${RESPONSE_TYPE}&scope=${SCOPE}&state=${STATE}`;
		return (window.location.href = authUrl);
  }

}
