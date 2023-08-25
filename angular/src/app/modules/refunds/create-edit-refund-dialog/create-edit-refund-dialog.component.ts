import { RefundDto } from './../../../service/model/refunds/refund.dto';
import { Injector } from '@angular/core';
import { Component, OnInit } from '@angular/core';
import { RefundsService } from '@app/service/api/refunds/refunds.service';
import { CreateRefundDto } from '@app/service/model/refunds/refund.dto';
import { DialogComponentBase } from '@shared/dialog-component-base';
import * as moment from 'moment';

@Component({
  selector: 'app-create-edit-refund-dialog',
  templateUrl: './create-edit-refund-dialog.component.html',
  styleUrls: ['./create-edit-refund-dialog.component.css']
})
export class CreateEditRefundDialogComponent extends DialogComponentBase<RefundDto> implements OnInit {

  public refund = {
    date: moment(new Date()).format("MM/YYYY")
  } as RefundDto
  public listDate: string[] = []
  public title:string = ""

  constructor(injector: Injector, private refundService: RefundsService) {
    super(injector)
    if(this.dialogData?.id){
      this.refund = this.dialogData
      this.title = `Edit refund: <strong>${this.refund.name}</strong>`
    }
    else{
      this.title = "Created new refund"
    }
  }

  ngOnInit(): void {
    this.getListRefundDate();
  }

  public saveAndClose() {
    this.trimData(this.refund)
    if (!this.dialogData?.id) {
      this.subscription.push(
        this.refundService.create(this.refund).subscribe((res) => {
          abp.notify.success(`Created new refund: ${this.refund.name}`)
          this.dialogRef.close(true)
        }))
    } else {
      this.subscription.push(
        this.refundService.update(this.refund).subscribe((res) => {
          abp.notify.success(`Edit Refund successful`)
          this.dialogRef.close(true)
        }))
    }
  }

  private getListRefundDate() {
    this.subscription.push(
    this.refundService.GetListMonthsForCreate().subscribe(rs => {
      this.listDate = rs.result.map(x => moment(x).format("MM/YYYY"))
    }))
  }
}
