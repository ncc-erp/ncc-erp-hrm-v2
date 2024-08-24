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
  updateUserActive(userId:number, isActive:boolean): Observable<any> {
    return this.processPut(`UpdateUserActive?userId=${userId}&&isActive=${isActive}`,{});
  }
  public getUserByEmail(email: string):Observable<any>{
    return this.processGet(`GetUserByEmail?email=${email}`)
  }
}

