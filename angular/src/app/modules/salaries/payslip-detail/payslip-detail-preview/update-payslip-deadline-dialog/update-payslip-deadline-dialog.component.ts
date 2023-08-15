import { PayslipService } from './../../../../../service/api/payslip/payslip.service';
import { AppComponentBase } from '@shared/app-component-base';
import { Component, OnInit, Injector, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { UpdatePayslipDeadLineDto } from '@app/service/model/payslip/payslip.dto';
import * as moment from 'moment';

@Component({
  selector: 'app-update-payslip-deadline-dialog',
  templateUrl: './update-payslip-deadline-dialog.component.html',
  styleUrls: ['./update-payslip-deadline-dialog.component.css']
})
export class UpdatePayslipDeadlineDialogComponent extends AppComponentBase implements OnInit {
  public deadline: string = ""
  private payslipId: number
  constructor(injector: Injector, @Inject(MAT_DIALOG_DATA) public data: any, private dialogRef: MatDialogRef<UpdatePayslipDeadLineDto>,
    private payslipService: PayslipService) {
    super(injector)
    this.deadline = data.deadline
    this.payslipId = data.payslipId
  }

  ngOnInit(): void {
  }

  save() {
    let dto = {
      deadline: this.formatDateYMDHm(this.deadline),
      payslipId: this.payslipId
    } as UpdatePayslipDeadLineDto
    this.isLoading = true
    this.payslipService.updatePayslipDeadline(dto).subscribe(rs => {
      abp.notify.success("Deadline updated")
      this.dialogRef.close(true)
    },
      () => { this.isLoading = false })
  }
}
