import { Component, Inject, Injector, OnInit } from "@angular/core";
import { MAT_DIALOG_DATA } from "@angular/material/dialog";
import { SkillService } from "@app/service/api/categories/skill.service";
import { EmployeeService } from "@app/service/api/employee/employee.service";
import { CreateUpdateEmployeeDto } from "@app/service/model/employee/employee.dto";
import { TempEmployeeTalentDto } from "@app/service/model/warning-employee/WarningEmployeeDto";
import { APP_ENUMS } from "@shared/AppEnums";
import { AppComponentBase } from "@shared/app-component-base";

@Component({
  selector: "app-multi-create-employee-from-temp",
  templateUrl: "./multi-create-employee-from-temp.component.html",
  styleUrls: ["./multi-create-employee-from-temp.component.css"],
})
export class MultiCreateEmployeeFromTempComponent
  extends AppComponentBase
  implements OnInit
{
  public displays: {
    employee: TempEmployeeTalentDto;
    isSuccess: boolean;
    error: string;
  }[];
  public skillList: { id: number; name: string; code: string }[];
  public numberOfLoaded: number;
  constructor(
    injector: Injector,
    @Inject(MAT_DIALOG_DATA) data: { employees: TempEmployeeTalentDto[] },
    private employeeService: EmployeeService,
    private skillService: SkillService
  ) {
    super(injector);
    this.displays = data.employees.map((employee) => ({
      employee: employee,
      isSuccess: null,
      error: null,
    }));
  }

  public onUpload() {
    this.numberOfLoaded = 0;
    this.displays.forEach((displayData) => {
      this.employeeService
        .createEmployee(
          this.getCreateDto(displayData.employee),
          displayData.employee.id
        )
        .subscribe(
          (rs) => {
            if (rs.success) {
              displayData.isSuccess = true;
            } else {
              displayData.isSuccess = false;
            }
            this.numberOfLoaded++;
          },
          (error) => {
            displayData.isSuccess = false;
            displayData.error = error;
            this.numberOfLoaded++;
          }
        );
    });
  }

  onRemove(id) {
    this.displays = this.displays.filter(
      (display) => display.employee.id != id
    );
  }

  public getProcess(): number {
    return (this.numberOfLoaded * 100) / this.displays.length;
  }

  private getCreateDto(employee: TempEmployeeTalentDto) {
    const createEmployee: CreateUpdateEmployeeDto = {
      id: 0,
      address: null,
      fullName: employee.fullName,
      birthday:
        employee.dateOfBirth != null
          ? this.formatDateYMD(employee.dateOfBirth)
          : "",
      userType: employee.userType,
      jobPositionId: employee.jobPositionId,
      levelId: employee.levelId,
      branchId: employee.branchId,
      idCard: null,
      issuedOn: null,
      issuedBy: null,
      phone: +employee.phone,
      bankAccountNumber: "",
      remainLeaveDay: 0,
      email: employee.nccEmail,
      probationPercentage:
        employee.userType == APP_ENUMS.UserType.ProbationaryStaff
          ? employee.probationPercentage
          : 100,
      salary: employee.salary,
      realSalary: (employee.salary * employee.probationPercentage) / 100,
      status: APP_ENUMS.UserStatus.Working,
      avatar: "",
      insuranceStatus: this.APP_ENUM.InsuranceStatus.NONE,
      placeOfPermanent: "",
      startWorkingDate: this.formatDateYMD(employee.onboardDate),
      taxCode: "",
      teams: [],
      skills: this.skillList
        .filter(({ name }) => employee.skillStr?.includes(name))
        .map(({ id }) => id),
      sex: employee.sex,
      bankId: null,
      contractStartDate: this.formatDateYMD(employee.onboardDate),
      contractEndDate: "",
      contractCode: "",
      personalEmail: employee.email,
    };

    this.trimData(createEmployee);

    return createEmployee;
  }

  ngOnInit(): void {
    this.skillService.getAll().subscribe((result) => {
      this.skillList = result.result.map(({ id, name, code }) => ({
        id,
        name,
        code,
      }));
    });
  }
}
