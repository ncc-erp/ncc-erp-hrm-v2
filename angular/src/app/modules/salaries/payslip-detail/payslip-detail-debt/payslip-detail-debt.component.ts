import { Component, Injector, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { PERMISSIONS_CONSTANT } from '@app/permission/permission';
import { DebtService } from '@app/service/api/debt/debt.service';
import { PayslipService } from '@app/service/api/payslip/payslip.service';
import { DebtDto } from '@app/service/model/debt/debt.dto';
import { PayslipDetailByTypeDto } from '@app/service/model/payslip/payslip.dto';
import { AppComponentBase } from '@shared/app-component-base';
import { APP_ENUMS } from '@shared/AppEnums';
import { PagedListingComponentBase, PagedRequestDto } from '@shared/paged-listing-component-base';

@Component({
  selector: 'app-payslip-detail-debt',
  templateUrl: './payslip-detail-debt.component.html',
  styleUrls: ['./payslip-detail-debt.component.css']
})
export class PayslipDetailDebtComponent extends AppComponentBase implements OnInit {

  constructor(
    injector: Injector,
    public route: ActivatedRoute,
    private payslipService: PayslipService
    ) 
    { 
      super(injector)
    }
  public listPayslipDetailDebts: PayslipDetailByTypeDto[] = [];
  public payslipId: number = 0;

  ngOnInit(): void {
    this.payslipId = Number(this.route.snapshot.queryParamMap.get('id'));
    this.getAllPayslipDebt();
  }

  public getAllPayslipDebt() {
    this.isLoading = true;
    this.subscription.push(
      this.payslipService.GetPayslipDetailByType(this.payslipId , APP_ENUMS.ESalaryType.Debt).subscribe((rs) => {
        this.listPayslipDetailDebts = rs.result;
        this.isLoading = false;
      },()=> this.isLoading = false)
    )
  }

  isViewTabDebt(){
    return this.isGranted(PERMISSIONS_CONSTANT.Payroll_Payslip_PayslipDetail_TabDebt_View);
  } 

}
