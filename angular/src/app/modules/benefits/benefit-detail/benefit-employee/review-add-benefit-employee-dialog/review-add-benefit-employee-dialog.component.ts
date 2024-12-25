import { Component, Injector, OnInit, ViewChild } from '@angular/core';
import { MatAccordion } from '@angular/material/expansion';
import { BenefitService } from '@app/service/api/benefits/benefit.service';
import { EmployeeService } from '@app/service/api/employee/employee.service';
import { BenefitOfEmployeeDto } from '@app/service/model/benefits/benefitEmployee.dto';
import { DialogComponentBase } from '@shared/dialog-component-base';
import { BadgeInfoDto } from '@shared/dto/user-infoDto';


@Component({
  selector: "app-review-add-benefit-employee-dialog",
  templateUrl: "./review-add-benefit-employee-dialog.component.html",
  styleUrls: ["./review-add-benefit-employee-dialog.component.css"],
})
export class ReviewAddBenefitEmployeeDialogComponent
  extends DialogComponentBase<any>
  implements OnInit
{
  public employeeIds = [];
  public newBenefitId: number;
  public employeesInfo = [] as EmployeeBenefitsInfo[];
  public headerTitle = ''
  @ViewChild(MatAccordion)accordion: MatAccordion

  constructor(
    injector: Injector,
    private benefitService: BenefitService,
    private employeeService: EmployeeService
  ) {
    super(injector);
    this.dialogRef.disableClose = true
    this.employeeIds = this.dialogData.listEmployeeId;
    this.newBenefitId = this.dialogData.newBenefitId;
    this.headerTitle = this.dialogData.headerTitle
  }

  ngOnInit(): void {    
    this.getEmployeeBenefits()
  }

  getEmployeeBenefits() {    
    this.subscription.push(
      this.benefitService.GetEmployeeBenefits(this.employeeIds)
        .subscribe(rs => {
          this.employeesInfo = rs.result
          console.log(this.employeesInfo)
        })
    )
  }
}

export interface EmployeeBenefitsInfo {
  id: number,
  fullName: string,
  sex: number,
  avatarFullPath: string;
  email: string,
  branchInfo: BadgeInfoDto,
  userTypeInfo: BadgeInfoDto,
  jobPositionInfo: BadgeInfoDto,
  levelInfo: BadgeInfoDto,
  benefits: BenefitOfEmployeeDto[]
}