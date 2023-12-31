import { GoogleLoginService } from './google-login.service';
import { Injectable } from '@angular/core';
import { AppAuthService } from '@shared/auth/app-auth.service';

@Injectable({
  providedIn: 'root'
})
export class LoginService {

  constructor(private _googleLoginService: GoogleLoginService, private authService:AppAuthService) { }
  authenticateGoogle(googleToken: string, finallyCallback?: () => void): void {
    finallyCallback = finallyCallback || (() => { });

    this._googleLoginService.googleAuthenticate(googleToken)
        .subscribe((result: any) => {
          this.authService.processAuthenticateResult(result.result)
        });
}


}
