import { ConfigurationDto, EmailSettingDto, GetConnectResultDto, LoginConfigDto, WorkerAutoUpdateAllEmployeeInfoToOtherToolConfigDto, NotifyChannelDto } from './../../../service/model/admin/configuration.dto';
import { ConfigurationService } from './../../../service/api/admin/configuration.service';
import { AppComponentBase } from 'shared/app-component-base';
import { Component, Injector, OnInit } from '@angular/core';
import { PERMISSIONS_CONSTANT } from '@app/permission/permission';
import { interval } from 'rxjs';
import { takeWhile } from 'rxjs/operators';

@Component({
  selector: 'app-configuration',
  templateUrl: './configuration.component.html',
  styleUrls: ['./configuration.component.css']
})
export class ConfigurationComponent extends AppComponentBase implements OnInit {
  public configuration: ConfigurationDto = {} as ConfigurationDto;
  public notifyChannels = {} as NotifyChannelDto;
  public isEditLoginSetting: boolean = false;
  public isEditAutoUpdateSetting: boolean = false;
  public isEditNotifySetting: boolean = false;
  public loginSetting: LoginConfigDto = {} as LoginConfigDto;
  public WorkerAutoUpdateAllEmployeeInfoToOtherToolSetting: WorkerAutoUpdateAllEmployeeInfoToOtherToolConfigDto = {} as WorkerAutoUpdateAllEmployeeInfoToOtherToolConfigDto;
  public emailSetting: EmailSettingDto = {} as EmailSettingDto;
  public isEditEmailSetting: boolean = false;
  public  timesheetConnectResult: GetConnectResultDto = {} as GetConnectResultDto;
  public  talentConnectResult: GetConnectResultDto = {} as GetConnectResultDto;
  public  imsConnectResult: GetConnectResultDto = {} as GetConnectResultDto;
  public  projectConnectResult: GetConnectResultDto = {} as GetConnectResultDto;
  public  finfastConnectResult: GetConnectResultDto = {} as GetConnectResultDto;
  constructor(injector: Injector, private configService: ConfigurationService) {
    super(injector)
  }

  ngOnInit(): void {
    this.listBreadCrumb = [
      {name: '<i class="fa-solid fa-house fa-sm"></i>',url:''}, 
      {name: ' <i class="fa-solid fa-chevron-right"></i> '}, 
      {name:'Configurations'}]
    this.getAllConfig();
    this.getLoginSetting();
    this.getAutoUpdateAllEmployeeInfoToOther();
    this.getEmailSetting();
    this.getNotifyChannels();
    this.checkConnectToTimesheet();
    this.checkConnectToFinfast();
    this.checkConnectToTalent();
    this.checkConnectToIMS();
    this.checkConnectToProject();
  }

  isShowHRMSettingView(){
    return this.isGranted(PERMISSIONS_CONSTANT.Admin_Configuration_HRMSetting_View);
  }

  isAllowHRMSettingEdit(){
    return this.isGranted(PERMISSIONS_CONSTANT.Admin_Configuration_HRMSetting_Edit);
  }

  isShowProjectSettingView(){
    return this.isGranted(PERMISSIONS_CONSTANT.Admin_Configuration_ProjectSettingView);
  }

  isShowIMSSettingView(){
    return this.isGranted(PERMISSIONS_CONSTANT.Admin_Configuration_IMSSetting_View);
  }
  
  isShowFinfastSettingView(){
    return this.isGranted(PERMISSIONS_CONSTANT.Admin_Configuration_FinfastSetting_View);
  }

  isShowTimesheetSettingView(){
    return this.isGranted(PERMISSIONS_CONSTANT.Admin_Configuration_TimesheetSetting_View);
  }

  isShowTalentSettingView(){
    return this.isGranted(PERMISSIONS_CONSTANT.Admin_Configuration_TalentSetting_View);
  }

  isShowLoginSettingView(){
    return this.isGranted(PERMISSIONS_CONSTANT.Admin_Configuration_LoginSetting_View);
  }

  isAllowLoginSettingEdit(){
    return this.isGranted(PERMISSIONS_CONSTANT.Admin_Configuration_LoginSetting_Edit);
  }
  isShowAutoUpdateSettingView(){
    return this.isGranted(PERMISSIONS_CONSTANT .Admin_Configuration_WokerAutoUpdateAllEmployeeInfo_View);
  }

  isAllowAutoUpdateSettingEdit(){
    return this.isGranted(PERMISSIONS_CONSTANT .Admin_Configuration_WokerAutoUpdateAllEmployeeInfo_Edit);
  }








  getAllConfig() {
    this.subscription.push(
      this.configService.GetAllSetting().subscribe(rs => {
        this.configuration = rs.result;
      })
    )
  }

  public checkConnectToTimesheet(){
    this.timesheetConnectResult = {} as GetConnectResultDto;
    this.configService.checkConnectToTimesheet().subscribe((rs)=>{
      this.timesheetConnectResult = rs.result;
    })
  }

  public checkConnectToProject(){
    this.projectConnectResult =  {} as GetConnectResultDto;
    this.configService.checkConnectToProject().subscribe((rs)=>{
      this.projectConnectResult = rs.result;
    })
  }

  public checkConnectToFinfast(){
    this.finfastConnectResult =  {} as GetConnectResultDto;
    this.configService.checkConnectToFinfast().subscribe((rs)=>{
      this.finfastConnectResult = rs.result;
    })
  }

  public checkConnectToIMS(){
    this.imsConnectResult =  {} as GetConnectResultDto;
    this.configService.checkConnectToIMS().subscribe((rs)=>{
      this.imsConnectResult = rs.result;
    })
  }

  public checkConnectToTalent(){
    this.talentConnectResult = {} as GetConnectResultDto;
    this.configService.checkConnectToTalent().subscribe((rs)=>{
      this.talentConnectResult = rs.result;
    })
  }

  getNotifyChannels() {
    this.subscription.push(
      this.configService.getNotifySettings().subscribe(rs => {
        this.notifyChannels = rs.result
      })
    )
  }

  changeNotifyPlatform() {
    var changePlatform = {
      toPlatform : this.notifyChannels.notifyPlatform
    }
    this.subscription.push(
      this.configService.changeNotifyPlatform(changePlatform).subscribe(rs => {
        if (rs) {
          abp.notify.success(`Change notify to ${this.notifyChannels.notifyPlatform}`)
        }
        this.getNotifyChannels()
      })
    )
  }

  changeNotifySettings() {
    this.subscription.push(
      this.configService.changeNotifySettings(this.notifyChannels).subscribe(rs => {
        if (rs) {
          abp.notify.success(`Change setting successful!`)
        }
        this.getNotifyChannels()
      })
    )
  }

  get itChannelLabel(): string {
    if (this.notifyChannels.notifyPlatform == this.APP_CONST.NotifyToKomu) {
      return "HR-IT channelId"
    } 
    if (this.notifyChannels.notifyPlatform == this.APP_CONST.NotifyToMezon) {
      return "HR-IT channelURL"
    }
    return "HR-IT channel"
  }

  get payrollChannelLabel(): string {
    if (this.notifyChannels.notifyPlatform == this.APP_CONST.NotifyToKomu) {
      return "Payroll channelId"
    } 
    if (this.notifyChannels.notifyPlatform == this.APP_CONST.NotifyToMezon) {
      return "Payroll channelURL"
    }
    return "Payroll channel"
  }  
  
  changeLoginSetting(){
    this.subscription.push(
      this.configService.ChangeLoginSetting(this.loginSetting).subscribe(rs =>{
        if(rs){
          abp.notify.success("Change setting successful!");
        }
        this.getLoginSetting();
      })
    )
  }

  isEnableNormalLogin(value){
    this.loginSetting.enableNormalLogin = value.toString();
  }

  
  getLoginSetting(){
    this.subscription.push(
      this.configService.GetLoginSetting().subscribe(rs =>{
        this.loginSetting = rs.result;
      })
    )
  }
  changeWorkerAutoUpdateAllEmployeeInfoToOther(){
    this.subscription.push(
      this.configService.ChangeWorkerAutoUpdateAllEmployeeInfoToOther(this.WorkerAutoUpdateAllEmployeeInfoToOtherToolSetting).subscribe(rs =>{
        if(rs){
          abp.notify.success("Change setting successful!");
        }
        this.getAutoUpdateAllEmployeeInfoToOther();
      })
    )
  }
  onChangeEnableAutoUpdateAllEmployeeInfoToOther(value){
    this.WorkerAutoUpdateAllEmployeeInfoToOtherToolSetting.enableWorkerAutoUpdateAllEmployeeInfoToOtherToolSetting = value.toString();
  }
  getAutoUpdateAllEmployeeInfoToOther(){
    this.subscription.push(
      this.configService.GetAutoUpdateAllEmployeeInfoToOther().subscribe(sc =>{
        this.WorkerAutoUpdateAllEmployeeInfoToOtherToolSetting = sc.result;        
      })
    )
  }
  getEmailSetting(){
    this.subscription.push(
      this.configService.getEmailSetting().subscribe(rs => {
        this.emailSetting = rs.result
      })
    )
  }

  enableSSL(value) {
    this.emailSetting.enableSsl = value
  }

  changeEmailSetting() {
    this.configService.setEmailSetting(this.emailSetting).subscribe(rs => {
      abp.notify.success("Change Email setting successful")
    })
  }
  
  booleanConvert(value: string){
    if(value == "true") return true;
    else return false;
  }

  checkDefaultCredentials(value: string){
    this.emailSetting.useDefaultCredentials = value;
  }
  
}



