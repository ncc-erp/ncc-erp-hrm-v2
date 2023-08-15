import { ActivatedRoute } from '@angular/router';
import { AppConsts } from '@shared/AppConsts';
import { AppComponentBase } from '@shared/app-component-base';
import { Component, OnInit, Injector, Output, EventEmitter, Input } from '@angular/core';
import { BranchService } from '@app/service/api/categories/branch.service';
import { JobPositionService } from '@app/service/api/categories/jobPosition.service';
import { LevelService } from '@app/service/api/categories/level.service';
import { TeamService } from '@app/service/api/categories/team.service';
import { UserTypeService } from '@app/service/api/categories/userType.service';
import { BranchDto } from '@app/service/model/categories/branch.dto';
import { LevelDto } from '@app/service/model/categories/level.dto';
import { UserTypeDto } from '@app/service/model/categories/userType.dto';
import { APP_ENUMS } from '@shared/AppEnums';
import { TeamsFilterInputDto } from '@shared/paged-listing-component-base';
import { SeniorityFilterDto } from '@app/service/model/employee/GetEmployeeExcept.dto';

@Component({
  selector: 'app-employee-filter',
  templateUrl: './employee-filter.component.html',
  styleUrls: ['./employee-filter.component.css']
})
export class EmployeeFilterComponent extends AppComponentBase implements OnInit {
  @Input() searchText: string
  @Input() isOnDialog: boolean
  @Input() isExistFilterContractEndDate: boolean;
  @Input() isExistFilterBirthday:boolean;
  @Output() onFilter?= new EventEmitter()
  @Output() onMultiFilterWithCondition? = new EventEmitter()
  @Output() onMultiFilter? = new EventEmitter()
  @Output() onFilterBySeniority = new EventEmitter()
  @Output() onDateSelectorChange = new EventEmitter()
  @Output() onSearch?= new EventEmitter()
  @Output() onSelect?= new EventEmitter()
  @Output() onCancel?= new EventEmitter()
  @Output() onChangeDaysLeftOfContractEnd = new EventEmitter();

  public userTypeList = []
  public userLevelList = []
  public branchList = []
  public positionList = []
  public teamList = []
  public filterTypeEnum = APP_ENUMS.FilterTypeEnum
  public filterMultipleTypeParamEnum = APP_ENUMS.FilterMultipleTypeParamEnum
  public statusList = []
  public genderList = []
  public defaultValue = {} as DefaulEmployeeFilterDto
  public daysLeftContractEnd : number = 20;
  public listDateOptions= [];
  constructor(injector: Injector, private branchService: BranchService,
    private levelService: LevelService, private positionService: JobPositionService,
    private teamService: TeamService,
    private userTyService: UserTypeService,
    private route:ActivatedRoute) {
    super(injector)
  }

  ngOnInit(): void {
    this.genderList = this.getListFormEnum(APP_ENUMS.Gender)
    this.statusList = this.getListFormEnum(APP_ENUMS.UserStatus, true)

    this.getAllUserType()
    this.getAllLevel()
    this.getAllBranch()
    this.getAllJobPositon()
    this.getAllTeam()
    this.setDefaultValue()
    this.getDateOptions()
  }
  getDateOptions(){
    this.listDateOptions = [APP_ENUMS.DATE_TIME_OPTIONS.All, APP_ENUMS.DATE_TIME_OPTIONS.Day
      ,APP_ENUMS.DATE_TIME_OPTIONS.Month, APP_ENUMS.DATE_TIME_OPTIONS.CustomTime];
  }

  onTableFilter(filterValue: any, property: string) {
    let filterItem = {
      comparision: 0,
      propertyName: property,
      value: filterValue
    }
    this.onFilter.emit(filterItem)
  }

  onTableMultiSelectWithConditionFilter(teamsFilterInput:TeamsFilterInputDto){
    this.onMultiFilterWithCondition.emit(teamsFilterInput);
  }

  onTableMultiSelectFilter(listData: any, property: number){
    let filterParam = {
      value : listData,
      property: property
    }
    this.onMultiFilter.emit(filterParam);
  }

  onTableFilterBySeniority(listData: SeniorityFilterDto){
    this.onFilterBySeniority.emit(listData);
  }

  onSearchFilter(searchValue: string) {
    this.onSearch.emit(searchValue)
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

  private getAllJobPositon() {
    this.subscription.push(this.positionService.getAll().subscribe(rs => {
      this.positionList = this.mapToFilter(rs.result, true)
    }))
  }

  private getAllTeam() {
    this.subscription.push(this.teamService.getAll().subscribe(rs => {
      this.teamList = this.mapToFilter(rs.result,true)
    }))
  }

  public setDefaultValue() {
    let defaultTeam = this.route.snapshot.queryParamMap.get("teamId")
    this.defaultValue = {
      branch: AppConsts.DEFAULT_ALL_FILTER_VALUE,
      userLevel: AppConsts.DEFAULT_ALL_FILTER_VALUE,
      status: APP_ENUMS.UserStatus.Working,
      jobPosition: AppConsts.DEFAULT_ALL_FILTER_VALUE,
      team: defaultTeam ? defaultTeam : AppConsts.DEFAULT_ALL_FILTER_VALUE,
      gender: AppConsts.DEFAULT_ALL_FILTER_VALUE,
      userType: AppConsts.DEFAULT_ALL_FILTER_VALUE,
      birthday: APP_ENUMS.DATE_TIME_OPTIONS.All
    } as DefaulEmployeeFilterDto
  }
  public onSelectClick() {
    this.onSelect.emit()
  }

  onChangeFilterDaysLeftOfContractEnd(){
    this.onChangeDaysLeftOfContractEnd.emit({
      daysLeftContractEnd: this.daysLeftContractEnd
    });
  }

  public onCancelClick() {
    this.onCancel.emit()
  }

  public handleDateSelectorChange(value){
    this.onDateSelectorChange.emit(value)
  }
}
export interface DefaulEmployeeFilterDto {
  userType: any;
  userLevel: any;
  status: any;
  jobPosition: any;
  team: any;
  branch: any;
  gender: any;
  birthday: any;
}
