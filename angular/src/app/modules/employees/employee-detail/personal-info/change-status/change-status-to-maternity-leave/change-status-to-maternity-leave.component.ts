import { Component, Injector, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { BenefitService } from '@app/service/api/benefits/benefit.service';
import { ChangeWorkingStatusService } from '@app/service/api/change-working-status/change-working-status.service';
import { BenefitOfEmployeeDto } from '@app/service/model/benefits/benefitEmployee.dto';
import { ChangeStatusToMaternityLeaveDto, ChangeStatusToPauseDto } from '@app/service/model/change-working-status/change-working-status.dto';
import { DialogComponentBase } from '@shared/dialog-component-base';
import * as moment from 'moment';
import { PersonalInfoComponent } from '../../personal-info.component';
import { ConfirmChangeStatusDialogComponent } from '../confirm-change-status-dialog/confirm-change-status-dialog.component';

@Component({
  selector: 'app-change-status-to-maternity-leave',
  templateUrl: './change-status-to-maternity-leave.component.html',
  styleUrls: ['./change-status-to-maternity-leave.component.css']
})
export class ChangeStatusToMaternityLeaveComponent extends DialogComponentBase<any> implements OnInit {

  public benefitsOfEmployee: BenefitOfEmployeeDto[] = [];
  public employeeInfo = {} as PersonalInfoComponent;
  public inputForChange = {} as ChangeStatusToMaternityLeaveDto;
  public now = new Date();
  constructor
  (
    injector: Injector,
    public dialog: MatDialog,
    private benefitService: BenefitService,
    private changeWorkingStatusService: ChangeWorkingStatusService
  ) {
    super(injector);
  }
  ngOnInit(): void {
    this.employeeInfo = this.dialogData.personalInfo;
    this.title = `Change working status of <b>${this.employeeInfo.fullName}</b> to <b class="text-danger">Maternity Leave</b>`;
    this.getAllBenefitOfEmployee();
    this.inputForChange.toStatus = this.APP_ENUM.UserStatus.MaternityLeave;
  }

  public getAllBenefitOfEmployee(){
    this.subscription.push(
      this.benefitService.GetAllBenefitsByEmployeeId(this.employeeInfo.id).subscribe((rs)=>{
        this.benefitsOfEmployee = rs.result;
        this.isLoading = false;
      })
    )
  }

  public onChangeApplyDateValue(value){
    this.benefitsOfEmployee.forEach((bf)=>{
      bf.endDate = bf.endDate? this.formatDateYMD(value) : null;;
    })
    this.inputForChange.backDate = moment(value).startOf('day').add(6, 'months').format("YYYY-MM-DD");
  }

  public setInputValue(){
    this.inputForChange.applyDate = this.formatDateYMD(this.inputForChange.applyDate);
    this.inputForChange.backDate = this.formatDateYMD(this.inputForChange.backDate);
    this.inputForChange.employeeId = this.employeeInfo.id;
    this.benefitsOfEmployee.forEach((bf)=>{
      bf.endDate = bf.endDate? this.formatDateYMD(bf.endDate) : null;
    })
    this.inputForChange.listCurrentBenefits = this.benefitsOfEmployee;
  }

  public saveAndClose(){
    this.setInputValue();
    if(this.inputForChange.applyDate > this.formatDateYMD(this.now)){
      this.openDialogConfirm();
    }else{
      this.onMaternityLeave();
    }
    
  }

  onMaternityLeave(){
    this.changeWorkingStatusService.ChangeWorkingStatusToMaternityLeave(this.inputForChange).subscribe((rs)=>{
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
        this.onMaternityLeave();
      }
      
    })
    
  }

}
