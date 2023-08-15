import { Component, Injector, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { BenefitService } from '@app/service/api/benefits/benefit.service';
import { JobPositionService } from '@app/service/api/categories/jobPosition.service';
import { LevelService } from '@app/service/api/categories/level.service';
import { UserTypeService } from '@app/service/api/categories/userType.service';
import { ChangeWorkingStatusService } from '@app/service/api/change-working-status/change-working-status.service';
import { BenefitOfEmployeeDto } from '@app/service/model/benefits/benefitEmployee.dto';
import { JobPositionDto } from '@app/service/model/categories/jobPosition.dto';
import { LevelDto } from '@app/service/model/categories/level.dto';
import { UserTypeDto } from '@app/service/model/categories/userType.dto';
import { ChangeStatusToMaternityLeaveDto, ChangeStatusWorkingDto, lastestEmployeeInfoDto } from '@app/service/model/change-working-status/change-working-status.dto';
import { APP_ENUMS } from '@shared/AppEnums';
import { DialogComponentBase } from '@shared/dialog-component-base';
import * as moment from 'moment';
import { PersonalInfoComponent } from '../../personal-info.component';
import { ConfirmChangeStatusDialogComponent } from '../confirm-change-status-dialog/confirm-change-status-dialog.component';

@Component({
  selector: 'app-change-status-to-working',
  templateUrl: './change-status-to-working.component.html',
  styleUrls: ['./change-status-to-working.component.css']
})
export class ChangeStatusToWorkingComponent  extends DialogComponentBase<any> implements OnInit {

  public benefitsOfEmployee: BenefitOfEmployeeDto[] = [];
  public employeeInfo = {} as PersonalInfoComponent;
  public inputForChange = {} as ChangeStatusWorkingDto;
  public levelsList:LevelDto[] = [];
  public userTypeList: UserTypeDto[] = [];
  public userTypes:UserTypeDto[] = [];
  public positionList: JobPositionDto[] = [];
  public lastestEmployeeInfo = {} as lastestEmployeeInfoDto;
  public now = new Date();
  constructor
  (
    injector: Injector,
    public dialog: MatDialog,
    private benefitService: BenefitService,
    private changeWorkingStatusService: ChangeWorkingStatusService,
    private levelService: LevelService,
    private userTypeService: UserTypeService,
    private jobPositionService: JobPositionService
  ) {
    super(injector);
  }
  ngOnInit(): void {
    this.employeeInfo = this.dialogData.personalInfo;
    this.title = `Change working status of <b>${this.employeeInfo.fullName}</b> to <b class="text-danger">Working</b>`;
    this.getAllBenefitOfEmployee();
    this.inputForChange.toStatus = this.APP_ENUM.UserStatus.Working;
    this.inputForChange.hasContract = true;
    this.getAllLevel();
    this.getAllUserType();
    this.getAllJobPositon();
    this.getLastestEmployeeInfo();
  }

  public getAllLevel(){
    this.subscription.push(
      this.levelService.getAll().subscribe((rs)=>{
        this.levelsList = this.mapToFilter(rs.result, true);
      })
    )
  }

  public getAllUserType(){
    this.subscription.push(
      this.userTypeService.getAll().subscribe((rs)=>{
        this.userTypes = rs.result;
        this.userTypeList = this.mapToFilter(rs.result, true);
      })
    )
  }

  private getAllJobPositon() {
    this.subscription.push(this.jobPositionService.getAll().subscribe(rs => {
      this.positionList = this.mapToFilter(rs.result, true)
    }))
  }

  

  public getAllBenefitOfEmployee(){
    this.subscription.push(
      this.benefitService.GetAllBenefitsByEmployeeId(this.employeeInfo.id).subscribe((rs)=>{
        this.benefitsOfEmployee = rs.result;
        this.isLoading = false;
      })
    )
  }

  public getApplyDateValue(applyDate){
    this.benefitsOfEmployee.forEach((bf)=>{
      bf.startDate = this.formatDateYMD(applyDate);
      bf.endDate = null
    })
    this.inputForChange.contractEndDate= this.getContractEndDate(this.inputForChange.toUserType,applyDate);    
  }

  public onSelectNewUserType(event){
    this.inputForChange.contractEndDate = this.getContractEndDate(event.value, this.inputForChange.applyDate)
  }
  public selectWorkingStatus(event){
    this.benefitsOfEmployee.forEach((bf)=>{
      bf.endDate = null;
    })
    this.inputForChange.hasContract = true;
    this.inputForChange.probationPercentage = 100;  
  }

  public onCreateContract(event){
    this.inputForChange.hasContract = event.checked;
  }


  public getContractEndDate( newuserType: number, applyDate) {
    var userTypeInfo = this.userTypes.find(item => item.id == newuserType)

    if(userTypeInfo && userTypeInfo.contractPeriodMonth > 0 && this.inputForChange.applyDate != null){
      var endDate = moment(applyDate).startOf('day').add(userTypeInfo.contractPeriodMonth, 'month').subtract(1, 'day').toDate()
      return this.formatDateYMD(endDate);
    }
  
    return null;
  }

  public onChange(event){
    if(this.inputForChange.probationPercentage != 0){
      this.inputForChange.basicSalary = Math.round( Number(this.inputForChange.realSalary)* 100 /(this.inputForChange.probationPercentage));
    }
  }

  private setValueInput(){
    this.inputForChange.employeeId = this.employeeInfo.id;
    this.inputForChange.applyDate = this.formatDateYMD(this.inputForChange.applyDate);
    this.inputForChange.listCurrentBenefits = this.benefitsOfEmployee;
    this.inputForChange.contractEndDate = this.inputForChange.contractEndDate? this.formatDateYMD(this.inputForChange.contractEndDate): null;
    this.benefitsOfEmployee.forEach((bf)=>{
      if(bf.endDate){
        bf.endDate = this.formatDateYMD(bf.endDate);
        bf.startDate = this.formatDateYMD(bf.startDate);
      }
    });
    this.inputForChange.listCurrentBenefits = this.benefitsOfEmployee;
    if(this.inputForChange.toUserType != APP_ENUMS.UserType.ProbationaryStaff){
      this.inputForChange.probationPercentage = 100;
      this.inputForChange.basicSalary = this.calculateBasicSalary(this.inputForChange.realSalary,this.inputForChange.probationPercentage)
    }
  }

  public saveAndClose(){
    this.setValueInput();
    if(this.inputForChange.applyDate > this.formatDateYMD(this.now)){
      this.openDialogConfirm();
    }else{
      this.onWorking();
    }
    
  }

  public onWorking(){
    this.changeWorkingStatusService.ChangeWorkingStatusToWorking(this.inputForChange).subscribe((rs)=>{
      if(this.inputForChange.applyDate  <= this.formatDateYMD(this.now) || this.inputForChange.isConfirmed){
        abp.message.success(`Change working status of employee ${this.employeeInfo.fullName} successful`);
      }else{
        abp.message.success( `${this.employeeInfo.fullName}'s Status will be updated on ${this.inputForChange.applyDate}`)
      }
        this.dialogRef.close(true);
    })
  }

  public openDialogConfirm(){
    let dg = this.dialog.open(ConfirmChangeStatusDialogComponent,{
      data: {
        applyDate: this.inputForChange.applyDate
      },
      width: "400px"
    });
    dg.afterClosed().subscribe((rs)=>{
      if(rs){  
        this.inputForChange.isConfirmed = rs.isConfirmed;
        this.onWorking();
      }
      
    })
    
  }

  public getLastestEmployeeInfo(){
    this.subscription.push(
      this.changeWorkingStatusService.GetLatestSalaryChangeRequest(this.employeeInfo.id).subscribe((rs)=>{
        this.lastestEmployeeInfo = rs.result;
        this.setDefaultValue(this.lastestEmployeeInfo);
      })
    )
    this.setDefaultValue(this.lastestEmployeeInfo);
  }

  public setDefaultValue(lastestEmployeeInfo : lastestEmployeeInfoDto){

    this.inputForChange.applyDate  = lastestEmployeeInfo.applyDate;
    this.inputForChange.basicSalary = lastestEmployeeInfo.basicSalary;
    this.inputForChange.realSalary = lastestEmployeeInfo.realSalary;
    this.inputForChange.contractEndDate = lastestEmployeeInfo.contractEndDate;
    this.inputForChange.hasContract = lastestEmployeeInfo.hasContract;
    this.inputForChange.probationPercentage = lastestEmployeeInfo.probationPercentage;
    this.inputForChange.toLevelId = lastestEmployeeInfo.toLevelId;
    this.inputForChange.toJobPositionId = lastestEmployeeInfo.toJobPositionId;
    this.inputForChange.toUserType = lastestEmployeeInfo.toUserType;
    this.inputForChange.basicSalary = this.calculateBasicSalary(this.inputForChange.realSalary,this.inputForChange.probationPercentage);
  }

  public calculateBasicSalary(realSalary: number, probationPercentage){
    if(probationPercentage == 0 || probationPercentage == 100) return realSalary
    let basicSalary = Math.round(realSalary / ( probationPercentage / 100 ))
    return basicSalary;
  }
  

}
