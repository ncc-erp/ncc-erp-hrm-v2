import { NotifyChannelDto } from './../../model/admin/configuration.dto';
import { Injectable, Injector } from '@angular/core';
import { ApiResponseDto } from '@app/service/model/common.dto';
import { Observable } from 'rxjs';
import { ConfigurationDto, EmailSettingDto, GetConnectResultDto, LoginConfigDto,WorkerAutoUpdateAllEmployeeInfoToOtherToolConfigDto } from '../../model/admin/configuration.dto';
import { BaseApiService } from '../base-api.service';

@Injectable({
  providedIn: 'root'
})
export class ConfigurationService extends BaseApiService {
  changeUrl() {
    return "Configuration"
  }

  constructor(injector: Injector) {
    super(injector)
  }
  public GetAllSetting(): Observable<ApiResponseDto<ConfigurationDto>> {
    return this.processGet("GetAllSetting");
  }

  public GetLoginSetting(): Observable<ApiResponseDto<LoginConfigDto>> {
    return this.processGet("GetLoginSetting");
  }

  public ChangeLoginSetting(input: LoginConfigDto): Observable<ApiResponseDto<string>> {
    return this.processPost("ChangeLoginSetting", input);
  }
  public ChangeWorkerAutoUpdateAllEmployeeInfoToOther(input: WorkerAutoUpdateAllEmployeeInfoToOtherToolConfigDto ) : Observable<ApiResponseDto<string>>{
    return this.processPost("ChangeWorkerAutoUpdateAllEmployeeInfoToOther", input);
  }
  public GetAutoUpdateAllEmployeeInfoToOther() : Observable<ApiResponseDto<WorkerAutoUpdateAllEmployeeInfoToOtherToolConfigDto>>{
    return this.processGet("GetAutoUpdateAllEmployeeInfoToOther");
  }
  public setEmailSetting(input: EmailSettingDto) {
    return this.processPost("SetEmailSetting", input)
  }

  public getEmailSetting():Observable<ApiResponseDto<EmailSettingDto>> {
    return this.processGet("GetEmailSetting")
  }
  
  public checkConnectToTimesheet():Observable<ApiResponseDto<GetConnectResultDto>> {
    return this.processGet("CheckConnectToTimesheet")
  }
  public checkConnectToTalent():Observable<ApiResponseDto<GetConnectResultDto>> {
    return this.processGet("CheckConnectToTalent")
  }
  public checkConnectToFinfast():Observable<ApiResponseDto<GetConnectResultDto>> {
    return this.processGet("CheckConnectToFinfast")
  }
  public checkConnectToProject():Observable<ApiResponseDto<GetConnectResultDto>> {
    return this.processGet("CheckConnectToProject")
  }
  public checkConnectToIMS():Observable<ApiResponseDto<GetConnectResultDto>> {
    return this.processGet("CheckConnectToIMS")
  }
  public getNotifySettings():Observable<ApiResponseDto<NotifyChannelDto>> {
    return this.processGet("GetNotifySettings")
  }
  public changeNotifyPlatform(payload: any):Observable<ApiResponseDto<any>> {
    return this.processPost(`ChangeNotifyPlatform`, payload)
  }
  public changeNotifySettings(payload: NotifyChannelDto):Observable<ApiResponseDto<any>> {
    return this.processPost(`ChangeNotifySettings`, payload)
  }

}
