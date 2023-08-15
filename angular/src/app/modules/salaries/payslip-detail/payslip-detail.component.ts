import { Component, Injector, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { PERMISSIONS_CONSTANT } from '@app/permission/permission';
import { EmployeeService } from '@app/service/api/employee/employee.service';
import { PayRollService } from '@app/service/api/pay-roll/pay-roll.service';
import { PaySlipDto } from '@app/service/model/payslip/payslip.dto';
import { PayRollDto } from '@app/service/model/salaries/salaries.dto';
import { AppComponentBase } from '@shared/app-component-base';
import { BaseEmployeeDto } from '@shared/dto/user-infoDto';
import * as moment from 'moment';


@Component({
  selector: 'app-payslip-detail',
  templateUrl: './payslip-detail.component.html',
  styleUrls: ['./payslip-detail.component.css']
})
export class PayslipDetailComponent extends AppComponentBase implements OnInit {

  public payslipId: number = 0;
  public payrollId: number = 0;
  public payroll = {} as PayRollDto;
  public employeeId: number = 0;
  public currentUrl: string = "";
  public listPayslips: PaySlipDto[] = [];
  public employee = {} as BaseEmployeeDto;
  private payrollStatus:number


  constructor(
    injector: Injector,
    public route: ActivatedRoute,
    public router: Router,
    private employeeService: EmployeeService,
    private payrollService: PayRollService
  ) {
    super(injector)
  }

  ngOnInit(): void {
    this.subscription.push(
      this.route.queryParamMap.subscribe(rs => {
        this.payrollId = Number(this.route.snapshot.queryParamMap.get('payrollId'));
        this.employeeId = Number(this.route.snapshot.queryParamMap.get('employeeId'));
        this.payslipId = Number(this.route.snapshot.queryParamMap.get('id'));
        this.payrollStatus = Number(this.route.snapshot.queryParamMap.get('status'));

        this.getPayrollById();
      })
    )
  }

  public onTabLinkClick() {
    this.currentUrl = this.router.url
  }

  public async getPayrollById() {
    let employeeName = (await this.employeeService.get(this.employeeId).toPromise().then()).result.employeeInfo.fullName;
    this.subscription.push(
      this.payrollService.get(this.payrollId).subscribe((data) => {
        this.payroll = data.result;
        this.listBreadCrumb = [
          { name: '<i class="fa-solid fa-house fa-sm"></i>', url: '' },
          { name: ' <i class="fa-solid fa-chevron-right"></i> ' },
          { name: ' Payroll ', url: '/app/payroll/list-payroll' },
          { name: ' <i class="fa-solid fa-chevron-right"></i> ' },
          { name: `Payroll -  ${moment(this.payroll.applyMonth).format('MM/YYYY')} `, url: '/app/payroll/list-payroll/payroll-detail', queryParams: { id: this.payrollId } },
          { name: ' <i class="fa-solid fa-chevron-right"></i> ' },
          { name: `<b>Payslip: </b>`},
          { name: `<b><span class='text-danger red-link'>${employeeName}</span></b>`, url: `/app/employees/list-employee/employee-detail/payslip-history`, queryParams: {id: this.employeeId}},
          { name: ` - <b class='text-success'>${moment(this.payroll.applyMonth).format('MM/YYYY')}</b>`}
        ]
      })
    )
  }
  isViewTabSalary() {
    return this.isGranted(PERMISSIONS_CONSTANT.Payroll_Payslip_PayslipDetail_TabSalary_View);
  }
  isViewTabDebt() {
    return this.isGranted(PERMISSIONS_CONSTANT.Payroll_Payslip_PayslipDetail_TabDebt_View);
  }
  isViewTabBenefit() {
    return this.isGranted(PERMISSIONS_CONSTANT.Payroll_Payslip_PayslipDetail_TabBenefit_View);
  }
  isViewTabBonus() {
    return this.isGranted(PERMISSIONS_CONSTANT.Payroll_Payslip_PayslipDetail_TabBonus_View);
  }

  isViewTabPunishment() {
    return this.isGranted(PERMISSIONS_CONSTANT.Payroll_Payslip_PayslipDetail_TabPunishment_View);
  }

  isViewTabPayslipPreview() {
    return this.isGranted(PERMISSIONS_CONSTANT.Payroll_Payslip_PayslipDetail_TabPayslipPreview_View);
  }
}
