import { Injectable, Injector } from '@angular/core';
import { AddBonusEmployeeDto, AddBonusForEmployeeDto, BonusEmployeeDto, EditBonusEmployeeDto } from '@app/service/model/bonuses/bonus.dto';
import { ApiResponseDto, MessageResponse } from '@app/service/model/common.dto';
import { GetEmployeeDto } from '@app/service/model/employee/employee.dto';
import { GetInputFilterDto } from '@app/service/model/employee/GetEmployeeExcept.dto';
import { SendBonusMailToOneEmployeeDto } from '@app/service/model/mail/sendMail.dto';
import { PagedRequestDto, PagedResultDto } from '@shared/paged-listing-component-base';
import { Observable } from 'rxjs';
import { BaseApiService } from '../base-api.service';

@Injectable({
  providedIn: 'root'
})
export class BonusService  extends BaseApiService{

  changeUrl() {
    return "Bonus"
  }

  constructor(injector: Injector) {
    super(injector)
  }
  public getListDate(): Observable<ApiResponseDto<any>> {
    return this.processGet("GetListDate");
  }
  public changeStatus(id: number): Observable<ApiResponseDto<number>> {
    return this.processPost("ChangeStatus?id="+id, {});
  }
  public getListMonthFilter(): Observable<ApiResponseDto<any>> {
    return this.processGet("GetListMonthFilter");
  }

  public getBonusDetail(id: number): Observable<any> {
    return this.httpClient.get(this.rootUrl + "GetBonusDetail?id="+id);
  }

  public getAllBonusEmployee(id: number , input: GetInputFilterDto): Observable<ApiResponseDto<PagedResultDto>> {

    return this.processPost("GetAllBonusEmployee?id="+id, input);
  }

  public quickAddEmployeeToBonus(employee : AddBonusEmployeeDto):Observable<ApiResponseDto<BonusEmployeeDto>>{
    return this.processPost("QuickAddEmployeeToBonus",employee);
  }
  public multipleAddEmployeeToBonus(employee : AddBonusEmployeeDto):Observable<ApiResponseDto<BonusEmployeeDto>>{
    return this.processPost("MultipleAddEmployeeToBonus",employee);
  }

  public updateEmployeeInBonus(employee : EditBonusEmployeeDto):Observable<ApiResponseDto<EditBonusEmployeeDto>>{
    return this.processPut("UpdateEmployeeInBonus", employee);
  }

  public deleteEmployeeFromBonus(id: number,bonusid: number): Observable<ApiResponseDto<number>>{
    return this.processDelete(`DeleteEmployeeFromBonus?id=${id}&bonusid=${bonusid}`);
  }

  public getAllEmployeeInBonus(bonusId: number): Observable<any> {
    return this.httpClient.get(this.rootUrl + "GetAllEmployeeInBonus?bonusId="+bonusId);
  }
  public getAllPagingBonusesByEmployeeId(employeeId: number ,payload : PagedRequestDto): Observable<ApiResponseDto<PagedResultDto>> {
    return this.processPost("GetAllPagingBonusesByEmployeeId?employeeId="+employeeId, payload);
  }
  public getListMonthFilterOfEmployee(employeeId: number): Observable<any> {
    return this.httpClient.get(this.rootUrl + "GetListMonthFilterOfEmployee?employeeId="+employeeId);
  }

  public importEmployeeToBonus(input: FormData):Observable<ApiResponseDto<MessageResponse>>{
    return this.processPost("ImportEmployeeToBonus",input);
  }

  public addBonusForEmployee(bonus : AddBonusForEmployeeDto):Observable<ApiResponseDto<AddBonusForEmployeeDto>>{
    return this.processPost("AddBonusForEmployee",bonus);
  }

  public GetAllEmployeeNotInBonus(bonusId: number): Observable<ApiResponseDto<GetEmployeeDto[]>>{
    return this.processGet(`GetAllEmployeeNotInBonus?bonusId=${bonusId}`);
  }

  public IsBonusHasEmployee(bonusId: number): Observable<ApiResponseDto<boolean>>{
    return this.processGet(`IsBonusHasEmployee?bonusId=${bonusId}`);
  }

  public sendMail(input : SendBonusMailToOneEmployeeDto): Observable<ApiResponseDto<void>>{
    return  this.processPost(`SendMailToOneEmployee`, input);
  }

  public sendAllMail(id: number , input: GetInputFilterDto): Observable<ApiResponseDto<string>> {
    return this.processPost("SendMailToAllEmployee?id="+id, input);
  }
  public getBonusTemplate(bonusEmployeeId: number):Observable<ApiResponseDto<any>>{
    return this.processGet(`GetBonusTemplate?bonusEmployeeId=${bonusEmployeeId}`);
  }
}
