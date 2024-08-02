import { ActivatedRoute, Router } from "@angular/router";
import { Component, OnInit, Injector } from "@angular/core";
import { AppComponentBase } from "@shared/app-component-base";
import { PayslipService } from "@app/service/api/payslip/payslip.service";
import { UserService } from "@app/service/api/user/user.service";
import { PERMISSIONS_CONSTANT } from "@app/permission/permission";
import { EmployeeService } from "@app/service/api/employee/employee.service";
import { MailPreviewInfo } from "@app/service/model/mail/mail.dto";
import { DomSanitizer } from "@angular/platform-browser";

@Component({
  selector: "app-payslip-detail-preview-link",
  templateUrl: "./payslip-detail-preview-link.component.html",
  styleUrls: ["./payslip-detail-preview-link.component.css"],
})
export class PayslipDetailPreviewLinkComponent
  extends AppComponentBase
  implements OnInit
{
  public payslipId: number;
  public mailInfo = new MailPreviewInfo();
  public deadline: string;
  public deadlineComfirmPayslip: string;
  public valid: boolean;
  public status: any;
  public fullName: string;
  public currentUserLoginFullName: string;
  public isLoadingPage: boolean = false;
  public isLoadingStatus: boolean = false;
  public isLoadingStatusQuit: boolean = false;
  public isValid: boolean = false;
  constructor(
    injector: Injector,
    private payslipService: PayslipService,
    private activatedRoute: ActivatedRoute,
    public sanitizer: DomSanitizer
  ) {
    super(injector);
  }
  ngOnInit(): void {
    this.payslipId = Number(
      this.activatedRoute.snapshot.queryParamMap.get("id")
    );
    this.getMailContent();
  }

  getMailContent() {
    if (this.isGranted(PERMISSIONS_CONSTANT.Admin)) {
      this.isLoadingPage = true;
      this.payslipService.getEmailTemplate(this.payslipId).subscribe((rs) => {
        this.mailInfo = rs.result.mailInfo;
        this.deadline = rs.result.deadline;
      });
    } else {
      this.payslipService
        .getDetaiPayslipTemplate(this.payslipId)
        .subscribe((rs) => {
          this.mailInfo = rs.result.mailInfo;
          this.deadline = rs.result.deadline;
          this.status = rs.result.status;
          this.fullName = rs.result.fullName;
          this.currentUserLoginFullName = rs.result.currentUserLoginFullName;
          this.valid = rs.result.valid;
          this.deadlineComfirmPayslip = this.convertDateTime(this.deadline);
          if (
            this.mailInfo &&
            this.deadline &&
            this.status !== 2 &&
            this.status !== 3 &&
            this.valid === true
          ) {
            return (this.isLoadingPage = true);
          }
          if (
            !this.mailInfo &&
            !this.deadline &&
            this.status === 2 &&
            this.valid === true
          ) {
            return (this.isLoadingStatus = true);
          }
          if (
            !this.mailInfo &&
            !this.deadline &&
            this.status === 3 &&
            this.valid === true
          ) {
            return (this.isLoadingStatusQuit = true);
          }
          
          if (this.valid === false) {
            return (this.isValid = true);
          }
        });
      }
    }
    convertDateTime(dateTimeString: string): string {
      const date = new Date(dateTimeString);
      
      const day = date.getDate().toString().padStart(2, "0");
      const month = (date.getMonth() + 1).toString().padStart(2, "0"); // Tháng bắt đầu từ 0
      const year = date.getFullYear();

    const hours = date.getHours().toString().padStart(2, "0");
    const minutes = date.getMinutes().toString().padStart(2, "0");

    return `${hours}:${minutes} ${day}/${month}/${year}`;
  }
}
