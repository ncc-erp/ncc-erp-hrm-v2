import { PayRollDto, CreatePayRollDto, UpdatePayrollDto } from './../../../../service/model/salaries/salaries.dto';
import { Component, OnInit, Injector } from '@angular/core';
import { DialogComponentBase } from '@shared/dialog-component-base';
import * as moment from 'moment';
import { PayRollService } from '@app/service/api/pay-roll/pay-roll.service';

@Component({
  selector: 'app-add-edit-pay-roll',
  templateUrl: './add-edit-pay-roll.component.html',
  styleUrls: ['./add-edit-pay-roll.component.css']
})
export class AddEditPayRollComponent extends DialogComponentBase<any> implements OnInit {

  constructor(injector: Injector , private payRollService: PayRollService) {super(injector) }
  public editPayRollData = {} as PayRollDto;
  public createPayRollData = {} as CreatePayRollDto;
  public currentDate = new Date();
  public currentYear = this.currentDate.getFullYear();
  public currentMonth = this.currentDate.getMonth();
  public listYears = [];
  public listMonths = [];

  ngOnInit(): void {
    if (this.dialogData?.payRoll) {
      this.editPayRollData = this.dialogData.payRoll;
      this.title = `Edit Payroll: <strong>${moment(this.editPayRollData.applyMonth).format('MM/YYYY')}</strong>`;
    } else {
      this.title = "Create new Payroll";
      this.createPayRollData.year = this.currentYear;
      this.createPayRollData.month = this.currentMonth;
      this.createPayRollData.standardOpenTalk = 2
    }
    this.getListMonthAndYears();
  }
  private getListMonthAndYears(){
    for (let y = this.currentYear - 1; y <= this.currentYear + 3; y++) {
      this.listYears.push(y)
    }
    for(let m = 1; m <= 12; m++){
      this.listMonths.push(m);
    }
  }

  public saveAndClose(){
    this.isLoading = true;
    if(this.dialogData?.payRoll){
      let updatePayrollDto = {
        id: this.editPayRollData.id,
        applyMonth: this.formatDateYMD(this.editPayRollData.applyMonth),
        normalWorkingDay: this.editPayRollData.standardWorkingDay,
        status: this.editPayRollData.status,
        openTalk: this.editPayRollData.standardOpentalk
      } as UpdatePayrollDto
      this.subscription.push(
        this.payRollService.update(updatePayrollDto).subscribe((rs)=>{
          abp.notify.success(`Update pay roll successful`)
          this.isLoading = false;
          this.dialogRef.close(true)
        },()=> this.isLoading = false)
      )
    }else{
      this.subscription.push(
        this.payRollService.create(this.createPayRollData).subscribe((rs)=>{
          abp.notify.success(`Create new pay roll successful`)
          this.isLoading = false;
          this.dialogRef.close(true)
        },()=> this.isLoading = false)
      )
    }
  }

}
