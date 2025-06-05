import { Injectable, Injector } from '@angular/core';
import { Observable } from 'rxjs';
import { BaseApiService } from '../base-api.service';

@Injectable({
  providedIn: 'root'
})
export class UserService  extends BaseApiService{
  constructor(injector: Injector){
      super(injector)
  }
  changeUrl() {
      return "User"
  }
  UpdateUserRole(user): Observable<any> {
    return this.processPut('UpdateUserRole', user);
  }
  updateUserActive(email:string, isActive:boolean): Observable<any> {
    return this.processPut(`UpdateUserActive?email=${email}&&isActive=${isActive}`,{});
  }
  getUserMezonIdByEmail(email : string): Observable<any> {
    return this.processGet(`GetUserMezonIdByEmail?email=${email}`);
  }

  updateUserMezonId(email: string, userMezonId: string): Observable<any> {
    return this.processPut(`UpdateUserMezonId?email=${email}&&userMezonId=${userMezonId}`, {});
  }
}

