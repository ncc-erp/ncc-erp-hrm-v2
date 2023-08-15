import { Component, Inject, Injector, OnInit } from "@angular/core";
import { FormBuilder, FormGroup, Validators } from "@angular/forms";
import { MAT_DIALOG_DATA } from "@angular/material/dialog";
import { Router } from "@angular/router";
import { BranchService } from "@app/service/api/categories/branch.service";
import { JobPositionService } from "@app/service/api/categories/jobPosition.service";
import { LevelService } from "@app/service/api/categories/level.service";
import { SkillService } from "@app/service/api/categories/skill.service";
import { UserTypeService } from "@app/service/api/categories/userType.service";
import { WarningEmployeeService } from "@app/service/api/warning-employee/warning-employee.service";
import {
  TempEmployeeTalentDto,
  UpdateTempEmployeeTalentDto,
} from "@app/service/model/warning-employee/WarningEmployeeDto";
import { AppComponentBase } from "@shared/app-component-base";
import { SelectOptionDto } from "@shared/dto/selectOptionDto";
import { forkJoin } from "rxjs";

@Component({
  selector: "app-employee-detail-dialog",
  templateUrl: "./employee-detail-dialog.component.html",
  styleUrls: ["./employee-detail-dialog.component.css"],
})
export class EmployeeDetailDialogComponent
  extends AppComponentBase
  implements OnInit
{
  public employee: TempEmployeeTalentDto;

  public formGroup: FormGroup;

  public branchSelectOptions: SelectOptionDto[] = [];
  public levelSelectOptions: SelectOptionDto[] = [];
  public userTypeSelectOptions: SelectOptionDto[] = [];
  public jobPositionSelectOptions: SelectOptionDto[] = [];
  public skillSelectOptions: SelectOptionDto[] = [];
  public genderSelectOptions: SelectOptionDto[] = [];
  constructor(
    injector: Injector,
    @Inject(MAT_DIALOG_DATA) data: { employee: TempEmployeeTalentDto },
    private branchService: BranchService,
    private levelService: LevelService,
    private userTypeService: UserTypeService,
    private jobPositionService: JobPositionService,
    private skillService: SkillService,
    private warningEmployeeService: WarningEmployeeService,
    private router: Router
  ) {
    super(injector);
    router = injector.get(Router);
    this.employee = data.employee;
  }

  ngOnInit(): void {
    this.isLoading = true;
    this.initFrom();

    forkJoin({
      branch: this.branchService.getAll(),
      level: this.levelService.getAll(),
      userType: this.userTypeService.getAll(),
      jobPosition: this.jobPositionService.getAll(),
      skill: this.skillService.getAll(),
    }).subscribe(({ branch, level, userType, jobPosition, skill }) => {
      this.branchSelectOptions = this.fillSelectOption(branch.result);
      this.levelSelectOptions = this.fillSelectOption(level.result);
      this.userTypeSelectOptions = this.fillSelectOption(userType.result);
      this.jobPositionSelectOptions = this.fillSelectOption(jobPosition.result);
      this.skillSelectOptions = this.fillSelectOption(skill.result);
      this.getGenderSelectOption();
      this.fillForm();
      this.isLoading = false;
      this.formGroup.markAllAsTouched()
    });
  }

  public getRealSalary(isString: boolean) {
    const realSalary =
      this.formGroup.controls["salary"].value *
      (this.formGroup.controls["probationPercentage"].value / 100);
    return isString ? realSalary.toLocaleString() : realSalary;
  }

  private initFrom() {
    this.formGroup = new FormBuilder().group({
      fullName: ["", [Validators.required]],
      gender: [null, [Validators.required]],
      nccEmail: ["", [Validators.email, Validators.required]],
      phone: [],
      branchId: [null, [Validators.required]],
      dob: [""],
      levelId: [null, [Validators.required]],
      userType: [null, [Validators.required]],
      jobPositionId: [null, [Validators.required]],
      skills: [""],
      onboardDate: [null, [Validators.required]],
      salary: [null, [Validators.required]],
      probationPercentage: [null, [Validators.required]],
      personalEmail: [null, [Validators.email]],
    });
  }

  private fillForm() {
    this.formGroup.get("fullName").setValue(this.employee.fullName);
    this.formGroup.get("gender").setValue(this.employee.sex);
    this.formGroup.get("nccEmail").setValue(this.employee.nccEmail);
    this.formGroup.get("phone").setValue(this.employee.phone);
    this.formGroup.get("branchId").setValue(this.employee.branchId);
    this.formGroup.get("dob").setValue(this.employee.dateOfBirth);
    this.formGroup.get("levelId").setValue(this.employee.levelId);
    this.formGroup.get("userType").setValue(this.employee.userType);
    this.formGroup.get("jobPositionId").setValue(this.employee.jobPositionId);
    this.formGroup
      .get("skills")
      .setValue(
        this.skillSelectOptions.filter(({ key }) =>
          this.employee.skillStr?.includes(key)
        )
      );
    this.formGroup.get("onboardDate").setValue(this.employee.onboardDate);
    this.formGroup.get("salary").setValue(this.employee.salary);
    this.formGroup
      .get("probationPercentage")
      .setValue(this.employee.probationPercentage);
    this.formGroup.get("personalEmail").setValue(this.employee.email);
  }

  private fillSelectOption(value: any[]): SelectOptionDto[] {
    return value.map((x) => ({ key: x.name, value: x.id }));
  }

  private getGenderSelectOption() {
    this.genderSelectOptions = Object.entries(this.APP_ENUM.Gender).map(
      (value) => ({ key: value[0], value: value[1] })
    );
  }

  onSave() {
    const newTempEmployee: UpdateTempEmployeeTalentDto = {
      id: this.employee.id,
      fullName: this.formGroup.controls["fullName"].value,
      nccEmail: this.formGroup.controls["nccEmail"].value,
      phone: this.formGroup.controls["phone"].value,
      branchId: this.formGroup.controls["branchId"].value,
      dateOfBirth: this.formGroup.controls["dob"].value,
      levelId: this.formGroup.controls["levelId"].value,
      userType: this.formGroup.controls["userType"].value,
      jobPositionId: this.formGroup.controls["jobPositionId"].value,
      skills: this.formGroup.controls["skills"].value.map(({key}) => key).join(" "),
      onboardDate: this.formGroup.controls["onboardDate"].value,
      salary: this.formGroup.controls["salary"].value,
      probationPercentage: this.formGroup.controls["probationPercentage"].value,
      personalEmail: this.formGroup.controls["personalEmail"].value,
    };

    this.warningEmployeeService
      .UpdateTempEmployeeTalent(newTempEmployee)
      .subscribe(({ success, error }) => {
        if (success) {
          abp.message.success(
            `${this.employee.fullName} has been updated`,
            "Updated"
          );
        } else {
          abp.message.error(
            `${error}`,
            "Update fail"
          );
        }
      });
  }

  onDirect() {
    this.onSave();
    this.router.navigate(["app", "employees", "create"], {
      queryParams: {
        tempEmployeeId: this.employee.id,
      },
    });
  }

  canSave() : boolean {
    return this.formGroup.invalid;
  }
}
