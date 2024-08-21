import { Component, Injector, OnInit } from "@angular/core";
import { ActivatedRoute } from "@angular/router";
import { PayslipService } from "@app/service/api/payslip/payslip.service";
import { InputcomplainPayslipDto } from "@app/service/model/payslip/payslip.dto";
import { AppComponentBase } from "@shared/app-component-base";
import { PayslipToConfirm } from "../payslip-detail-preview-link/payslip-detail-preview-link.component";

@Component({
  selector: "app-payslip-to-confirm-mail-or-complain-mail",
  templateUrl: "./payslip-to-confirm-mail-or-complain-mail.component.html",
  styleUrls: ["./payslip-to-confirm-mail-or-complain-mail.component.css"],
})
export class PayslipToConfirmMailOrComplainMailComponent
  extends AppComponentBase
  implements OnInit
{
  public payslipId: number;
  public payslipToConfirm: string;
  public complainMessage: string = ""
  public uri: string;
  public result: string = ""
  public isSent = false
  public userLogin : PayslipToConfirm
  constructor(
    injector: Injector,
    private payslipService: PayslipService,
    private activatedRoute: ActivatedRoute
  ) {
    super(injector);
  }
  ngOnInit(): void {
    this.payslipId = Number(this.activatedRoute.snapshot.queryParamMap.get("id"));
    this.uri = this.activatedRoute.snapshot.url.map((segment) => segment.path).join("/");
    if (this.uri === "confirm-mail") {
      this.confirmPayslipMail();
    }else{
      this.getCurrentUser();
    }
    

  }
  send() {
    this.payslipService.complainPayslipMail(this.payslipId, this.complainMessage)
      .subscribe(rs => {
        this.isSent = true
        this.result = `<h3>${rs.result}</h3>`
        abp.notify.success("Complain send successful")
      })
  }
  public confirmPayslipMail() {
    this.payslipService.confirmPayslipMail(this.payslipId).subscribe((rs) => {
      this.payslipToConfirm = rs.result;
    });
  }
  getCurrentUser() {
    this.payslipService
      .getStatusEmployeeToComplain(this.payslipId)
      .subscribe((rs) => {
        this.userLogin = rs.result
      });
  }
}
