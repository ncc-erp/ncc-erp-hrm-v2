import { ActivatedRoute, Router } from "@angular/router";
import { Component, OnInit, Injector } from "@angular/core";
import { AppComponentBase } from "@shared/app-component-base";
import { PayslipService } from "@app/service/api/payslip/payslip.service";
import { UserService } from "@app/service/api/user/user.service";
import { PERMISSIONS_CONSTANT } from "@app/permission/permission";
import { EmployeeService } from "@app/service/api/employee/employee.service";
import { MailPreviewInfo } from "@app/service/model/mail/mail.dto";
import { DomSanitizer } from "@angular/platform-browser";
import { CheckValidType, EmailFunc } from "@shared/AppEnums";

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
  public payslipDetail: PaysslipToConfirm;
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
    this.payslipService
      .getPayslipPreviewToConfirm(this.payslipId)
      .subscribe((rs) => {
        this.payslipDetail = rs.result
      });
  }
}
export interface PaysslipToConfirm {
  deadline: number;
  mailInfo: MailPreviewInfoDto;
  checkValidType: CheckValidType;
  message: string;
}

export interface MailPreviewInfoDto {
  templateId: number;
  name: string;
  description: string;
  mailFuncType: EmailFunc;
  subject: string;
  bodyMessage: string;
  sendToEmail: string;
  currentUserLoginId: number;
  tenantId: number;
}
