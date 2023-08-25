import { ApiResponseDto } from './../../model/common.dto';
import { Observable } from 'rxjs';
import { Injectable, Injector } from '@angular/core';
import {BaseApiService} from '../base-api.service'

@Injectable({
  providedIn: 'root'
})
export class PayRollService extends BaseApiService{
  changeUrl() {
    return "Payroll"
  }

  constructor(injector : Injector) {
    super(injector);
  }

  public changeStatus(input: object):Observable<ApiResponseDto<any>>{
    return this.processPost('ChangeStatus', input);
  }

  public ExecuatePayroll(id: number):Observable<ApiResponseDto<any>>{
    return this.processPut(`ExecuatePayroll?payrollId=${id}`, {});
  }

  public GetListDateFromPayroll():Observable<ApiResponseDto<string[]>>{
    return this.processGet('GetListDateFromPayroll');
  }

  public CreateFinfastOutcomeEntry(id: number):Observable<ApiResponseDto<any>>{
    return this.processPost(`CreateFinfastOutcomeEntry?payrollId=${id}`, {});
  }

  public ValidFinfastBranch(payrollId: number):Observable<ApiResponseDto<any>>{
    return this.processGet(`ValidFinfastBranch?payrollId=${payrollId}`);
  }
}
