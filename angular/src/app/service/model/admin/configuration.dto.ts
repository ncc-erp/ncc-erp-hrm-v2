import { Oauth2Mezon } from './../../../../shared/AppConsts';


export interface ConfigurationDto {
    imsService: SettingDto,
    talentService: SettingDto,
    projectService: SettingDto,
    timesheetService: SettingDto,
    hrmV2Service: string,
    finfastService: SettingDto,
    komuService:KomuSettingDto,
    hrmService: SettingDto,
    oauth2Mezon: Oauth2Mezon,
    botHRM: BotHRMSetting,
}

export interface BotHRMSetting{
    nameBot: string,
    applicationToken: string,
    applicationId: string,
    urlAuthenticate: string,
    urlSentToken: string,
}
export interface Oauth2Mezon{
    client_Id: string,
    client_Secret: string,
    redirect_URI: string,
    grant_Type: string,
    url_Oauth2Mezon: string,
    url_UserInfo: string,
}
export interface SettingDto {
    baseAddress: string;
    securityCode: string;
}

export interface KomuSettingDto {
    baseAddress: string
    securityCode: string
    channelIdDevMode: string
    enableNoticeKomu: string
}

export interface AutoCreateUpdateDto {
    AutoCreateUpdateTimesheetUser: string
    AutoCreateUpdateIMSUser: string
    AutoCreateUpdateProjectUser: string
    AutoCreateUpdateTalentUser: string
}

export interface LoginConfigDto {
    googleClientId: string
    enableNormalLogin: boolean
    enableLoginMezon: boolean
    enableLoginGoogle: boolean
}
export interface WorkerAutoUpdateAllEmployeeInfoToOtherToolConfigDto {
    runAtHour : string
    enableWorkerAutoUpdateAllEmployeeInfoToOtherToolSetting : boolean
}
export interface EmailSettingDto {
    displayName: string
    defaultAddress: string
    host: string
    port: string
    userName: string
    password: string
    enableSsl: string
    useDefaultCredentials: string
}

export interface NotifyChannelDto {
    notifyPlatform: string,
    itChannel: string,
    payrollChannel: string,
    clanWebhookURL: string,  
}
export interface GetConnectResultDto {
    isConnected: boolean,
    message: string
}
