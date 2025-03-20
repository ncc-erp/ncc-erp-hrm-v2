import { AppAuthService } from './../../shared/auth/app-auth.service';
import { AppConsts } from './../../shared/AppConsts';
import { HttpClient ,HttpHeaders} from '@angular/common/http';
import { Injectable, Injector } from '@angular/core';
import { Observable, throwError } from 'rxjs';
import { BaseApiService } from "../../app/service/api/base-api.service";
import { IHashMezonAuthModel } from '@app/service/model/employee/MezonUser.dto';
import { mergeMap, catchError } from 'rxjs/operators';
import { HttpResponseBase } from '@angular/common/http';
import { of,isObservable } from 'rxjs';
import { TokenAuthServiceProxy } from '@shared/service-proxies/service-proxies';
import {
  mergeMap as _observableMergeMap,
  catchError as _observableCatch,
} from 'rxjs/operators';
import {
  throwError as _observableThrow,
  of as _observableOf,
} from 'rxjs';
import { AuthenticateResultModel } from '@shared/service-proxies/service-proxies';

@Injectable({
  providedIn: 'root'
})
export class GoogleLoginService extends BaseApiService {

  changeUrl() {
    return 'TokenAuth';
  }

  constructor(
    injector:Injector,private appAuthService : AppAuthService,private tokenAuthServiceProxy : TokenAuthServiceProxy,private http: HttpClient
  ) {
    super(injector);
  }

  name() {
    return 'TokenAuth';
  }
  googleAuthenticate(googleToken: string): Observable<any> {
    return this.httpClient.post(AppConsts.remoteServiceBaseUrl +
      '/api/TokenAuth/GoogleAuthenticate', {googleToken: googleToken});
  }

  mezonAuthenticate(mezonToken: string):Observable<any>{
    
    return this.httpClient.post(AppConsts.remoteServiceBaseUrl+
      '/api/TokenAuth/MezonAuthenticate?codeOauth2Mezon='+mezonToken,{}
    )
  }

 
}
