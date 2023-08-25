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
  
}

