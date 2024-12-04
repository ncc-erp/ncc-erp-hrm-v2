

export interface ConfigurationDto {
    imsService: SettingDto,
    talentService: SettingDto,
    projectService: SettingDto,
    timesheetService: SettingDto,
    hrmV2Service: string,
    finfastService: SettingDto,
    komuService:KomuSettingDto,
    hrmService: SettingDto
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
    sendDMToMezon: string    
}
export interface GetConnectResultDto {
    isConnected: boolean,
    message: string
}
