import { Injectable, Injector } from '@angular/core';
import { ApiResponseDto } from '@app/service/model/common.dto';
import { DebtInputFilterDto } from '@app/service/model/debt/debt.dto';
import { SendDebtMailToOneEmployeeDto } from '@app/service/model/mail/sendMail.dto';
import { PagedRequestDto, PagedResultDto } from '@shared/paged-listing-component-base';
import { Observable } from 'rxjs';
import { BaseApiService } from '../base-api.service';

@Injectable({
  providedIn: 'root'
})
export class DebtService extends BaseApiService {

  changeUrl() {
    return "Debt"
  }

  constructor(injector: Injector) {
    super(injector)
  }

  get(debtId) {
    return this.processGet(`Get?id=${debtId}`);
  }

  public getAllDebtPagging(input: DebtInputFilterDto): Observable<ApiResponseDto<PagedResultDto>> {
    return this.processPost("GetAllPaging", input);
  }

  public getAllDebtByEmployee(id: number, payload: PagedRequestDto ):Observable<ApiResponseDto<any>> {
    return this.processPost(`GetByEmployeeId?id=${id}`, payload)
  }

  public setDone(id: number) {
    return this.processGet(`SetDone?id=${id}`)
  }

  public sendAllMail(input: DebtInputFilterDto):Observable<ApiResponseDto<any>>{
    return this.processPost(`SendMailToAllEmployee`,input);
  }

  public sendMail(input: SendDebtMailToOneEmployeeDto):Observable<ApiResponseDto<any>>{
    return this.processPost(`SendMailToOneEmployee`, input);
  }

  public getDebtTemplate(debtId: number):Observable<ApiResponseDto<any>>{
    return this.processGet(`GetDebtTemplate?debtId=${debtId}`);
  }
}
