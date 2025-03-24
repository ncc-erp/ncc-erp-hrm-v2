

import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { finalize } from 'rxjs/operators';
import { TokenService, LogService, UtilsService } from 'abp-ng2-module';
import { AppConsts } from '@shared/AppConsts';
import { UrlHelper } from '@shared/helpers/UrlHelper';
import {
    AuthenticateModel,
    AuthenticateResultModel,
    TokenAuthServiceProxy,
} from '@shared/service-proxies/service-proxies';
import { MezonAppEvent, MezonWebViewEvent } from 'types/webview';
import { Subject } from '@node_modules/rxjs';
import { Observable } from '@node_modules/rxjs';
import { HttpResponse } from '@angular/common/http';

@Injectable()
export class AppAuthService {
    authenticateModel: AuthenticateModel;
    authenticateResult: AuthenticateResultModel;
    rememberMe: boolean;
    private userHashData = new Subject<string>();
    private isInMezon = new Subject<boolean>();

    userHashData$ = this.userHashData.asObservable();
    isInMezon$ = this.isInMezon.asObservable();
    

    constructor(
        private _tokenAuthService: TokenAuthServiceProxy,
        private _router: Router,
        private _utilsService: UtilsService,
        private _tokenService: TokenService,
        private _logService: LogService
    ) {
        this.clear();
    }

    logout(reload?: boolean): void {
        abp.auth.clearToken();
        abp.utils.setCookieValue(
            AppConsts.authorization.encryptedAuthTokenName,
            undefined,
            undefined,
            abp.appPath
        );
        if (reload !== false) {
            location.href = AppConsts.appBaseUrl;
        }
    }

    authenticate(finallyCallback?: () => void): void {
        finallyCallback = finallyCallback || (() => { });

        this._tokenAuthService
            .authenticate(this.authenticateModel)
            .pipe(
                finalize(() => {
                    finallyCallback();
                })
            )
            .subscribe((result: AuthenticateResultModel) => {
                this.processAuthenticateResult(result);
            });
    }

    public processAuthenticateResult(
        authenticateResult: AuthenticateResultModel
    ) {
        this.authenticateResult = authenticateResult;

        if (authenticateResult.accessToken) {
            // Successfully logged in
            this.login(
                authenticateResult.accessToken,
                authenticateResult.encryptedAccessToken,
                authenticateResult.expireInSeconds,
                this.rememberMe
            );
        } else {
            // Unexpected result!

            this._logService.warn('Unexpected authenticateResult!');
            this._router.navigate(['account/login']);
        }
    }

    ping() {
        window.Mezon.WebView.postEvent("PING" as MezonWebViewEvent, { message: "PING" }, () => { })
    }

    listenToPong() {
        window.Mezon.WebView.onEvent("PONG" as MezonAppEvent, () => {
            
        });
    }

    sendBotId() {
        window.Mezon.WebView.postEvent("SEND_BOT_ID" as MezonWebViewEvent, { appId: AppConsts.mezonAppId }, () => { })
    }
    listenToUserHashInfo() {
        window.Mezon.WebView.onEvent("USER_HASH_INFO" as MezonAppEvent, async (_,data: any) => {
            this.userHashData.next(data.message.web_app_data);
            
        }
        
    )}


    removeEventListeners() {
        window.Mezon.WebView.offEvent("CURRENT_USER_INFO" as MezonAppEvent, () => { })
        window.Mezon.WebView.offEvent("USER_HASH_INFO" as MezonAppEvent, () => { })
    }

    private login(
        accessToken: string,
        encryptedAccessToken: string,
        expireInSeconds: number,
        rememberMe?: boolean
    ): void {
        const tokenExpireDate = rememberMe
            ? new Date(new Date().getTime() + 1000 * expireInSeconds)
            : undefined;

        this._tokenService.setToken(accessToken, tokenExpireDate);

        this._utilsService.setCookieValue(
            AppConsts.authorization.encryptedAuthTokenName,
            encryptedAccessToken,
            tokenExpireDate,
            abp.appPath
        );

        let initialUrl = UrlHelper.initialUrl;
        if (initialUrl.indexOf('/login') > 0) {
            initialUrl = AppConsts.appBaseUrl;
        }

        location.href = initialUrl;
    }

    private clear(): void {
        this.authenticateModel = new AuthenticateModel();
        this.authenticateModel.rememberClient = false;
        this.authenticateResult = null;
        this.rememberMe = false;
    }

}
