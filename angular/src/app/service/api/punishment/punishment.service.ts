import { PunishmentEmployeeDto, UpdateEmployeeInPunishmentDto } from './../../model/punishments/punishments.dto';
import { PagedRequestDto, PagedResultDto } from './../../../../shared/paged-listing-component-base';
import { Injectable, Injector } from '@angular/core';
import { Observable } from 'rxjs';
import { BaseApiService } from '../base-api.service';
import { ApiResponseDto, MessageResponse } from '@app/service/model/common.dto';
import { PunishmentsDto } from '@app/service/model/punishments/punishments.dto';
import { ResultGeneratePunishmentDto } from '@app/service/model/categories/punishmentType.dto';
import { GetInputFilterDto } from '@app/service/model/employee/GetEmployeeExcept.dto';
import { GetEmployeeDto } from '@app/service/model/employee/employee.dto';

@Injectable({
  providedIn: 'root'
})
export class PunishmentService  extends BaseApiService{

  changeUrl() {
    return "Punishment"
  }

  constructor(injector: Injector) {
    super(injector)
  }
  public getListDate(): Observable<ApiResponseDto<string[]>> {
    return this.processGet("GetListDate");
  }

  public changeStatus(id: number): Observable<ApiResponseDto<number>> {
    return this.processPost("ChangeStatus?id="+id, {});
  }

  public getPunishmentById(id: number):Observable<ApiResponseDto<PunishmentsDto>>{
    return this.processGet("GetPunishmentById?id="+id);
  }

  public getDateFromPunishments(): Observable<ApiResponseDto<string[]>>{
    return this.processGet("GetDateFromPunishments");
  }

  public getAllEmployeeInPunishment(id: number , input: GetInputFilterDto): Observable<ApiResponseDto<PagedResultDto>> {
    return this.processPost("GetAllEmployeeInPunishment?id="+id, input);
  }

  public addEmployeeToPunishment(employee : PunishmentEmployeeDto):Observable<ApiResponseDto<PunishmentEmployeeDto>>{
    return this.processPost("AddEmployeeToPunishment",employee);
  }

  public updateEmployeeInPunishment(employee : UpdateEmployeeInPunishmentDto):Observable<ApiResponseDto<UpdateEmployeeInPunishmentDto>>{
    return this.processPut("UpdateEmployeeInPunishment", employee);
  }

  public deleteEmployeeFromPunishment(id: number): Observable<ApiResponseDto<number>>{
    return this.processDelete(`DeleteEmployeeFromPunishment?id=${id}`);
  }

  public importFilePunishmentDetailComponent(input : FormData):Observable<ApiResponseDto<MessageResponse>>{
    return this.processPost(`ImportEmployeePunishmentsFromFile`,input)
  }

  public generatePunishment(PunishmentTypeIds: number[], date: string):Observable<ApiResponseDto<ResultGeneratePunishmentDto[]>>{
    return this.processPost(`GeneratePunishmentsByPunishmentType`,{PunishmentTypeIds,date});
}
  
  public getPunishmentByEmployeeId(id: number , payload: PagedRequestDto):Observable<ApiResponseDto<PagedResultDto>>{
    return this.processPost(`GetPunishmentByEmployeeId?id=${id}`, payload)
  }

  public getDateFromPunishmentsOfEmployee(id:number): Observable<ApiResponseDto<string[]>>{
    return this.processGet(`GetDateFromPunishmentsOfEmployee?id=${id}`);
  }

  public updatePunishmentOfEmployee(employee : UpdateEmployeeInPunishmentDto):Observable<ApiResponseDto<UpdateEmployeeInPunishmentDto>>{
    return this.processPut("UpdatePunishmentOfEmployee", employee);
  }

  public GetAllEmployeeNotInPunishment(punishmentId: number): Observable<ApiResponseDto<GetEmployeeDto[]>>{
    return this.processGet(`GetAllEmployeeNotInPunishment?punishmentId=${punishmentId}`);
  }

  public IsPunishmentHasEmployee(punishId: number):  Observable<ApiResponseDto<boolean>>{
    return this.processGet(`IsPunishmentHasEmployee?punishId=${punishId}`);
  }
}