import { Injectable, Injector } from '@angular/core';

import { BaseApiService } from '../base-api.service';

@Injectable({
  providedIn: 'root'
})
export class BankService extends BaseApiService {

  changeUrl() {
    return "Bank"
  }

  constructor(injector: Injector) {
    super(injector)
  }
}
