import { Injectable, Injector } from '@angular/core';
import { BaseApiService } from '../base-api.service';

@Injectable({
  providedIn: 'root'
})
export class UserTypeService extends BaseApiService {

  changeUrl() {
    return "UserType"
  }

  constructor(injector: Injector) {
    super(injector)
  }
}
