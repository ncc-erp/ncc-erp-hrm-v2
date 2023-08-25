import { Injectable, Injector } from '@angular/core';
import { ApiResponseDto } from '@app/service/model/common.dto';
import { InputToGetAllPagingDto } from '@app/service/model/punishment-fund/punishment-fund.dto';
import { Observable } from 'rxjs';
import {BaseApiService} from '../base-api.service'

@Injectable({
  providedIn: 'root'
})
export class PunishmentFundsService extends BaseApiService{
  changeUrl() {
    return "PunishmentFund"
  }

  constructor(injector: Injector) {
    super(injector);
  }
  public getAllPunishmentFundsPagging(input: InputToGetAllPagingDto): Observable<ApiResponseDto<any>> {
    return this.processPost('GetAllPaging', input);
  }
  public add(input):Observable<ApiResponseDto<any>>{
    return this.processPost( `Add`, input);
  }
  public disburse(input):Observable<ApiResponseDto<any>>{
    return this.processPost( `Disburse`, input);
  }

}
