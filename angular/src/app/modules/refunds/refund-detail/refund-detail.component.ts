import { DefaulEmployeeFilterDto } from './../../../../shared/components/employee/employee-filter/employee-filter.component';
import { startWithTap } from '@shared/helpers/observerHelper';
import { PERMISSIONS_CONSTANT } from './../../../permission/permission';
import { finalize } from 'rxjs/operators';
import { ActivatedRoute } from '@angular/router';
import { JobPositionService } from './../../../service/api/categories/jobPosition.service';
import { UserTypeService } from './../../../service/api/categories/userType.service';
import { TeamService } from './../../../service/api/categories/team.service';
import { LevelService } from './../../../service/api/categories/level.service';
import { EmployeeService } from './../../../service/api/employee/employee.service';
import { BranchService } from './../../../service/api/categories/branch.service';
import { Component, OnInit, Injector } from '@angular/core';
import { BranchDto } from '@app/service/model/categories/branch.dto';
import { JobPositionDto } from '@app/service/model/categories/jobPosition.dto';
import { LevelDto } from '@app/service/model/categories/level.dto';
import { TeamDto } from '@app/service/model/categories/team.dto';
import { UserTypeDto } from '@app/service/model/categories/userType.dto';
import { GetEmployeeDto } from '@app/service/model/employee/employee.dto';
import { GetInputFilterDto } from '@app/service/model/employee/GetEmployeeExcept.dto';
import { APP_ENUMS } from '@shared/AppEnums';
import { PagedListingComponentBase, PagedRequestDto } from '@shared/paged-listing-component-base';
import { RefundEmployeeDto } from '@app/service/model/refunds/refundEmployee.dto';
import { RefundDto } from '@app/service/model/refunds/refund.dto';
import { RefundsService } from '@app/service/api/refunds/refunds.service';

@Component({
  selector: 'app-refund-detail',
  templateUrl: './refund-detail.component.html',
  styleUrls: ['./refund-detail.component.css']
})
export class RefundDetailComponent extends PagedListingComponentBase<RefundEmployeeDto> implements OnInit{

  private refundId: number;
  public refund = {} as RefundDto;
  public employeeList: GetEmployeeDto[] = [];
  public refundEmployeeList: RefundEmployeeDto[] = [];
  public isEditingEmployee: boolean = false;
  public isAddingEmployee: boolean = false;
  public searchEmployee: string = "";
  public defaultValue = {} as DefaulEmployeeFilterDto;
  public userTypeList: UserTypeDto[] = []
  public userLevelList: LevelDto[] = [];
  public branchList: BranchDto[] = []
  public positionList: JobPositionDto[] = []
  public teamList: TeamDto[] = []
  public filterTypeEnum = APP_ENUMS.FilterTypeEnum
  public statusList = [];
  public genderList = [];
  public filterMultipleTypeParamEnum = APP_ENUMS.FilterMultipleTypeParamEnum;

  public isFocusing: boolean = false;
  protected list(request: PagedRequestDto, pageNumber: number, finishedCallback: Function): void {
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
      this.refundService.GetRefundEmployeesPaging(this.refundId, input)
        .pipe(finalize(() => {
          finishedCallback();
        }))
        .subscribe(rs => {
          this.refundEmployeeList = rs.result.items;
          this.showPaging(rs.result, pageNumber)
        })
    );
    this.isAddingEmployee = false;
    this.isEditingEmployee = false;
  }

  constructor(injector: Injector, private employeeService: EmployeeService,
    private branchService: BranchService,
    private levelService: LevelService,
    private teamService: TeamService,
    private userTyService: UserTypeService,
    private positionService: JobPositionService,
    private refundService: RefundsService, private route: ActivatedRoute) { super(injector) }

  ngOnInit(): void {
    this.refundId = Number(this.route.snapshot.queryParamMap.get('id'));
    this.getRefundById();
    this.getAllUser();
    this.refresh();
    this.genderList = this.getListFormEnum(APP_ENUMS.Gender);
    this.statusList = this.getListFormEnum(APP_ENUMS.UserStatus, true);
    this.getAllUserType()
    this.getAllLevel();
    this.getAllBranch();
    this.getAllTeam();
    this.getAllJobPositon();
    this.setDefaultValue();
  }

  isShowAddEmployeeBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Refund_RefundDetail_AddEmployee);
  }
  isShowEditBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Refund_RefundDetail_Edit);
  }
  isShowDeleteBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Refund_RefundDetail_Delete);
  }

  isAllowViewTabRefund(){
    return this.isGranted(PERMISSIONS_CONSTANT.Refund_RefundDetail_View);
  }

  isAllowViewTabPersonalInfo(){
    return true
  }

  public getAllUser() {
    this.subscription.push(
      this.refundService.GetAllEmployeeNotInRefund(this.refundId)
      .pipe(startWithTap(() => { this.isLoading = true}))
      .pipe(finalize(() => this.isLoading = false))
      .subscribe((res) => {
        this.employeeList = res.result;
      })
    )
  }

  public getRefundById() {
    this.subscription.push(this.refundService.get(Number(this.refundId)).subscribe((res) => {
      this.refund = res.result;
      this.listBreadCrumb = [
        { name: '<i class="fa-solid fa-house fa-sm"></i>', url: '' },
        { name: ' <i class="fa-solid fa-chevron-right"></i> ' },
        { name: 'Refund', url: '/app/refunds/list-refund' },
        { name: '<i class="fa-solid fa-chevron-right fa-sm"></i>' },
        { name: this.refund.name }]
    }))
  }

  public onAddEmployee() {
    let employee = {} as RefundEmployeeDto;
    employee.createMode = true;
    this.isAddingEmployee = true;
    employee.note = this.refund.name
    this.refundEmployeeList.unshift(employee);
  }

  public onUpdate(employee: RefundEmployeeDto) {
    employee.updateMode = true;
    this.isEditingEmployee = true;
    this.isFocusing = true;
  }

  public focusOut() {
    this.isFocusing = false;
  }

  public onSave(employee: RefundEmployeeDto) {
    employee.refundId = this.refundId;
    if (employee.id) {
      this.subscription.push(
        this.refundService.UpdateRefundEmployee(employee)
        .pipe(startWithTap(() => { this.isLoading = true}))
        .pipe(finalize(() => this.onCompletedCall()))
        .subscribe((res) => {
          if (res) {
            abp.notify.success("Updated employee");
            employee.updateMode = false;
            this.isEditingEmployee = false;
            this.getAllUser()
            this.refresh();
          }
        })
      )
    } else {
      this.refundService.AddEmployeeToRefund(employee)
      .pipe(startWithTap(() => {this.isLoading = true}))
      .pipe(finalize(() => this.onCompletedCall()))
      .subscribe(res => {
        if (res) {
          abp.notify.success("Added employee to refund");
          employee.createMode = false;
          this.isEditingEmployee = false;
          this.isAddingEmployee = false;
          this.getAllUser()
          this.refresh();
        }
      })
    }

  }

  public onCancel(employee: RefundEmployeeDto, i) {
    if(employee.id) {
      this.refresh();
    }
    else {
      this.refundEmployeeList.splice(i, 1);
    }
    this.isAddingEmployee = false
    this.isEditingEmployee = false
  }

  public onDelete(employee: RefundEmployeeDto) {
    this.confirmDelete(`Remove employee: <strong>${employee.fullName}</strong>`,
      () => this.refundService.DeleteRefundEmployee(employee.id).toPromise().then(rs => {
        abp.notify.success(`Removed ${employee.fullName}`);
        this.getAllUser();
      }))
  }


  public setDefaultValue() {
    this.defaultValue = {
      branch: this.APP_CONST.DEFAULT_ALL_FILTER_VALUE,
      userLevel: this.APP_CONST.DEFAULT_ALL_FILTER_VALUE,
      status: this.APP_CONST.DEFAULT_ALL_FILTER_VALUE,
      team: this.APP_CONST.DEFAULT_ALL_FILTER_VALUE,
      gender: this.APP_CONST.DEFAULT_ALL_FILTER_VALUE,
      userType: this.APP_CONST.DEFAULT_ALL_FILTER_VALUE,
      jobPosition: this.APP_CONST.DEFAULT_ALL_FILTER_VALUE
    } as DefaulEmployeeFilterDto
  }

  onSearchFilter(searchValue: string) {
    this.onSearch(searchValue)
  }

  private getAllUserType() {
    this.subscription.push(this.userTyService.getAll().subscribe(rs => {
      this.userTypeList = this.mapToFilter(rs.result, true)
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
      this.teamList = this.mapToFilter(rs.result, true)
    }))
  }

  private getAllJobPositon() {
    this.subscription.push(this.positionService.getAll().subscribe(rs => {
      this.positionList = this.mapToFilter(rs.result, true)
    }))
  }

  onCompletedCall() {
    this.isLoading = false;
    this.isAddingEmployee = false;
    this.isEditingEmployee = false;
  }

}
