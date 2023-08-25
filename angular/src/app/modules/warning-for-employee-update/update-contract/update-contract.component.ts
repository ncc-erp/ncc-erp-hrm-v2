import { Component, Injector, OnInit } from '@angular/core';
import { EmployeeService } from '@app/service/api/employee/employee.service';
import { ContractEmployeeExpiredDto, GetEmployeeDto } from '@app/service/model/employee/employee.dto';
import { GetInputFilterDto, SeniorityFilterDto } from '@app/service/model/employee/GetEmployeeExcept.dto';
import { FilterDto, PagedListingComponentBase, PagedRequestDto, TeamsFilterInputDto } from '@shared/paged-listing-component-base';
import { finalize } from 'rxjs/operators';
import { WarningEmployeeService } from '@app/service/api/warning-employee/warning-employee.service';
import { APP_ENUMS } from '@shared/AppEnums';
import { UserTypeDto } from '@app/service/model/categories/userType.dto';
import { BranchDto } from '@app/service/model/categories/branch.dto';
import { JobPositionDto } from '@app/service/model/categories/jobPosition.dto';
import { UserTypeService } from '@app/service/api/categories/userType.service';
import { JobPositionService } from '@app/service/api/categories/jobPosition.service';
import { BranchService } from '@app/service/api/categories/branch.service';
import { PERMISSIONS_CONSTANT } from '@app/permission/permission';
@Component({
  selector: 'app-update-contract',
  templateUrl: './update-contract.component.html',
  styleUrls: ['./update-contract.component.css']
})
export class UpdateContractComponent extends PagedListingComponentBase<UpdateContractComponent> implements OnInit {
  protected list(request: PagedRequestDto, pageNumber: number, finishedCallback: Function): void {
    let input = {
      branchIds: this.branchIds,
      userTypes: this.userTypes,
      jobPositionIds: this.jobPositionIds,
      daysLeftContractEndDate: this.daysLeftContractEnd,
      gridParam: request
    } as GetInputFilterDto;

    this.subscription.push(
      this.warningEmployeeService.GetAllEmployeesToUpdateContract(input)
      .pipe(finalize(() => {
        finishedCallback();
      }))
      .subscribe((rs)=>{
        this.employeeList = rs.result.items;
        this.employeeList.forEach((employee)=>{
          if(employee.contractEndDate){
            employee.contractDayLeft = this.GetContractDaysLeft(employee.contractEndDate);
          }else{
            employee.contractDayLeft  = null;
          }
        })
        this.showPaging(rs.result, pageNumber)
      })
    )
  }

  constructor(injector: Injector,
    private userTyService: UserTypeService,
    private positionService: JobPositionService,
    private branchService: BranchService,
    private warningEmployeeService : WarningEmployeeService) {
    super(injector);
  }
  public employeeList: ContractEmployeeExpiredDto[] = [];
  public userTypeList: UserTypeDto[] = [];
  public branchList: BranchDto[] = []
  public positionList: JobPositionDto[] = []
  public daysLeftContractEnd:number = 20;
  public currentDate = new Date();
  public filterMultipleTypeParamEnum = APP_ENUMS.FilterMultipleTypeParamEnum;
  ngOnInit(): void {
    this.refresh();
    this.listBreadCrumb = [
      {name: '<i class="fa-solid fa-house fa-sm"></i>',url:''}, 
      {name: ' <i class="fa-solid fa-chevron-right"></i> '},
      {name:' Update Contract'}];
    this.getAllUserType();
    this.getAllJobPositon();
    this.getAllBranch();
  }

  public GetContractDaysLeft(endDate){
    let date = new Date(endDate);
    let currentDate = new Date();
    let difference = date.getTime() - currentDate.getTime();
    let TotalDays = Math.ceil(difference / (1000 * 3600 * 24));
    return TotalDays;
  }


  public onFilter(filterItem: FilterDto): void {
    let existFilterItem = this.filterItems.find(x => x.propertyName === filterItem.propertyName)
    if (filterItem.value == this.APP_CONST.DEFAULT_ALL_FILTER_VALUE) {
      this.removeFilterItem(filterItem)
    }
    else if (existFilterItem) {
      existFilterItem.value = filterItem.value
    }
    else {
      this.filterItems.push(filterItem)
    }

    this.refresh()
  }

  public onMultiFilterWithCondition(teamsFilterInput : TeamsFilterInputDto){
    this.teamsId = teamsFilterInput?.teamIds
    this.isAndCondition = teamsFilterInput?.isAndCondition
    this.refresh()
  }

  public onFilterBySeniority(data: SeniorityFilterDto){
    if(data.comparison != null && data.seniorityType != null && data.seniorityValue != ""){
      this.seniorityFilterInput = {
        comparison: data.comparison,
        seniorityType: data.seniorityType,
        seniorityValue: data.seniorityValue
      };
    }else{
      this.seniorityFilterInput = null;
    }
    this.refresh()
  }
  public getTeamsString(employee: GetEmployeeDto){
    return employee.teams.map(team => `<span class="badge badge-team mr-2 text-white">${team.teamName}</span>`).join("")
  }

  public onChangeDaysLeftOfContractEnd(data){
    this.daysLeftContractEnd = data.daysLeftContractEnd;
    this.refresh();
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
  private getAllUserType() {
    this.subscription.push(this.userTyService.getAll().subscribe(rs => {
      this.userTypeList = this.mapToFilter(rs.result, true)
    }))
  }
  isAllowViewTabPersonalInfo(){
    return this.isGranted(PERMISSIONS_CONSTANT.Employee_EmployeeDetail_TabPersonalInfo_View);
  }

}
