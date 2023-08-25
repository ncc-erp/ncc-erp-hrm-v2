import { LevelDto } from './../../../service/model/categories/level.dto';
import { JobPositionDto } from './../../../service/model/categories/jobPosition.dto';
import { TeamDto } from './../../../service/model/categories/team.dto';
import { BranchDto } from './../../../service/model/categories/branch.dto';
import { UserTypeDto } from './../../../service/model/categories/userType.dto';
import { JobPositionService } from '@app/service/api/categories/jobPosition.service';
import { UserTypeService } from '@app/service/api/categories/userType.service';
import { TeamService } from '@app/service/api/categories/team.service';
import { LevelService } from '@app/service/api/categories/level.service';
import { BranchService } from '@app/service/api/categories/branch.service';
import { APP_ENUMS } from '@shared/AppEnums';
import { AppConsts } from '@shared/AppConsts';
import { ImportFilePunishmentDetailComponent } from './import-file-punishment-detail/import-file-punishment-detail.component';
import { finalize } from 'rxjs/operators';
import { ActivatedRoute } from '@angular/router';
import { PunishmentService } from './../../../service/api/punishment/punishment.service';
import { DefaulEmployeeFilterPunishmentDto, PunishmentEmployeeDto } from './../../../service/model/punishments/punishments.dto';
import { EmployeeService } from './../../../service/api/employee/employee.service';
import { PunishmentsDto } from '@app/service/model/punishments/punishments.dto';
import { PagedListingComponentBase, PagedRequestDto } from '@shared/paged-listing-component-base';
import { Component, OnInit, Injector } from '@angular/core';
import { GetEmployeeDto } from '@app/service/model/employee/employee.dto';
import { isBuffer, map } from 'lodash-es';
import { GetInputFilterDto } from '@app/service/model/employee/GetEmployeeExcept.dto';
import { PERMISSIONS_CONSTANT } from '@app/permission/permission';
import {startWith } from 'rxjs/operators'
import { startWithTap } from '@shared/helpers/observerHelper'
@Component({
  selector: 'app-punishment-detail',
  templateUrl: './punishment-detail.component.html',
  styleUrls: ['./punishment-detail.component.css']
})
export class PunishmentDetailComponent extends PagedListingComponentBase<PunishmentEmployeeDto> implements OnInit {
  private punishmentId: number;
  public punishment = {} as PunishmentsDto;
  public employeeList: GetEmployeeDto[] = [];
  public punishmentEmployeeList: PunishmentEmployeeDto[] = [];
  public isEditingEmployee: boolean = false;
  public isAddingEmployee: boolean = false;
  public searchEmployee: string = "";
  public defaultValue = {} as DefaulEmployeeFilterPunishmentDto;
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
      this.punishmentService.getAllEmployeeInPunishment(this.punishmentId, input)
        .pipe(finalize(() => {
          finishedCallback();
        }))
        .subscribe(rs => {
          this.punishmentEmployeeList = rs.result.items;
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
    private punishmentService: PunishmentService, private route: ActivatedRoute) { super(injector) }
  ngOnInit(): void {
    this.punishmentId = Number(this.route.snapshot.queryParamMap.get('id'));
    this.getPunishmentById();
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
    return this.isGranted(PERMISSIONS_CONSTANT.Punishment_PunishmentDetail_AddEmployee);
  }
  isShowEditBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Punishment_PunishmentDetail_Edit);
  }
  isShowDeleteBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Punishment_PunishmentDetail_Delete);
  }
  isShowImportBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Punishment_PunishmentDetail_Import);
  }
  isShowDownloadTemplateBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Punishment_PunishmentDetail_DownloadTemplate);
  }
  isAllowViewTabPunishment(){
    return this.isGranted(PERMISSIONS_CONSTANT.Employee_EmployeeDetail_TabPunishment_View);
  }
  isAllowViewTabPersonalInfo(){
    return this.isGranted(PERMISSIONS_CONSTANT.Employee_EmployeeDetail_TabPersonalInfo_View);
  }

  public getAllUser() {
    this.subscription.push(
      this.punishmentService.GetAllEmployeeNotInPunishment(this.punishmentId)
      .pipe(startWithTap(() => { this.isLoading = true}))
      .pipe(finalize(() => this.isLoading = false))
      .subscribe((res) => {
        this.employeeList = res.result;
      })
    )
  }

  public getPunishmentById() {
    this.subscription.push(this.punishmentService.getPunishmentById(Number(this.punishmentId)).subscribe((res) => {
      this.punishment = res.result;
      this.listBreadCrumb = [
        { name: '<i class="fa-solid fa-house fa-sm"></i>', url: '' },
        { name: ' <i class="fa-solid fa-chevron-right"></i> ' },
        { name: 'Punishments', url: '/app/punishments/list-punishments' },
        { name: '<i class="fa-solid fa-chevron-right fa-sm"></i>' },
        { name: this.punishment.name }]
    }))
  }

  public onAddEmployee() {
    let employee = {} as PunishmentEmployeeDto;
    employee.createMode = true;
    this.isAddingEmployee = true;
    employee.note = this.punishment.name
    this.punishmentEmployeeList.unshift(employee);
  }

  public onUpdate(employee: PunishmentEmployeeDto) {
    employee.updateMode = true;
    this.isEditingEmployee = true;
    this.isFocusing = true;
  }

  public focusOut() {
    this.isFocusing = false;
  }

  public onSave(employee: PunishmentEmployeeDto) {
    employee.punishmentId = this.punishmentId;
    if (employee.id) {
      this.subscription.push(
        this.punishmentService.updateEmployeeInPunishment(employee)
        .pipe(startWithTap(() => { this.isLoading = true}))
        .pipe(finalize(() => this.onCompletedCall()))
        .subscribe((res) => {
          if (res) {
            abp.notify.success("Update employee in punishment successful");
            employee.updateMode = false;
            this.isEditingEmployee = false;
            this.getAllUser()
            this.refresh();
          }
        })
      )
    } else {
      this.punishmentService.addEmployeeToPunishment(employee)
      .pipe(startWithTap(() => {this.isLoading = true}))
      .pipe(finalize(() => this.onCompletedCall()))
      .subscribe(res => {
        if (res) {
          abp.notify.success("Add employee to punishment successful");
          employee.createMode = false;
          this.isEditingEmployee = false;
          this.isAddingEmployee = false;
          this.getAllUser()
          this.refresh();
        }
      })
    }

  }

  public onCancel(employee: PunishmentEmployeeDto, i) {
    if(employee.id) {
      this.refresh();
      this.isEditingEmployee = false
    }
    else {
      this.punishmentEmployeeList.splice(i, 1);
      this.isAddingEmployee = false
    }
  }

  public onDelete(employee: PunishmentEmployeeDto) {
    this.confirmDelete(`Delete employee <strong>${employee.fullName}</strong>`,
      () => this.punishmentService.deleteEmployeeFromPunishment(employee.id).toPromise().then(rs => {
        abp.notify.success(`Deleted punishment ${employee.fullName}`);
        this.getAllUser();
      }))
  }

  public onImportFile() {
    this.openDialog(ImportFilePunishmentDetailComponent, {
      data: {
        punishmentId: this.punishmentId
      }
    });
  }
  public setDefaultValue() {
    this.defaultValue = {
      branch: AppConsts.DEFAULT_ALL_FILTER_VALUE,
      userLevel: AppConsts.DEFAULT_ALL_FILTER_VALUE,
      status: AppConsts.DEFAULT_ALL_FILTER_VALUE,
      team: AppConsts.DEFAULT_ALL_FILTER_VALUE,
      gender: AppConsts.DEFAULT_ALL_FILTER_VALUE,
      userType: AppConsts.DEFAULT_ALL_FILTER_VALUE,
      jobPosition: AppConsts.DEFAULT_ALL_FILTER_VALUE
    } as DefaulEmployeeFilterPunishmentDto
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
