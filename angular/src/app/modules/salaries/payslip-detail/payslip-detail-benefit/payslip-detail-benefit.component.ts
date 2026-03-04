import { Component, Injector, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { PERMISSIONS_CONSTANT } from '@app/permission/permission';
import { BenefitService } from '@app/service/api/benefits/benefit.service';
import { EmployeeService } from '@app/service/api/employee/employee.service';
import { PayslipService } from '@app/service/api/payslip/payslip.service';
import { BenefitOfEmployeeDto } from '@app/service/model/benefits/benefitEmployee.dto';
import { PayslipDetailByTypeDto, UpdatePayslipDetailDto } from '@app/service/model/payslip/payslip.dto';
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
  public isEditing: boolean = false;
  private payrollStatus: number;

  ngOnInit(): void {
    this.payslipId = Number(this.route.snapshot.queryParamMap.get('id'));
    this.payrollStatus = Number(this.route.snapshot.queryParamMap.get('status'));
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

  public onSave(ps: PayslipDetailByTypeDto){
    this.edit(ps);
  }

  public onUpdate(ps: PayslipDetailByTypeDto){
    ps.createMode = true;
    this.isEditing = true
  }

  public onCancel(ps: PayslipDetailByTypeDto){
    ps.createMode = false;
    this.isEditing = false;
    this.getAllPayslipBenefit();
  }

  public edit(ps: PayslipDetailByTypeDto){
    var input = {} as UpdatePayslipDetailDto;
    input.note = ps.note;
    input.money = ps.money;
    input.id = ps.id;

    this.subscription.push(
      this.payslipService.UpdatePayslipDetailBenefit(input).subscribe((rs) => {
        abp.notify.success(rs.result);
        ps.createMode = false;
        this.isEditing = false;
        this.getAllPayslipBenefit();
      },() => ps.createMode = true)
    )
  }

  isShowActions(){
    return this.payrollStatus == APP_ENUMS.PayrollStatus.New  || this.payrollStatus == APP_ENUMS.PayrollStatus.RejectedByKT;
  }

  isShowEditBenefit(){
    return this.isGranted(PERMISSIONS_CONSTANT.Payroll_Payslip_PayslipDetail_Edit);
  }
}
