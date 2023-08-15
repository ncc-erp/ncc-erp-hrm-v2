import { Injectable, Injector } from '@angular/core';
import { BaseApiService } from '../base-api.service';

@Injectable({
  providedIn: 'root'
})
export class BranchService extends BaseApiService {

  changeUrl() {
    return "Branch"
  }

  constructor(injector: Injector) {
    super(injector)
  }
}
