import { AddBenefitEmployeeDialogComponent } from './add-benefit-employee-dialog/add-benefit-employee-dialog.component';
import { BEmployeeDefaultFilterDto, BenefitEmployeeDto, updateDateType } from './../../../../service/model/benefits/benefitEmployee.dto';
import { AfterViewInit, Component, Injector, OnInit, QueryList, ViewChildren } from '@angular/core';
import { FilterDto, PagedListingComponentBase, PagedRequestDto } from '@shared/paged-listing-component-base';
import { finalize } from 'rxjs/operators';
import { EmployeeService } from '@app/service/api/employee/employee.service';
import { BenefitService } from '@app/service/api/benefits/benefit.service';
import { BranchService } from '@app/service/api/categories/branch.service';
import { LevelService } from '@app/service/api/categories/level.service';
import { TeamService } from '@app/service/api/categories/team.service';
import { UserTypeService } from '@app/service/api/categories/userType.service';
import { ActivatedRoute } from '@angular/router';
import { GetEmployeeDto } from '@app/service/model/employee/employee.dto';
import { APP_ENUMS } from '@shared/AppEnums';
import { AppConsts } from '@shared/AppConsts';
import { DatePipe } from '@angular/common';
import { JobPositionService } from '@app/service/api/categories/jobPosition.service';
import { UserTypeDto } from '@app/service/model/categories/userType.dto';
import { BranchDto } from '@app/service/model/categories/branch.dto';
import { JobPositionDto } from '@app/service/model/categories/jobPosition.dto';
import { TeamDto } from '@app/service/model/categories/team.dto';
import { AddEmployeeComponent } from '@shared/components/employee/add-employee/add-employee.component';
import { AddEmployeeToBenefitDto, QuickAddEmployeeDto } from '@app/service/model/benefits/addEmployee.dto';
import { UpdateBenefitDateDialogComponent } from './update-benefit-date-dialog/update-benefit-date-dialog.component';
import { GetInputFilterDto } from '@app/service/model/employee/GetEmployeeExcept.dto';
import * as moment from 'moment';
import { benefitDto } from '@app/service/model/benefits/beneft.dto';
import { AbstractControl, NgControl, NgModel } from '@angular/forms';
import { PERMISSIONS_CONSTANT } from '@app/permission/permission';
import { startWithTap } from '@shared/helpers/observerHelper';

@Component({
  selector: 'app-benefit-employee',
  templateUrl: './benefit-employee.component.html',
  styleUrls: ['./benefit-employee.component.css'],
  providers: [
    {
      provide: NgControl,
      useClass: NgModel
    }
  ]
})
export class BenefitEmployeeComponent extends PagedListingComponentBase<BenefitEmployeeDto> implements OnInit, AfterViewInit {
  @ViewChildren("startDate") startDate: QueryList<NgModel>
  @ViewChildren("endDate") endDate: QueryList<NgModel>
  protected list(request: PagedRequestDto, pageNumber: number, finishedCallback: Function): void {
    if(this.isAllowRoutingTabEmployee()){
      let input = {
        statusIds: this.statusIds,
        levelIds: this.levelIds,
        branchIds: this.branchIds,
        userTypes: this.userTypes,
        teamIds: this.teamIds,
        isAndCondition : this.isAndCondition,
        gridParam: request,
        jobPositionIds: this.jobPositionIds,
      } as GetInputFilterDto;
      this.subscription.push(
        this.benefitService.GetEmployeeInBenefitPaging(this.benefitId, input)
          .pipe(finalize(() => {
            finishedCallback();
          }))
          .subscribe(rs => {
            this.listBenefitEmployee = rs.result.items;
            this.showPaging(rs.result, pageNumber)
          })
      );
      this.isAddingEmployee = false;
      this.isEditingEmployee = false;
    } 
  }


  constructor(injector: Injector,
    private datePipe: DatePipe,
    private jobPositionService: JobPositionService,
    private benefitService: BenefitService,
    private branchService: BranchService,
    private levelService: LevelService,
    private teamService: TeamService,
    private userTyService: UserTypeService,
    private route: ActivatedRoute) { super(injector) }

  private benefitId: number;
  public benefitName: string = ""
  public benefit = {} as benefitDto;
  public employeeList: GetEmployeeDto[] = [];
  public listBenefitEmployee: BenefitEmployeeDto[] = [];
  public isEditingEmployee: boolean = false;
  public isAddingEmployee: boolean = false;
  public searchEmployee: string = ""
  public listApplyDate: any[] = []

  public userTypeList: UserTypeDto[] = []
  public userLevelList = []
  public branchList: BranchDto[] = []
  public positionList: JobPositionDto[] = []
  public teamList: TeamDto[] = []
  public filterTypeEnum = APP_ENUMS.FilterTypeEnum
  public statusList = []
  public genderList = []
  public defaultValue = {} as any
  public moneyList = []
  public isActive: boolean = true
  public benefitType: number
  public filterMultipleTypeParamEnum = APP_ENUMS.FilterMultipleTypeParamEnum;
  ngOnInit(): void {
    this.benefitId = Number(this.route.snapshot.queryParamMap.get('id'))
    this.benefitName = this.route.snapshot.queryParamMap.get('name')
    this.isActive = this.route.snapshot.queryParamMap.get("active") === "true" ? true : false
    this.benefitType = Number(this.route.snapshot.queryParamMap.get("type"))
    this.genderList = this.getListFormEnum(APP_ENUMS.Gender)
    this.statusList = this.getListFormEnum(APP_ENUMS.UserStatus, true)
    this.getAllUserType()
    this.getAllLevel()
    this.getAllBranch()
    this.getAllTeam()
    this.getAllJobPosition()
    this.setDefaultValue()
    this.getBenefitById();
    this.getListApplyDate()
    this.getAllEmployee()
    this.refresh();
  }

  ngAfterViewInit(): void {
    if (this.benefit.type !== APP_ENUMS.BenefitType.CheDoChung) {
      this.startDate.changes.subscribe(list => {
        if (list.first) {
          list.first.control.addValidators((control: AbstractControl) => {
            if (!this.endDate.first.value || !this.endDate.first.touched) return
            if (moment(this.endDate.first.value).toDate() <= moment(this.startDate.first.value).toDate()) {
              this.endDate.first.control.setErrors({ endDateInput: true })
              return
            }
            this.endDate.first.control.setErrors(null)
          })
        }
      })
      this.endDate.changes.subscribe(list => {
        if (list.first) {
          list.first.control.addValidators((control: AbstractControl) => {
            if (!this.startDate.first.value) return
            if (moment(this.endDate.first.value).toDate() <= moment(this.startDate.first.value).toDate()) {
              return { endDateInput: true }
            }
          })
        }
      })
    }
  }


  onSearchFilter(searchValue: string) {
    this.onSearch(searchValue)
  }

  private getListApplyDate() {
    this.subscription.push(this.benefitService.GetListMonthFilter().subscribe(rs => {
      this.listApplyDate = rs.result.map(x => { return { key: this.datePipe.transform(x, "MM/yyyy"), value: x } })
    }))
  }

  private getAllUserType() {
    this.subscription.push(this.userTyService.getAll().subscribe(rs => {
      this.userTypeList = this.mapToFilter(rs.result, true)
    }))
  }

  private getAllJobPosition() {
    this.subscription.push(this.jobPositionService.getAll().subscribe(rs => {
      this.positionList = this.mapToFilter(rs.result, true)
    }))
  }

  private getAllLevel() {
    this.subscription.push(this.levelService.getAll().subscribe(rs => {
      this.userLevelList = this.mapToFilter(rs.result, true)
    }))
  }

  private getAllBranch() {
    this.subscription.push(this.branchService.getAll().subscribe(rs => {
      this.branchList = this.mapToFilter(rs.result, true)
    }))
  }

  private getAllTeam() {
    this.subscription.push(this.teamService.getAll().subscribe(rs => {
      this.teamList = rs.result.map(x => {
        return {
          key: x.name,
          value: x.id
        }
      })
    }))
  }

  private getAllEmployee() {
    this.subscription.push(this.benefitService.GetAllEmployeeNotInBenefit(this.benefitId)
    .pipe(startWithTap(() => {
      this.isLoading = true;
    }))
    .pipe(finalize(() => {
      this.isLoading = false;
    }))
    .subscribe(rs => {
      this.employeeList = rs.result
    }))
  }

  public async openAddEmployeeDialog() {
    if (this.benefit.type != APP_ENUMS.BenefitType.CheDoChung) {
      let ref = this.dialog.open(AddBenefitEmployeeDialogComponent, {
        width: "700px",
        data: {
          title: `Add employee to benefit: <strong>${this.benefitName}</strong>`,
          benefitId: this.benefitId,
          benefitName: this.benefitName,
          benefitType: this.benefit.type,
          startDate: this.benefit.applyDate
        }
      })
      ref.afterClosed().subscribe((result: AddEmployeeToBenefitDto) => {
        if (result) {
          this.subscription.push(this.benefitService.AddEmployeeToBenefit(result)
          .pipe(startWithTap(() => {
            this.isLoading = true;
          }))
          .pipe(finalize(() => {
            this.isLoading = false;
          }))
          .subscribe(rs => {
            abp.message.success(`Added ${result.listEmployeeId.length} Employee to benefit`)
            this.refresh()
          }))
        }
      })
    } else {
      const addedEmployeeIds = await this.benefitService.GetListEmployeeIdInBenefit(this.benefit.id).toPromise()
      this.dialog.open(AddEmployeeComponent, {
        width: "92vw",
        height: "97vh",
        maxWidth: "100vw",
        data: {
          title: `Add employee to benefit: <strong>${this.benefit.name}</strong>`,
          addedEmployeeIds: addedEmployeeIds.result
        }
      }).afterClosed().subscribe(selectedEmployees => {
        if (selectedEmployees?.length) {
          let addEmployeesToBenefit: AddEmployeeToBenefitDto = {
            benefitId: this.benefit.id,
            endDate: null,
            listEmployeeId: selectedEmployees,
            startDate: null
          }
          this.subscription.push(this.benefitService.AddEmployeeToBenefit(addEmployeesToBenefit)
          .pipe(startWithTap(() => {
            this.isLoading = true;
          }))
          .pipe(finalize(() => {
            this.isLoading = false;
          }))
          .subscribe(result => {
            abp.message.success(`Added ${selectedEmployees.length} Employee to benefit`)
            this.refresh()
          }))
        }
      })
    }
  }
  public updateStartDate() {
    this.openDialog(UpdateBenefitDateDialogComponent,
      { title: `Update start date <strong>${this.benefitName}</strong>`, benefitId: this.benefitId, type: updateDateType.startDate })
  }
  public updateEndDate() {
    this.openDialog(UpdateBenefitDateDialogComponent,
      { title: `Update end date of <strong>${this.benefitName}</strong>`, benefitId: this.benefitId, type: updateDateType.endDate })
  }


  public setDefaultValue() {
    this.defaultValue = {
      branch: AppConsts.DEFAULT_ALL_FILTER_VALUE,
      userLevel: AppConsts.DEFAULT_ALL_FILTER_VALUE,
      status: AppConsts.DEFAULT_ALL_FILTER_VALUE,
      team: AppConsts.DEFAULT_ALL_FILTER_VALUE,
      gender: AppConsts.DEFAULT_ALL_FILTER_VALUE,
      userType: AppConsts.DEFAULT_ALL_FILTER_VALUE,
      jobPosition: AppConsts.DEFAULT_ALL_FILTER_VALUE,
      applyMonth: AppConsts.DEFAULT_ALL_FILTER_VALUE
    } as BEmployeeDefaultFilterDto
  }

  public getBenefitById() {
    this.subscription.push(
      this.benefitService.get(Number(this.benefitId)).subscribe((res) => {
        this.benefit = res.result;
      }))
  }

  public onUpdate(employee: BenefitEmployeeDto) {
    employee.createMode = true;
    this.isEditingEmployee = true;
  }

  public onSave(employee: BenefitEmployeeDto) {
    employee.startDate = employee.startDate ? this.formatDateYMD(employee.startDate) : null
    employee.endDate = employee.endDate ? this.formatDateYMD(employee.endDate) : null
    let { employeeId, startDate, endDate } = employee
    let input = {
      benefitId: this.benefitId,
      employeeId: employeeId,
      startDate: startDate,
      endDate: endDate
    } as QuickAddEmployeeDto
    if (!this.isEditingEmployee) {
      this.subscription.push(
        this.benefitService.QuickAddEmployee(input)
        .pipe(startWithTap(() => {
          this.isLoading = true
        }))
        .pipe(finalize(() => {
          this.isLoading = false;
          this.isEditingEmployee = false;
          employee.createMode = false;
          this.getAllEmployee();
        }))
        .subscribe(rs => {
          abp.notify.success(`Added new employee to benefit`)
          this.refresh();
        }))
    }
    else {
      this.subscription.push(
        this.benefitService.UpdateBenefitEmployee(employee)
        .pipe(startWithTap(() => {
          this.isLoading = true
          
        }))
        .pipe(finalize(() => {
          this.isLoading = false;
          this.getAllEmployee();
        }))
        .subscribe(rs => {
          this.isEditingEmployee = false;
          employee.createMode = false;
          abp.notify.success(`Update successful`)
          this.refresh();
        }))
    }
  }

  public onCancel(employee: BenefitEmployeeDto) {
    this.isEditingEmployee = false;
    this.isAddingEmployee = false;
    this.refresh();
  }

  public onDelete(employee: BenefitEmployeeDto) {
    this.confirmDelete(`Remove <strong>${employee.fullName}</strong> from benefit <strong>${this.benefitName}</strong>`,
      () => this.benefitService.RemoveEmployeeFromBenefit(employee.id).toPromise().then(rs => {
        abp.notify.success(`Remove ${employee.fullName} from benefit ${this.benefitName}`);
      }))
  }
  public onFilter(filterItem: FilterDto): void {
    let existFilterItem = this.filterItems.find(x => x.propertyName === filterItem.propertyName)
    if (existFilterItem) {
      existFilterItem.value = filterItem.value
    }
    else {
      this.filterItems.push(filterItem)
    }
    this.refresh()
  }


  public onQuickAdd() {
    let employee = {} as BenefitEmployeeDto;
    if (this.benefit.type !== APP_ENUMS.BenefitType.CheDoChung) {
      employee.startDate = this.formatDateYMD(new Date())
      employee.endDate = null
    }
    employee.createMode = true;
    this.isAddingEmployee = true;
    this.listBenefitEmployee.unshift(employee);

  }

  public navigateListBenefit() {
    this.router.navigate(['/app/benefits/list-benefit'])
  }
  isShowAddBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Benefit_BenefitDetail_TabEmployee_Add);
  }
  isShowQuickAddBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Benefit_BenefitDetail_TabEmployee_QuickAdd);
  }
  isShowEditBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Benefit_BenefitDetail_TabEmployee_Edit);
  }
  isShowDeleteBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Benefit_BenefitDetail_TabEmployee_Delete);
  }
  isShowUpdateAllStartDateBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Benefit_BenefitDetail_TabEmployee_UpdateAllStartDate);
  }
  isShowUpdateAllEndDateBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Benefit_BenefitDetail_TabEmployee_UpdateAllEndDate);
  }
  isAllowViewTabBenefit(){
    return this.isGranted(PERMISSIONS_CONSTANT.Employee_EmployeeDetail_TabBenefit_View);
  }
  isAllowViewTabPersonalInfo(){
    return this.isGranted(PERMISSIONS_CONSTANT.Employee_EmployeeDetail_TabPersonalInfo_View);
  }
  isAllowRoutingTabEmployee(){
    return this.isGranted(PERMISSIONS_CONSTANT.Benefit_BenefitDetail_TabEmployee_View);
  }


}

