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
  public getAllPaggingPayslip(month : any,payload: any): Observable<ApiResponseDto<any>> {
    return this.processGetAllPaging(`GetAllPagingPayslip?month=${month}`, payload)
  }

  public deletePayrollToken(month: string): Observable<ApiResponseDto<any>> {
   return this.processDelete(`DeletePayrollToken?month=${month}`);
  }
  public deletePayslipToken(id: any): Observable<ApiResponseDto<any>> {
   return this.processDelete(`DeletePaySlipToken?id=${id}`);
  }

  public sendToken(id: any): Observable<ApiResponseDto<any>> {
    return this.processPost(`SendToken?payslipId=${id}`,null );
  }
}
