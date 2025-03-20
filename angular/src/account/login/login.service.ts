import { TokenService } from '@node_modules/abp-ng2-module/public-api';

import { GoogleLoginService } from './google-login.service';
import { Injectable } from '@angular/core';
import { AppAuthService } from '@shared/auth/app-auth.service';
import { IHashMezonAuthModel } from '@app/service/model/employee/MezonUser.dto';
import { catchError, finalize } from 'rxjs/operators';
import { TokenAuthServiceProxy } from '@shared/service-proxies/service-proxies';
@Injectable({
  providedIn: 'root'
})
export class LoginService {

  constructor(private _googleLoginService: GoogleLoginService,private _tokenAuthServiceProxy: TokenAuthServiceProxy,
     private authService:AppAuthService,
   ) { }
  authenticateGoogle(googleToken: string, finallyCallback?: () => void): void {
    finallyCallback = finallyCallback || (() => { });

    this._googleLoginService.googleAuthenticate(googleToken)
        .subscribe((result: any) => {
          this.authService.processAuthenticateResult(result.result)
        });
}

authenticateMezonHash(authDto: IHashMezonAuthModel, errorHandller?: (error?: any) => any): void {
  this._tokenAuthServiceProxy
      .mezonHashAuthenticate(authDto)
      .pipe(
          finalize(() => { }),
          catchError((error) => {
              return errorHandller(error);
          })
      )
      .subscribe((result: any) => {
          this.authService.processAuthenticateResult(result);
      })
}

authenticateMezon(mezonToken: string, finallyCallback?: () => void): void{
  finallyCallback = finallyCallback || (() => {});

  this._googleLoginService.mezonAuthenticate(mezonToken)
      .subscribe((result: any) => {
        this.authService.processAuthenticateResult(result.result)
      });
}

}
