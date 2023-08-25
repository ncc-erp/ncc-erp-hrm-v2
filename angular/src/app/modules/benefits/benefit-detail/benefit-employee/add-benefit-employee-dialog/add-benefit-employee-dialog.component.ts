import { BenefitEmployeeDto } from '@app/service/model/benefits/benefitEmployee.dto';
import { AfterViewInit, Component, Inject, Injector, OnInit, ViewChild } from '@angular/core';
import { MatDialog, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { BenefitService } from '@app/service/api/benefits/benefit.service';
import { AddEmployeeToBenefitDto } from '@app/service/model/benefits/addEmployee.dto';
import { AddEmployeeComponent } from '@shared/components/employee/add-employee/add-employee.component';
import { DialogComponentBase } from '@shared/dialog-component-base';
import { NgModel } from '@angular/forms';
import { benefitDto } from '@app/service/model/benefits/beneft.dto';
import { AddSimpleDateValidator } from '@shared/helpers/date.validators'
@Component({
  selector: 'app-add-benefit-employee-dialog',
  templateUrl: './add-benefit-employee-dialog.component.html',
  styleUrls: ['./add-benefit-employee-dialog.component.css']
})
export class AddBenefitEmployeeDialogComponent extends DialogComponentBase<BenefitEmployeeDto> implements OnInit, AfterViewInit {
  @ViewChild('startDate', { static: true }) startDate: NgModel
  @ViewChild('endDate', { static: true }) endDate: NgModel
  public benefitEmployee = {} as AddEmployeeToBenefitDto;
  private addedBenefitEmployee: number[] = []
  private benefitId: number = 0

  constructor(injector: Injector,
    private benefitService: BenefitService,
    @Inject(MAT_DIALOG_DATA) public data,
    private dialog: MatDialog) {
    super(injector)
  }
  ngAfterViewInit(): void {
    AddSimpleDateValidator(this.startDate.control, this.endDate.control)
  }

  ngOnInit(): void {
    this.title = this.data.title
    this.benefitId = this.data.benefitId
    this.benefitEmployee.startDate = this.data.startDate
    this.benefitEmployee.endDate = null
  }

  stepUpdateNoteMoney() {
    this.addEmployee();
  }

  private async GetListEmployeeIdInBenefit() {
    await this.benefitService.GetListEmployeeIdInBenefit(this.benefitId).toPromise().then(rs => {
      this.addedBenefitEmployee = rs.result
    })
  }

  public async addEmployee() {
    await this.GetListEmployeeIdInBenefit();
    let ref = this.dialog.open(AddEmployeeComponent, {
      width: "92vw",
      height: "97vh",
      maxWidth: "100vw",
      data: {
        title: `Add employee to benefit: <strong>${this.data.benefitName}</strong>`,
        addedEmployeeIds: this.addedBenefitEmployee
      }
    })

    ref.afterClosed().subscribe((listBenefitId: number[]) => {
      if (listBenefitId && listBenefitId.length) {
        let input = {
          benefitId: this.benefitId,
          startDate: this.formatDateYMD(this.benefitEmployee.startDate),
          endDate: this.benefitEmployee.endDate ? this.formatDateYMD(this.benefitEmployee.endDate) : null,
          listEmployeeId: listBenefitId
        } as AddEmployeeToBenefitDto
        this.dialogRef.close(input)
      }
    })
  }
}

export interface AddBenefitEmployeeDialogData {
  benefit: benefitDto,
  benefitEmployee: BenefitEmployeeDto
}