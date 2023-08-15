import { Injectable, Injector } from '@angular/core';
import { BaseApiService } from '../base-api.service';

@Injectable({
  providedIn: 'root'
})
export class DebtPaymentPlanService extends BaseApiService {

  changeUrl() {
    return "DebtPaymentPlan"
  }

  constructor(injector: Injector) {
    super(injector)
  }
  getPaymentPlansByDebtId(debtId){
    return this.processGet(`GetPaymentPlansByDebId?debtId=${debtId}`)
  }
  generatePaymentPlan(startDate,endDate,money,debtId){
    return this.processPost(`GeneratePlan`,{startDate,endDate,money,debtId})
  }
}
