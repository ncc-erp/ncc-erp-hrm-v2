import { UpdatePayslipDeadlineDialogComponent } from './update-payslip-deadline-dialog/update-payslip-deadline-dialog.component';
import { UpdatePayslipDeadLineDto } from './../../../../service/model/payslip/payslip.dto';
import { Component, Injector, OnInit, ViewEncapsulation } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { DomSanitizer } from '@angular/platform-browser';
import { ActivatedRoute } from '@angular/router';
import { EditEmailDialogComponent, EditEmailDialogData } from '@app/modules/admin/email-templates/edit-email-dialog/edit-email-dialog.component';
import { PERMISSIONS_CONSTANT } from '@app/permission/permission';
import { PayslipService } from '@app/service/api/payslip/payslip.service';
import { MailPreviewInfo } from '@app/service/model/mail/mail.dto';
import { SendMailOneemployeeDto } from '@app/service/model/mail/sendMail.dto';
import { GetSalaryDetailDto } from '@app/service/model/payslip/GetSalaryDetailDto';
import { PayslipDetailDto, PaySlipDto } from '@app/service/model/payslip/payslip.dto';
import { AppComponentBase } from '@shared/app-component-base';
import { APP_ENUMS } from '@shared/AppEnums';
import { cloneDeep } from 'lodash-es';
import * as moment from 'moment';

@Component({
  selector: 'app-payslip-detail-preview',
  templateUrl: './payslip-detail-preview.component.html',
  styleUrls: ['./payslip-detail-preview.component.css'],
})
export class PayslipDetailPreviewComponent extends AppComponentBase implements OnInit {
  public payslipId: number
  public payslip: PaySlipDto = {} as PaySlipDto
  public payslipDetail: PayslipDetailDto = {} as PayslipDetailDto
  public payslipResults: GetSalaryDetailDto[] = []
  public totalRealSalary: number = 0
  public mailInfo = new MailPreviewInfo();
  public deadline: string
  public sending: boolean = false;
  private payrollStatus: number;

  constructor(
    injector: Injector,
    private payslipService: PayslipService,
    private activatedRoute: ActivatedRoute,
    public sanitizer: DomSanitizer,
    private dialog: MatDialog
  ) {
    super(injector)
  }
  ngOnInit(): void {
    this.payslipId = Number(this.activatedRoute.snapshot.queryParamMap.get('id'))
    this.payrollStatus = Number(this.activatedRoute.snapshot.queryParamMap.get('status'));

    this.getMailContent();
  }

  sendMail() {
    abp.message.confirm(`Send mai to ${this.mailInfo.sendToEmail}`, "", rs => {
      if (rs) {
        let dto = {
          payslipId: this.payslipId,
          mailContent: this.mailInfo,
          deadline: this.formatDateYMDHm(this.deadline)
        } as SendMailOneemployeeDto

        this.isLoading = true
        this.subscription.push(
          this.payslipService.sendMailToOneEmployee(dto).subscribe(response => {
            abp.message.success(`Mail sent to ${this.mailInfo.sendToEmail}`)
          },
            () => { this.isLoading = false })
        )
      }
    })

  }

  onEdit() {
    const editMailDialogData: EditEmailDialogData = {
      mailInfo: cloneDeep(this.mailInfo),
      showDialogHeader: false,
      temporarySave: true,
    }
    const dialogRef = this.dialog.open(EditEmailDialogComponent, {
      data: editMailDialogData,
      width: '1600px',
      panelClass: 'email-dialog'
    })
    dialogRef.afterClosed().subscribe(rs => {
      if (rs) {
        this.mailInfo = cloneDeep(rs)
      }
    })
  }


  getMailContent() {
    this.payslipService.getEmailTemplate(this.payslipId).subscribe(rs => {
      this.mailInfo = rs.result.mailInfo
      this.deadline = rs.result.deadline
    })
  }

  updateDeadline() {
    let ref = this.dialog.open(UpdatePayslipDeadlineDialogComponent, {
      width: "500px",
      data: {
        deadline: this.formatDateYMDHm(this.deadline),
        payslipId: this.payslipId
      }
    })
    ref.afterClosed().subscribe(rs => {
      if (rs) {
        this.getMailContent()
      }
    })
  }

  isViewTabPayslipPreview() {
    return this.isGranted(PERMISSIONS_CONSTANT.Payroll_Payslip_PayslipDetail_TabPayslipPreview_View);
  }
  isShowActions(){
    return this.payrollStatus  != APP_ENUMS.PayrollStatus.Executed;
  }
}
