import { PagedRequestDto, PagedResultDto } from './../../../../shared/paged-listing-component-base';
import { ApiResponseDto } from './../../model/common.dto';
import { Injectable, Injector } from '@angular/core';
import { Observable } from 'rxjs';
import { BaseApiService } from '../base-api.service';
import { GetInputFilterDto } from '@app/service/model/employee/GetEmployeeExcept.dto';
import { AddEmployeeToRefundDto, UpdateRefundEmployeeDto } from '@app/service/model/refunds/refundEmployee.dto';

@Injectable({
  providedIn: 'root'
})
export class RefundsService extends BaseApiService {
  changeUrl() {
    return "Refund";
  }

  constructor(injector: Injector) {
    super(injector);
   }

   public GetRefundEmployeesPaging(id:number, input: GetInputFilterDto): Observable<ApiResponseDto<PagedResultDto>> {
    return this.processPost(`GetRefundEmployeesPaging?id=${id}`,input);
  }

  public AddEmployeeToRefund(input: AddEmployeeToRefundDto): Observable<ApiResponseDto<AddEmployeeToRefundDto>> {
    return this.processPost(`AddEmployeeToRefund`,input);
  }

  public UpdateRefundEmployee(input: UpdateRefundEmployeeDto): Observable<ApiResponseDto<UpdateRefundEmployeeDto>> {
    return this.processPut(`UpdateRefundEmployee`,input);
  }

  public DeleteRefundEmployee(id:number): Observable<ApiResponseDto<UpdateRefundEmployeeDto>> {
    return this.processDelete(`DeleteRefundEmployee?id=${id}`);
  }

  public GetListRefundDate(): Observable<ApiResponseDto<string[]>> {
    return this.processGet(`GetListRefundDate`);
  }

  public GetListMonthsForCreate(): Observable<ApiResponseDto<string[]>> {
    return this.processGet(`GetListMonthsForCreate`);
  }
  public GetAllEmployeeNotInRefund(id:number): Observable<ApiResponseDto<any>> {
    return this.processGet(`GetAllEmployeeNotInRefund?id=${id}`);
  }

  public ActiveRefund(id:number): Observable<ApiResponseDto<number>> {
    return this.processGet(`ActiveRefund?id=${id}`);
  }

  public DeActiveRefund(id:number): Observable<ApiResponseDto<number>> {
    return this.processGet(`DeActiveRefund?id=${id}`);
  }

}
