import { PERMISSIONS_CONSTANT } from './../../../permission/permission';
import { BranchService } from './../../../service/api/categories/branch.service';
import { JobPositionService } from './../../../service/api/categories/jobPosition.service';
import { UserTypeDto } from '@app/service/model/categories/userType.dto';
import { Router } from '@angular/router';
import { finalize } from 'rxjs/operators';
import { PagedListingComponentBase, PagedRequestDto } from '@shared/paged-listing-component-base';
import { Component, OnInit, Injector } from '@angular/core';
import { WarningEmployeeService } from '@app/service/api/warning-employee/warning-employee.service';
import { TempEmployeeTalentDto, UpdateTempEmployeeTalentDto } from '@app/service/model/warning-employee/WarningEmployeeDto';
import { GetInputFilterDto } from '@app/service/model/employee/GetEmployeeExcept.dto';
import { BranchDto } from '@app/service/model/categories/branch.dto';
import { JobPositionDto } from '@app/service/model/categories/jobPosition.dto';
import { APP_ENUMS } from '@shared/AppEnums';
import { UserTypeService } from '@app/service/api/categories/userType.service';
import { X } from '@angular/cdk/keycodes';
import { FormControl, Validators } from '@angular/forms';
import { EmployeeDetailDialogComponent } from './employee-detail-dialog/employee-detail-dialog.component';
import { MultiCreateEmployeeFromTempComponent } from './multi-create-employee-from-temp/multi-create-employee-from-temp.component';

@Component({
  selector: 'app-temp-employee-talent',
  templateUrl: './temp-employee-talent.component.html',
  styleUrls: ['./temp-employee-talent.component.css']
})
export class TempEmployeeTalentComponent extends PagedListingComponentBase<TempEmployeeTalentDto> implements OnInit {
  protected list(request: PagedRequestDto, pageNumber: number, finishedCallback: Function): void {

    var dto = {
      statusIds: this.statusIds,
      branchIds: this.branchIds,
      userTypes: this.userTypes,
      jobPositionIds: this.jobPositionIds,
      gridParam: request
    } as GetInputFilterDto

    this.warningEmployeeService
      .GetTempEmployeeTalentPaging(dto)
      .pipe(
        finalize(() => {
          finishedCallback();
        })
      )
      .subscribe((rs) => {
        this.controlTempEmployee = [];
        rs.result.items.forEach((tempEmployee) => {
          this.controlTempEmployee.push({
            employee: tempEmployee,
            isSelected: false,
            isEdited: false,
            formControl: {
              nccEmail: new FormControl(this.getSuggestEmail(tempEmployee), [
                Validators.required,
                Validators.email,
              ]),
              jobPosition: new FormControl(tempEmployee.jobPositionId, [
                Validators.required,
              ]),
            },
          });
        });
        this.showPaging(rs.result, pageNumber);
      });
  }
  isShowSalary: boolean = false;
  WarningEmployee_PlanOnboard_Create = PERMISSIONS_CONSTANT.WarningEmployee_PlanOnboard_Create
  WarningEmployee_PlanOnboard_Edit = PERMISSIONS_CONSTANT.WarningEmployee_PlanOnboard_Edit;
  WarningEmployee_PlanOnboard_Delete = PERMISSIONS_CONSTANT.WarningEmployee_PlanOnboard_Delete
  WarningEmployee_PlanOnboard_ViewSalary = PERMISSIONS_CONSTANT.WarningEmployee_PlanOnboard_ViewSalary

  public controlTempEmployee: ControlTempEmployee[] = [];
  public allSelected = false;
  public listBreadCrumb = [];
  public userTypeList: UserTypeDto[] = [];
  public branchList: BranchDto[] = []
  public positionList: JobPositionDto[] = []
  public statusList = [
      {
        key: "Accepted offer",
        value: 8
      },
      {
        key: "Rejected offer",
        value: 9
      },
      {
        key: "Onboarded",
        value: 10
      },
  ];
  public filterMultipleTypeParamEnum = APP_ENUMS.FilterMultipleTypeParamEnum;

  constructor(
    injector: Injector,
    private userTyService: UserTypeService,
    private positionService: JobPositionService,
    private branchService: BranchService,
    private warningEmployeeService: WarningEmployeeService
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.setBreadCrumb()
    this.getAllUserType()
    this.getAllJobPositon()
    this.getAllBranch()
    this.statusIds.push(APP_ENUMS.TalentOnboardStatus.AcceptedOffer);
    this.bindDefaultFilter();
    this.allSelected = false;
    this.refresh()
  }

  public bindDefaultFilter(){
    if(this.filterItems.length) {
      this.filterItems.forEach(filterItem => {
        this.defaultFilterValue[filterItem.propertyName] = filterItem.value;
      })
    } else {
      this.sortDirection = this.APP_ENUM.SortDirectionEnum.Descending;
      this.sortProperty = 'updatedTime';
    }
  }

  confirm(temp: {employee: TempEmployeeTalentDto, formControl: {nccEmail: FormControl, jobPosition: FormControl}}) {
    const employee = temp.employee;
    employee.nccEmail = temp.formControl.nccEmail.value;
    employee.jobPositionId = temp.formControl.jobPosition.value;
    this.dialog
      .open(EmployeeDetailDialogComponent, {
        data: { employee: employee },
        minWidth: "680px",
      })
      .afterClosed()
      .subscribe(({ isReload }) => {
        if (isReload != null && isReload) {
          this.refresh();
        }
      });
  }
  private getAllUserType() {
    this.subscription.push(this.userTyService.getAll().subscribe(rs => {
      this.userTypeList = this.mapToFilter(rs.result, true).filter(x => x.value == APP_ENUMS.UserType.Internship || x.value == APP_ENUMS.UserType.ProbationaryStaff)
    }))
  }

  private getAllJobPositon() {
    this.subscription.push(this.positionService.getAll().subscribe(rs => {
      this.positionList = this.mapToFilter(rs.result, true)
    }))
  }
  private getAllBranch() {
    this.subscription.push(this.branchService.getAll().subscribe(rs => {
      this.branchList = this.mapToFilter(rs.result, true)
    }))
  }

  setBreadCrumb() {
    this.listBreadCrumb = [
      { name: '<i class="fa-solid fa-house fa-sm"></i>', url: '' },
      { name: ' <i class="fa-solid fa-chevron-right"></i> ' },
      { name: 'Plan Onboard Employee' }];
  }

  isShowCreatEmployeeBtn(employee: TempEmployeeTalentDto) {
    if (employee.onboardStatus == this.APP_ENUM.TalentOnboardStatus.RejectedOffer
      || employee.onboardStatus == this.APP_ENUM.TalentOnboardStatus.Onboarded) {
      return false
    }
    return true;
  }

  isShowEditEmployeeBtn(employee: TempEmployeeTalentDto) {
    if (employee.onboardStatus == this.APP_ENUM.TalentOnboardStatus.RejectedOffer
      || employee.onboardStatus == this.APP_ENUM.TalentOnboardStatus.Onboarded) {
      return false
    }
    return true;
  }

  delete(employee:TempEmployeeTalentDto){
    abp.message.confirm(`Delete temp employee ${employee.fullName}`, "", (rs)=>{
      if(rs){
        this.subscription.push(
          this.warningEmployeeService.DeleteTempEmployeeTalent(employee.id).subscribe(rs =>{
            abp.notify.success(`Deleted temp employee <strong>${employee.fullName}</strong>`)
            this.refresh()
          })
        )
      }
    }, true)
  }

  onEdit(control: ControlTempEmployee) {
    if (Object.entries(control.formControl).some((value) => value[1].invalid) && control.isEdited) {
      Object.entries(control.formControl).forEach((value) => {
        value[1].markAsTouched({ onlySelf: true });
      });
      return;
    }
    control.isEdited = !control.isEdited;
    if (!control.isEdited){
      const employee = control.employee;
      const newTempEmployee: UpdateTempEmployeeTalentDto = {
        ...employee,
        personalEmail: employee.email,
        nccEmail: control.formControl.nccEmail.value,
        jobPositionId: control.formControl.jobPosition.value,
        skills: employee.skillStr ?? "",
      };
      this.warningEmployeeService.UpdateTempEmployeeTalent(newTempEmployee).subscribe(({success, error}) => {
        if (success) {
          abp.notify.success(`${employee.fullName} has update`, `Update success`);
        } else {
          abp.notify.error(`${employee.fullName} : ${error}`, `Update fail`);
        }
        if (this.controlTempEmployee.every(({ isEdited }) => !isEdited)) {
          this.refresh();
        }
      })
    }
  }

  onCreateAll() {
    if (this.controlTempEmployee.every(({ isSelected }) => !isSelected)) {
      abp.message.error(`Please select temp employee first!`);
      return;
    }
    const tempInvalid = this.controlTempEmployee.filter(
      ({ formControl, isSelected }) =>
        isSelected &&
        (formControl.jobPosition.invalid || formControl.nccEmail.invalid)
    );
    if (tempInvalid.length) {
      abp.message.error(
        `${tempInvalid
          .map(({ employee }) => employee.fullName)
          .join(", ")} is invalid`
      );
      return;
    }

    this.dialog.open(MultiCreateEmployeeFromTempComponent, {
      width: "50%",
      minWidth: "700px",
      data: {
        employees: this.controlTempEmployee
          .filter((temp) => temp.isSelected)
          .map(({ employee, formControl }) => {
            employee.nccEmail = formControl.nccEmail.value;
            employee.jobPositionId = formControl.jobPosition.value;
            return employee;
          }),
      },
    }).afterClosed().subscribe(() => {
      this.refresh();
    });
  }

  someSelected(): boolean{
    if (this.controlTempEmployee == null) {
      return false;
    }
    return (
      this.controlTempEmployee.filter((temp) => temp.isSelected).length > 0 &&
      !this.allSelected
    );
  }

  onSetAll(isSelect: boolean){
    if (this.controlTempEmployee == null) {
      return;
    }
    this.allSelected = isSelect;
    this.controlTempEmployee.forEach((temp) => temp.isSelected = isSelect);
  }

  updateAllSelected() {
    this.allSelected =
      this.controlTempEmployee != null &&
      this.controlTempEmployee.every(({isSelected}) => isSelected);
  }

  isShowCreateBtn(){
    return this.permission.isGranted(this.WarningEmployee_PlanOnboard_Create)
  }

  isShowEditBtn(){
    return this.permission.isGranted(this.WarningEmployee_PlanOnboard_Edit);
  }

  isShowDeleteBtn(){
    return this.permission.isGranted(this.WarningEmployee_PlanOnboard_Delete)
  }

  isShowCheckbox() {
    return this.permission.isGranted(this.WarningEmployee_PlanOnboard_ViewSalary)
  }

  getSuggestEmail(employee : TempEmployeeTalentDto){
    if (employee.nccEmail != null){
      return employee.nccEmail;
    }
    const fullname = employee.fullName
      .trim()
      .toLowerCase()
      .normalize("NFD")
      .replace(/[\u0300-\u036f]/g, "")
      .replace("đ", "d")
      .split(" ");
    return `${fullname.pop()}.${fullname.join("")}@ncc.asia`;
  }
}

interface ControlTempEmployee {
  employee: TempEmployeeTalentDto;
  isSelected: boolean;
  isEdited: boolean;
  formControl: { nccEmail: FormControl; jobPosition: FormControl };
}
