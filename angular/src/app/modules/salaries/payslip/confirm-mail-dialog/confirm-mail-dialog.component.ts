import { SendMailOneemployeeDto } from '@app/service/model/mail/sendMail.dto';
import { MailDialogComponent, MailDialogData } from '@app/modules/admin/email-templates/mail-dialog/mail-dialog.component';
import { SendMailAllEmployeeDto } from './../../../../service/model/mail/sendMail.dto';
import { AppComponentBase } from '@shared/app-component-base';
import { MailService } from './../../../../service/api/mail/mail.service';
import { Component, OnInit, Injector, Inject } from '@angular/core';
import { PayslipService } from '@app/service/api/payslip/payslip.service';
import { MAT_DIALOG_DATA, MatDialogRef, MatDialog } from '@angular/material/dialog';
import { UpdatePayslipDeadLineDto } from '@app/service/model/payslip/payslip.dto';
import * as moment from 'moment';

@Component({
  selector: 'app-confirm-mail-dialog',
  templateUrl: './confirm-mail-dialog.component.html',
  styleUrls: ['./confirm-mail-dialog.component.css']
})
export class ConfirmMailDialogComponent extends AppComponentBase {
  public deadline: Date = new Date()
  public payslipId: number
  public title: string = ""

  constructor(private payslipService: PayslipService,
    @Inject(MAT_DIALOG_DATA) public data: any,
    private dialog: MatDialog,
    public dialogRef: MatDialogRef<ConfirmMailDialogComponent>,
    injector: Injector) {
    super(injector)
    this.payslipId = data.payslipId
    if (!this.payslipId) {
      this.title = "Send mail to all employee"
    }
    else {
      this.title = `Send mail to ${this.data.email}`
    }
  }

  send() {
    if (!this.payslipId) {
      this.sendMailAllEmployee()
    }
    else {
      this.sendMailOneEmployee()
    }
  }

  private sendMailAllEmployee() {
    let dto = {
      deadline: this.formatDateYMDHm(this.deadline),
      payrollId: this.data.payrollId
    } as SendMailAllEmployeeDto

    this.isLoading = true

    this.subscription.push(
      this.payslipService.sendMailToAllEmployee(dto).subscribe(rs => {
        abp.message.success(rs.result)
        this.dialogRef.close(true)
      },
        () => this.isLoading = false)
    )
  }

  private async sendMailOneEmployee() {
    let input = {
      deadline: this.formatDateYMDHm(this.deadline),
      payslipId: this.payslipId
    } as UpdatePayslipDeadLineDto

    await this.payslipService.updatePayslipDeadline(input).toPromise().then()

    this.subscription.push(
      this.payslipService.getEmailTemplate(this.payslipId).subscribe(rs => {
        const dialogData = {
          showEditButton: true,
          mailInfo: rs.result.mailInfo,
          showDialogHeader: false,
          deadline: this.deadline,
          showSendMailButton: true,
          showSendMailHeader: true
        }

        const ref = this.dialog.open(MailDialogComponent, {
          data: dialogData,
          width: '1600px',
          panelClass: 'email-dialog',
        })

        ref.afterClosed().subscribe(result => {
          if (result) {
            let dto = {
              payslipId: this.payslipId,
              mailContent: result,
              deadline: this.formatDateYMDHm(this.deadline)
            } as SendMailOneemployeeDto
            this.subscription.push(
              this.payslipService.sendMailToOneEmployee(dto).subscribe(response => {
                abp.message.success(`Mail sent to ${rs.result.mailInfo.sendToEmail}!`)
                this.dialogRef.close(true)
              })
            )
          }
        })
      })
    )
  }
}
