import { UpdateBenefitStatusDto } from './../../model/benefits/ChangeBenefitStatus.dto';
import { UpdateEmployeeDateDto } from './../../model/benefits/updateEmployeeDate.dto';
import { AddUpdateBenefitemployeeDto } from './../../model/benefits/updateBenefitEmployee.dto';
import { AddEmployeeToBenefitDto, QuickAddEmployeeDto } from './../../model/benefits/addEmployee.dto';
import { Injectable, Injector } from '@angular/core';
import { BenefitEmployeeDto } from '@app/service/model/benefits/benefitEmployee.dto';
import { benefitDto } from '@app/service/model/benefits/beneft.dto';
import { CloneBenefitDto } from '@app/service/model/benefits/CloneBenefitDto';
import { ApiResponseDto } from '@app/service/model/common.dto';
import { PagedRequestDto, PagedResultDto } from '@shared/paged-listing-component-base';
import { Observable } from 'rxjs';
import { BaseApiService } from '../base-api.service';
import { GetEmployeeDto } from '@app/service/model/employee/employee.dto';
import { GetInputFilterDto } from '@app/service/model/employee/GetEmployeeExcept.dto';

@Injectable({
  providedIn: 'root'
})
export class BenefitService extends BaseApiService {

  changeUrl() {
    return "Benefit"
  }

  constructor(injector: Injector) {
    super(injector)
  }

  GetEmployeeInBenefitPaging(benefitId: number, input: GetInputFilterDto): Observable<ApiResponseDto<PagedResultDto>> {
    return this.processPost(`GetEmployeeInBenefit?benefitId=${benefitId}`, input)
  }

  GetListMonthFilter(): Observable<ApiResponseDto<any>> {
    return this.processGet(`GetListMonthFilter`)
  }

  AddEmployeeToBenefit(input: AddEmployeeToBenefitDto): Observable<ApiResponseDto<benefitDto>> {
    return this.processPost(`AddEmployeeToBenefit`, input)
  }

  QuickAddEmployee(input: QuickAddEmployeeDto): Observable<ApiResponseDto<BenefitEmployeeDto>> {
    return this.processPost(`QuickAddEmployee`, input)
  }

  GetListEmployeeIdInBenefit(benefitId: number): Observable<ApiResponseDto<number[]>> {
    return this.processGet(`GetListEmployeeIdInBenefit?benefitId=${benefitId}`)
  }

  GetBenefitByEmployeeId(payload: PagedRequestDto,employeeId: number): Observable<ApiResponseDto<PagedResultDto>>{
    return this.processGetAllPaging(`GetBenefitByEmployeeId?id=${employeeId}`,payload)
  }

  RemoveEmployeeFromBenefit(id: number): Observable<ApiResponseDto<number>> {
    return this.processDelete(`RemoveEmployeeFromBenefit?id=${id}`)
  }

  
  UpdateBenefitEmployee(input: AddUpdateBenefitemployeeDto): Observable<ApiResponseDto<BenefitEmployeeDto>> {
    return this.processPut(`UpdateBenefitEmployee`, input)
  }

  UpdateAllStartDate(input:UpdateEmployeeDateDto): Observable<ApiResponseDto<BenefitEmployeeDto>> {
    return this.processPut(`UpdateAllStartDate`, input)
  }

  UpdateAllEndDate(input: UpdateEmployeeDateDto): Observable<ApiResponseDto<BenefitEmployeeDto>> {
    return this.processPut(`UpdateAllEndDate`, input)
  }

  UpdateStatus(input: UpdateBenefitStatusDto): Observable<ApiResponseDto<BenefitEmployeeDto>> {
    return this.processPut(`UpdateStatus`, input)
  }

  CloneBenefit(input: CloneBenefitDto): Observable<ApiResponseDto<any>>{
    return this.processPost(`CloneBenefit`, input)
  }

  GetAllEmployeeNotInBenefit(benefitId: number): Observable<ApiResponseDto<GetEmployeeDto[]>>{
    return this.processGet(`GetAllEmployeeNotInBenefit?benefitId=${benefitId}`)
  }
  GetAllBenefitsByEmployeeId(id: number): Observable<ApiResponseDto<any>>{
    return this.processGet(`GetAllBenefitsByEmployeeId?id=${id}`)
  }

}
