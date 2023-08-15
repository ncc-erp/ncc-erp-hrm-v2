import { Component, Injector, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { BenefitService } from '@app/service/api/benefits/benefit.service';
import { ChangeWorkingStatusService } from '@app/service/api/change-working-status/change-working-status.service';
import { BenefitOfEmployeeDto } from '@app/service/model/benefits/benefitEmployee.dto';
import { ChangeStatusToPauseDto, ExtendWorkingStatusDto } from '@app/service/model/change-working-status/change-working-status.dto';
import { DialogComponentBase } from '@shared/dialog-component-base';
import { PersonalInfoComponent } from '../../personal-info.component';

@Component({
  selector: 'app-extend-maternity-leave',
  templateUrl: './extend-maternity-leave.component.html',
  styleUrls: ['./extend-maternity-leave.component.css']
})
export class ExtendMaternityLeaveComponent extends DialogComponentBase<any> implements OnInit {

  public benefitsOfEmployee: BenefitOfEmployeeDto[] = [];
  public employeeInfo = {} as PersonalInfoComponent;
  public inputForChange = {} as ExtendWorkingStatusDto;
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
    this.title = `Extend maternity leave for <b>${this.employeeInfo.fullName}`;
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


  public setInputValue(){
    this.inputForChange.backDate = this.formatDateYMD(this.inputForChange.backDate);
    this.inputForChange.employeeId = this.employeeInfo.id;
    this.benefitsOfEmployee.forEach((bf)=>{
      bf.endDate = bf.endDate? this.formatDateYMD(bf.endDate) : null;
      bf.startDate = this.formatDateYMD(bf.startDate);
    })
    this.inputForChange.listCurrentBenefits =  this.benefitsOfEmployee;
  }

  public saveAndClose(){
    this.setInputValue();
    this.changeWorkingStatusService.ExtendMaternityLeave(this.inputForChange).subscribe((rs)=>{
        abp.notify.success(`Change working of employee ${this.employeeInfo.fullName} successful`);
        this.dialogRef.close(true);
    })
  }


}
