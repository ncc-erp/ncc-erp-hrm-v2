import { SendMailOneemployeeDto } from '@app/service/model/mail/sendMail.dto';
import { MailDialogComponent, MailDialogData } from '@app/modules/admin/email-templates/mail-dialog/mail-dialog.component';
import { SendMailAllEmployeeDto } from '../../../../service/model/mail/sendMail.dto';
import { AppComponentBase } from '@shared/app-component-base';
import { MailService } from '../../../../service/api/mail/mail.service';
import { Component, OnInit, Injector, Inject } from '@angular/core';
import { PayslipService } from '@app/service/api/payslip/payslip.service';
import { MAT_DIALOG_DATA, MatDialogRef, MatDialog } from '@angular/material/dialog';
import { UpdatePayslipDeadLineDto } from '@app/service/model/payslip/payslip.dto';
import * as moment from 'moment';

@Component({
  selector: 'app-send-direct-message-dialog',
  templateUrl: './send-direct-message-touser.component.html',
  styleUrls: ['./send-direct-message-touser.component.css']
})
export class SendDirectMessageToUserComponent extends AppComponentBase {
  public deadline: Date = new Date()
  public payslipId: number
  public title: string = ""
  public userName : string =""

  constructor(private payslipService: PayslipService,
    @Inject(MAT_DIALOG_DATA) public data: any,
    private dialog: MatDialog,
    public dialogRef: MatDialogRef<SendDirectMessageToUserComponent>,
    injector: Injector) {
    super(injector)
    this.payslipId = data.payslipId
    if (!this.payslipId) {
      this.title = "Send Mezon direct message to all employees"
    }
    else {
       this.userName = data.email.split("@")[0];
      this.title = `Send Mezon direct message to ${this.userName}`
    }
   
  }

  send() {
    if (!this.payslipId) {
      this.sendDirectMessageAllEmployee()
    }
    else {
      this.sendDirectMessageToUser()
    }
  }

   sendDirectMessageAllEmployee() {
    let dto = {
      deadline: this.formatDateYMDHm(this.deadline),
      payrollId: this.data.payrollId
    } as SendMailAllEmployeeDto
    this.isLoading = true
    this.subscription.push(
      this.payslipService.sendDirectMessageToAllUser(dto).subscribe(
        (rs) => {
          abp.message.success(rs.result)
          this.dialogRef.close(true)
        },
        () => (this.isLoading = false)
      )
    );
  }


async sendDirectMessageToUser() {
    let input = {
      deadline: this.formatDateYMDHm(this.deadline),
      payslipId: this.payslipId,
    } as UpdatePayslipDeadLineDto;

    await this.payslipService.updatePayslipDeadline(input).toPromise().then();
    this.subscription.push(
           this.payslipService.sendDirectMessageToUser(this.payslipId).subscribe(rs =>{        
                abp.message.success(
                    `Mezon direct message sent do ${this.userName}!`
                  );
                this.dialogRef.close(true);         
           })
    )
  }
}
