import { Injectable, Injector } from '@angular/core';
import { Observable } from 'rxjs';
import { ChangeStatusToMaternityLeaveDto, ChangeStatusToPauseDto, ChangeStatusToQuitDto, ChangeStatusWorkingDto, ExtendWorkingStatusDto } from '../../model/change-working-status/change-working-status.dto';
import { ApiResponseDto } from '../../model/common.dto';
import { BaseApiService } from '../base-api.service';

@Injectable({
  providedIn: 'root'
})
export class ChangeWorkingStatusService extends BaseApiService {

  changeUrl() {
    return "ChangeEmployeeWorkingStatus"
  }

  constructor(injector: Injector) {
    super(injector)
  }

  public ChangeWorkingStatusToQuit( input : ChangeStatusToQuitDto): Observable<ApiResponseDto<any>>{
    return this.processPost(`ChangeStatusToQuit`, input);
  }

  public ChangeWorkingStatusToPause( input : ChangeStatusToPauseDto): Observable<ApiResponseDto<any>>{
    return this.processPost(`ChangeStatusToPause`, input);
  }

  public ChangeWorkingStatusToMaternityLeave( input : ChangeStatusToMaternityLeaveDto): Observable<ApiResponseDto<any>>{
    return this.processPost(`ChangeStatusToMaterityLeave`, input);
  }

  public ChangeWorkingStatusToWorking( input: ChangeStatusWorkingDto): Observable<ApiResponseDto<any>>{
    return this.processPost(`ChangeStatusToWorking`, input);
  }

  public ExtendMaternityLeave( input : ExtendWorkingStatusDto): Observable<ApiResponseDto<any>>{
    return this.processPost(`ExtendMaternityLeave`, input);
  }

  public ExtendPausing( input : ExtendWorkingStatusDto): Observable<ApiResponseDto<any>>{
    return this.processPost(`ExtendPausing`, input);
  }

  public GetLatestSalaryChangeRequest(employeeId: number): Observable<ApiResponseDto<any>>{
    return this.processGet(`GetLatestSalaryChangeRequest?employeeId=${employeeId}`);
  }
}
