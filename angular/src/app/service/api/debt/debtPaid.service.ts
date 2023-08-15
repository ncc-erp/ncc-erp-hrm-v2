import { Injectable, Injector } from '@angular/core';
import { BaseApiService } from '../base-api.service';

@Injectable({
  providedIn: 'root'
})
export class DebtPaidService extends BaseApiService {

  changeUrl() {
    return "DebtPaid"
  }

  constructor(injector: Injector) {
    super(injector)
  }

  getDebtPaidsByDebtId(debtId){
    return this.processGet(`GetPaidsByDebtId?debtId=${debtId}`)
  }
}
