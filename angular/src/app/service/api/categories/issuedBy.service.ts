import { Injectable, Injector } from '@angular/core';

import { BaseApiService } from '../base-api.service';

@Injectable({
  providedIn: 'root'
})

export class IssuedByService extends BaseApiService {

  changeUrl() {
    return "IssuedBy"
  }

  constructor(injector: Injector) {
    super(injector)
  }
}