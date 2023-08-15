import { Component, Injector, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { PERMISSIONS_CONSTANT } from '@app/permission/permission';
import { EmployeeService } from '@app/service/api/employee/employee.service';
import { AppComponentBase } from '@shared/app-component-base';
import { BaseEmployeeDto, EmployeeBreadcrumbInfoDto } from '@shared/dto/user-infoDto';

@Component({
  selector: 'app-employee-detail',
  templateUrl: './employee-detail.component.html',
  styleUrls: ['./employee-detail.component.css']
})
export class EmployeeDetailComponent extends AppComponentBase implements OnInit, OnDestroy {
  public employeeId: number
  public employee: EmployeeBreadcrumbInfoDto
  public currentUrl: string = ""
  public isAllowEdit: boolean
  constructor(
    injector: Injector,
    private activatedRoute: ActivatedRoute,
    private employeeService: EmployeeService,
    private router: Router
  ) {
    super(injector)
  }

  ngOnInit(): void {
    this.activatedRoute.queryParams.subscribe((rs)=>{
      this.getEmployeeBasicInfo()
    })
  }


  isAllowViewTabPersonalInfo(){
    return this.isGranted(PERMISSIONS_CONSTANT.Employee_EmployeeDetail_TabPersonalInfo_View);
  }

  isAllowViewTabContract(){
    return this.isGranted(PERMISSIONS_CONSTANT.Employee_EmployeeDetail_TabContract_View);
  }

  isAllowViewTabDebt(){
    return this.isGranted(PERMISSIONS_CONSTANT.Employee_EmployeeDetail_TabDebt_View);
  }

  isAllowViewTabBenefit(){
    return this.isGranted(PERMISSIONS_CONSTANT.Employee_EmployeeDetail_TabBenefit_View);
  }
  isAllowViewTabBonus(){
    return this.isGranted(PERMISSIONS_CONSTANT.Employee_EmployeeDetail_TabBonus_View);
  }
  isAllowViewTabPunishment(){
    return this.isGranted(PERMISSIONS_CONSTANT.Employee_EmployeeDetail_TabPunishment_View);
  }
  isAllowViewTabSalaryHistory(){
    return this.isGranted(PERMISSIONS_CONSTANT.Employee_EmployeeDetail_TabSalaryHistory_View);
  }

  isAllowViewTabWorkingHistory(){
    return this.isGranted(PERMISSIONS_CONSTANT.Employee_EmployeeDetail_TabWorkingHistory_View);
  }

  isAllowViewTabBranchHistory(){
    return this.isGranted(PERMISSIONS_CONSTANT.Employee_EmployeeDetail_TabBranchHistory_View);
  }

  isAllowViewTabPayslipHistory(){
    return this.isGranted(PERMISSIONS_CONSTANT.Employee_EmployeeDetail_TabPayslipHistory_View);
  }

  getAvatar(member) {
    if(member.avatarFullPath) {
      return  member.avatarFullPath;
    }
    if(member.sex == this.APP_ENUM.Gender.Female ){
      return 'assets/img/women.png';
    }
    return 'assets/img/men.png';
  }
  getEmployeeBasicInfo(){
    this.employeeId = Number(this.activatedRoute.snapshot.queryParamMap.get('id'));
    this.currentUrl = this.router.url
    if (this.employeeId) {
      this.subscription.push(
        this.employeeService.GetEmployeeBasicInfoForBreadcrumb(this.employeeId).subscribe(rs => {
          this.employee = rs.result
        })
      )
    }
  }

  ngOnDestroy(){
    this.employeeId = null;
    this.employee = {} as EmployeeBreadcrumbInfoDto
  }
}
