import { Injectable, Injector } from '@angular/core';
import { BaseApiService } from '../base-api.service';
import { ApiResponseDto } from '../../model/common.dto';
import { Observable } from 'rxjs';




import { HttpClient } from '@angular/common/http';
import * as moment from 'moment';

@Injectable({
  providedIn: 'root'
})
export class PayrollTokenServiceService extends BaseApiService {
  changeUrl() { 
    return "PayrollToken"
  }
  constructor(injector: Injector) { 
    super(injector)
  }
  public UpdatePaySlipToken(input: object):Observable<ApiResponseDto<any>>{
    return this.processPost('UpdatePaySlipToken', input);
  }
  public getAllPaggingPayslip(payrollId : any,payload: any): Observable<ApiResponseDto<any>> {
    return this.processGetAllPaging(`GetAllPagingPayslip?payrollId=${payrollId}`, payload)
  }

  public deletePayrollToken(payrollId: any): Observable<ApiResponseDto<any>> {
   return this.processDelete(`DeletePayrollToken?payrollId=${payrollId}`);
  }
  public deletePayslipToken(id: any): Observable<ApiResponseDto<any>> {
   return this.processDelete(`DeletePaySlipToken?id=${id}`);
  }

  public sendToken(id: any): Observable<ApiResponseDto<any>> {
    return this.processPost(`SendToken?paySlipTokenId=${id}`,null );
  }
}
