import { AvatarDto, CreateUpdateEmployeeDto } from './../../model/employee/employee.dto';
import { GetInputFilterDto } from '../../model/employee/GetEmployeeExcept.dto';
import { Injectable, Injector } from '@angular/core';
import { ApiResponseDto, MessageResponse } from '@app/service/model/common.dto';
import { PagedRequestDto } from '@shared/paged-listing-component-base';
import { Observable } from 'rxjs';
import { BaseApiService } from '../base-api.service';
import { ChangeEmployeeBranchDto } from '@app/modules/employees/employee-detail/personal-info/change-branch/change-branch.component';

@Injectable({
  providedIn: 'root'
})
export class EmployeeService extends BaseApiService {

  changeUrl() {
    return "Employee"
  }

  constructor(injector: Injector) {
    super(injector)
  }

  SyncAllEmployees():Observable<ApiResponseDto<any>> {
    return this.processPost(`UpdateAllWorkingEmployeeInfoToOtherTools`,{})
  }
  
  GetEmployeeExcept( input: GetInputFilterDto): Observable<ApiResponseDto<any>> {
    return this.processPost(`GetEmployeeExcept`, input)
  }

  get(id: number){
    return this.processGet(`Get?id=${id}`)
  }
  public uploadAvatar(file: File , employeeId : string): Observable<ApiResponseDto<AvatarDto>>{
    const formData = new FormData();
    formData.append('file', file);
    formData.append('employeeId', employeeId);
    return this.processPost(`UploadAvatar`, formData)
  }

  public updateEmployee(employee: CreateUpdateEmployeeDto){
    return this.processPut(`Update`,employee)
  }

  public updateEmployeeToOtherTool(input){
    return this.processPost(`UpdateEmployeeInfoToOtherTool`, input)
  }

  public changeEmployeeBranch(input: ChangeEmployeeBranchDto):Observable<ApiResponseDto<any>>{
    return this.processPost(`ChangeEmployeeBranch` , input);
  }

  public getAllEmployeesBackToWork(input: GetInputFilterDto):Observable<ApiResponseDto<any>>{
    return this.processPost(`GetAllEmployeesBackToWork`, input);
  }

  public exportEmployee(request : GetInputFilterDto): Observable<ApiResponseDto<any>>{
    return this.processPost(`ExportEmployee`, request);
  }

  public GetDataMetaToCreateEmployeeByFile():Observable<ApiResponseDto<any>>{
    return this.processGet(`GetDataMetaToCreateEmployeeByFile`);
  }

  public GetDataMetaToUpdateEmployeeByFile():Observable<ApiResponseDto<any>>{
    return this.processGet(`GetDataMetaToUpdateEmployeeByFile`);
  }

  public GetAllEmployeesToUpdateContract(input: GetInputFilterDto):Observable<ApiResponseDto<any>>{
    return this.processPost(`GetAllEmployeesToUpdateContract`, input);
  }
  public GetEmployeeBasicInfoForBreadcrumb(employeeId: number):Observable<ApiResponseDto<any>>{
    return this.processGet(`GetEmployeeBasicInfoForBreadcrumb?employeeId=${employeeId}`)
  }

  public createEmployeeFromFile(input : FormData):Observable<ApiResponseDto<any>>{
    return this.processPost(`CreateEmployeeFromFile`,input)
  }

  public updateEmployeeFromFile(input : FormData):Observable<ApiResponseDto<any>>{
    return this.processPost(`UpdateEmployeeFromFile`,input)
  }

  public ReCreateEmployeeToOtherTool(employeeId: number):Observable<ApiResponseDto<any>>{
    return this.processGet(`ReCreateEmployeeToOtherTool?employeeId=${employeeId}`);
  }

  public createEmployee(data:any, tempemployeeId?:number): Observable<ApiResponseDto<any>>{
    return this.processPost(`Create?tempemployeeId=${tempemployeeId}`, data);
  }

  public ExportEmployeeStatistic(startDate:string, endDate:string): Observable<ApiResponseDto<any>>{
    var dto = {
      startDate:startDate,
      endDate:endDate
    }
    return this.processPost(`ExportEmployeeStatistic`,dto);
  }

  public getAllEmployeeBasicInfo(): Observable<ApiResponseDto<any>>{
    return this.processGet(`GetAllEmployeeBasicInfo`);
  }

  public quitJobToOtherTool(input): Observable<ApiResponseDto<any>>{
    return this.processPost(`QuitJobToOtherTool`, input);
  }
}
