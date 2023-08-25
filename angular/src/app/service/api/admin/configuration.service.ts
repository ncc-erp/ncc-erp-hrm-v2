import { Injectable, Injector } from '@angular/core';
import { ApiResponseDto } from '@app/service/model/common.dto';
import { Observable } from 'rxjs';
import { ConfigurationDto, DiscordChannelDto, EmailSettingDto, GetConnectResultDto, LoginConfigDto,WorkerAutoUpdateAllEmployeeInfoToOtherToolConfigDto } from '../../model/admin/configuration.dto';
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

  public GetDiscordChannels():Observable<ApiResponseDto<DiscordChannelDto>> {
    return this.processGet("GetDiscordChannels")
  }

  public SetDiscordChannels(input: DiscordChannelDto) {
    return this.processPost("SetDiscordChannels", input)
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

}
