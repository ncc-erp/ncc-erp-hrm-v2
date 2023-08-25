import { Injectable, Injector } from '@angular/core';
import {BaseApiService} from '../base-api.service';
import { ApiResponseDto } from '../../model/common.dto';
import { Observable } from 'rxjs';
import { GetInputFilterDto, GetInputFilterRequestUpdateInfoDto } from '../../model/employee/GetEmployeeExcept.dto';
import {GetRequestDetailDto, UpdateEmployeeBackDateDto, UpdateRequestDetailDto} from '../../model/warning-employee/WarningEmployeeDto';
@Injectable({
  providedIn: 'root'
})
export class WarningEmployeeService extends BaseApiService{
  changeUrl() {
    return "WarningEmployee"
  }

  constructor(injector : Injector) {
    super(injector);
  }
  public getAllEmployeesBackToWork(input: GetInputFilterDto):Observable<ApiResponseDto<any>>{
    return this.processPost(`GetAllEmployeesBackToWork`, input);
  }
  public GetAllEmployeesToUpdateContract(input: GetInputFilterDto):Observable<ApiResponseDto<any>>{
    return this.processPost(`GetAllEmployeesToUpdateContract`, input);
  }
  public updateEmployeeBackDate(input: UpdateEmployeeBackDateDto):Observable<ApiResponseDto<UpdateEmployeeBackDateDto>>{
    return this.processPut(`UpdateEmployeeBackDate`,input);
  }

  public GetTempEmployeeTalentPaging(input: GetInputFilterDto):Observable<ApiResponseDto<any>>{
    return this.processPost(`GetTempEmployeeTalentPaging`,input);
  }

  public GetTempEmployeeTalentById(id:number):Observable<ApiResponseDto<any>>{
    return this.processGet(`GetTempEmployeeTalentById?id=${id}`);
  }
  public getRequestUpdateInfo(input: GetInputFilterRequestUpdateInfoDto):Observable<ApiResponseDto<any>>{
    return this.processPost(`GetRequestUpdateInfo`,input);
  }
  public approveRequestUpdateInfo(input: UpdateRequestDetailDto):Observable<ApiResponseDto<any>>{
    return this.processPut(`ApproveRequestUpdateInfo`,input);
  }
  public rejectRequestUpdateInfo(input: object):Observable<ApiResponseDto<any>>{
    return this.processPut(`RejectRequestUpdateInfo`,input);
  }
  public getRequestDetailById(id: number):Observable<ApiResponseDto<any>>{
    return this.processGet(`GetRequestDetailById?id=${id}`);
  }
  public GetPlanQuitEmployee():Observable<ApiResponseDto<any>>{
    return this.processGet(`GetPlanQuitEmployee`);
  }

  public DeleteTempEmployeeTalent(id:number):Observable<ApiResponseDto<any>>{
    return this.processDelete(`DeleteTempEmployeeTalent?id=${id}`);
  }

  public DeletePlanQuitBgJob(id:number):Observable<ApiResponseDto<any>>{
    return this.processDelete(`DeletePlanQuitBgJob?id=${id}`);
  }

  public UpdatePlanQuitBgJob(input):Observable<ApiResponseDto<any>>{
    return this.processPut(`UpdatePlanQuitBgJob`, input);
  }
  public UpdateTempEmployeeTalent(input):Observable<ApiResponseDto<any>>{
    return this.processPut(`UpdateTempEmployeeTalent`, input);
  }
}
