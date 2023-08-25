import { Component, Injector, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { PERMISSIONS_CONSTANT } from '@app/permission/permission';
import { BenefitService } from '@app/service/api/benefits/benefit.service';
import { EmployeeService } from '@app/service/api/employee/employee.service';
import { PayslipService } from '@app/service/api/payslip/payslip.service';
import { BenefitOfEmployeeDto } from '@app/service/model/benefits/benefitEmployee.dto';
import { PayslipDetailByTypeDto } from '@app/service/model/payslip/payslip.dto';
import { AppComponentBase } from '@shared/app-component-base';
import { APP_ENUMS } from '@shared/AppEnums';
import { PagedListingComponentBase, PagedRequestDto } from '@shared/paged-listing-component-base';

@Component({
  selector: 'app-payslip-detail-benefit',
  templateUrl: './payslip-detail-benefit.component.html',
  styleUrls: ['./payslip-detail-benefit.component.css']
})
export class PayslipDetailBenefitComponent extends AppComponentBase implements OnInit {

  constructor(
    injector : Injector , 
    private route: ActivatedRoute, 
    public payslipService: PayslipService
    ) 
    { 
      super(injector)
    }

  public listPayslipDetailBenefits: PayslipDetailByTypeDto[] = [];
  public payslipId: number = 0;

  ngOnInit(): void {
    this.payslipId = Number(this.route.snapshot.queryParamMap.get('id'));
    this.getAllPayslipBenefit()

  }
  public getAllPayslipBenefit() {
    this.isLoading = true;
    this.subscription.push(
      this.payslipService.GetPayslipDetailByType(this.payslipId , APP_ENUMS.ESalaryType.Benefit).subscribe((rs) => {
        this.listPayslipDetailBenefits = rs.result;
        this.isLoading = false;
      },()=> this.isLoading = false)
    )
  }
  isViewTabBenefit(){
    return this.isGranted(PERMISSIONS_CONSTANT.Payroll_Payslip_PayslipDetail_TabBenefit_View);
  } 

}
