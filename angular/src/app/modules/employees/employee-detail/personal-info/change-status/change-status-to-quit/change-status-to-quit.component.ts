import { Component, Injector, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { BenefitService } from '@app/service/api/benefits/benefit.service';
import { BenefitOfEmployeeDto } from '@app/service/model/benefits/benefitEmployee.dto';
import { DialogComponentBase } from '@shared/dialog-component-base';
import { PersonalInfoComponent } from '../../personal-info.component';
import { ChangeStatusToQuitDto } from '@app/service/model/change-working-status/change-working-status.dto'
import {ChangeWorkingStatusService} from '@app/service/api/change-working-status/change-working-status.service'
import { AppConsts } from '@shared/AppConsts';
import { ConfirmChangeStatusDialogComponent } from '../confirm-change-status-dialog/confirm-change-status-dialog.component';
import * as moment from 'moment';
@Component({
  selector: 'app-change-status-to-quit',
  templateUrl: './change-status-to-quit.component.html',
  styleUrls: ['./change-status-to-quit.component.css']
})
export class ChangeStatusToQuitComponent extends DialogComponentBase<any> implements OnInit {

  public benefitsOfEmployee: BenefitOfEmployeeDto[] = [];
  public employeeInfo = {} as PersonalInfoComponent;
  public inputForChange = {} as ChangeStatusToQuitDto;
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
    this.title = `Change working status of <b>${this.employeeInfo.fullName}</b> to <b class="text-danger">Quit</b>`;
    this.getAllBenefitOfEmployee();
    this.inputForChange.toStatus = this.APP_ENUM.UserStatus.Quit;
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
      bf.endDate = value? this.formatDateYMD(value) : null;
    })
  }

  public setInputValue(){
    this.inputForChange.applyDate = this.formatDateYMD(this.inputForChange.applyDate);
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
      this.onQuit();
    }
  }
  public onQuit(){
    this.changeWorkingStatusService.ChangeWorkingStatusToQuit(this.inputForChange).subscribe((rs)=>{
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
        this.onQuit();
      }
      
    })
    
  }

}
