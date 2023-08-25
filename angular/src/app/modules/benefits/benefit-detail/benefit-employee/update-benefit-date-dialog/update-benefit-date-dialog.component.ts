import { UpdateEmployeeDateDto } from './../../../../../service/model/benefits/updateEmployeeDate.dto';
import { Component, Injector, OnInit } from '@angular/core';
import { BenefitService } from '@app/service/api/benefits/benefit.service';
import { DialogComponentBase } from '@shared/dialog-component-base';
import { updateDateType } from '@app/service/model/benefits/benefitEmployee.dto';

@Component({
  selector: 'app-update-benefit-date-dialog',
  templateUrl: './update-benefit-date-dialog.component.html',
  styleUrls: ['./update-benefit-date-dialog.component.css']
})
export class UpdateBenefitDateDialogComponent extends DialogComponentBase<any> implements OnInit {

  public date: string
  private benefitId: number
  public updateType: number
  public EUpdateDateType = updateDateType
  constructor(injector: Injector, private benefitService: BenefitService) {
    super(injector)
  }

  ngOnInit(): void {
    this.benefitId = this.dialogData.benefitId
    this.title = this.dialogData.title
    this.updateType = this.dialogData.type
  }

  saveAndClose() {
    this.date = this.date ? this.formatDateYMD(this.date) : null
    let input = {
      benefitId: this.benefitId,
      date: this.date
    } as UpdateEmployeeDateDto
    if (this.dialogData.type === updateDateType.startDate) {
      this.subscription.push(this.benefitService.UpdateAllStartDate(input).subscribe(rs => {
        this.dialogRef.close(true)
        abp.notify.success(`Update start date to ${this.date}`)
      }))
    }
    else {
      this.subscription.push(this.benefitService.UpdateAllEndDate(input).subscribe(rs => {
        this.dialogRef.close(true)
        abp.notify.success(`Update end date to ${this.date}`)
      }))
    }
  }
}
