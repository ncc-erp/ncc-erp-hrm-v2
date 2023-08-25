import { DatePipe } from '@angular/common';
import { Component, Inject, Injector, Input, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { UploadAvatarComponent } from '@app/modules/employees/upload-avatar/upload-avatar.component';
import { EmployeeService } from '@app/service/api/employee/employee.service';
import { GetEmployeeDto } from '@app/service/model/employee/employee.dto';
import { GetInputFilterDto, SeniorityFilterDto } from '@app/service/model/employee/GetEmployeeExcept.dto';
import { APP_ENUMS } from '@shared/AppEnums';
import { FilterDto, PagedListingComponentBase, PagedRequestDto, TeamsFilterInputDto } from '@shared/paged-listing-component-base';
import { finalize } from 'rxjs/operators';
import * as FileSaver from 'file-saver';
import { PERMISSIONS_CONSTANT } from '@app/permission/permission';
import { CreateEmployeeFromFileComponent } from '@app/modules/employees/employee-list/create-employee-from-file/create-employee-from-file.component';
import { UpdateEmployeeFromFileComponent } from '@app/modules/employees/employee-list/update-employee-from-file/update-employee-from-file.component';
@Component({
  selector: 'app-list-employee',
  templateUrl: './list-employee.component.html',
  styleUrls: ['./list-employee.component.css'],
  providers: [DatePipe,
    { provide: MatDialogRef, useValue: {} }
  ]
})
export class ListEmployeeComponent extends PagedListingComponentBase<GetEmployeeDto> implements OnInit {
  public columnList = [
    {
      name: "Stt",
      displayName: "#",
      isShow: true
    },
    {
      name: "Employee",
      displayName: "Employee",
      isShow: true
    },
    {
      name: "Birthday",
      displayName: "Birthday",
      isShow: true
    },
    {
      name: "Status",
      displayName: "Status",
      isShow: true
    },
    {
      name: "Team",
      displayName: "Teams",
      isShow: true
    },
    {
      name: "Seniority",
      displayName: "Seniority",
      isShow: true
    },
    {
      name: "UpdatedTime",
      displayName: "Updated time",
      isShow: true
    }
  ]
  public addedEmployeeIds: number[] = []
  @Input() employeeList: GetEmployeeDto[] = []
  @Input() isOnDialog: boolean
  public selectedEmployees: GetEmployeeDto[] = []
  public allSelected: boolean = false;
  public dataMultiFilter: any = {};
  public isAndCondition: boolean = false;
  public filterParamType = APP_ENUMS.FilterMultipleTypeParamEnum;
  public requestInput = {} as GetInputFilterDto;
  public birthdayFromDate:string;
  public birthdayToDate:string;
  constructor(injector: Injector,
    private datePipe: DatePipe,
    private employeeService: EmployeeService, @Inject(MAT_DIALOG_DATA) public data: any) {
    super(injector)
  }

  ngOnInit(): void {
    if (this.isOnDialog) {
      this.isDialog = true
    }
    this.statusIds = [APP_ENUMS.UserStatus.Working];
    this.addedEmployeeIds = this.data.addedEmployeeIds ? this.data.addedEmployeeIds : []
    this.filterItems = []
    this.bindDefaultFilter()
    this.refresh()
  }


  isShowEditBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Employee_Edit);
  }
  isShowDeleteBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Employee_Delete);
  }
  isShowUploadAvatarBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Employee_UploadAvatar);
  }
  isAllowRoutingDetail(){
    return this.isGranted(PERMISSIONS_CONSTANT.Employee_EmployeeDetail);
  }
  isAllowViewTabPersonalInfo(){
    return this.isGranted(PERMISSIONS_CONSTANT.Employee_EmployeeDetail_TabPersonalInfo_View);
  }

  protected list(request: PagedRequestDto, pageNumber: number, finishedCallback: Function): void {

    let input = {
      addedEmployeeIds: this.addedEmployeeIds,
      teamIds: this.teamIds,
      isAndCondition: this.isAndCondition,
      statusIds: this.statusIds,
      branchIds: this.branchIds,
      levelIds: this.levelIds,
      userTypes: this.userTypes,
      jobPositionIds: this.jobPositionIds,
      seniority: this.seniorityFilterInput,
      birthdayFromDate: this.birthdayFromDate,
      birthdayToDate: this.birthdayToDate,
      gridParam: request
    } as GetInputFilterDto
    this.requestInput = input;
    this.subscription.push(
      this.employeeService.GetEmployeeExcept(input)
        .pipe(finalize(() => {
          finishedCallback()
        })).subscribe(rs => {
          if (this.isOnDialog) {
            this.employeeList = rs.result.items.filter(employee => !this.checkAlreadyAdd(employee.id));

          }
          else {
            this.employeeList = rs.result.items
          }
          this.employeeList.forEach(employee => {
            employee.selected = this.checkSelected(employee.id)
            employee.countSeniority = this.GetStartWorkingTimeStr(employee)
          })
          this.checkAllSelected()
          this.showPaging(rs.result, pageNumber)
        })
    )
  }

  checkSelected(id: number) {
    let selectedIds = this.selectedEmployees.map(x => x.id)
    if (selectedIds.includes(id)) {
      return true
    }
    return false
  }

  private GetStartWorkingTimeStr(employee: GetEmployeeDto): string {
    return employee.startWorkingDate ?
      `<div>
        ${employee.seniority?.years > 0 ? employee.seniority?.years + 'y' : ''}
        ${employee.seniority?.months > 0 ? employee.seniority?.months + 'm' : ''}
        ${employee.seniority?.days > 0 ? employee.seniority?.days + 'd' : ''}
      </div>` : '';
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
  checkAlreadyAdd(id: number) {
    if (this.addedEmployeeIds.includes(id)) {
      return true
    }
    return false
  }

  public onEmployeeSelect(employee: GetEmployeeDto) {
    this.allSelected = this.employeeList != null && this.employeeList.every(t => t.selected);
    if (!employee.selected) {
      this.selectedEmployees = this.selectedEmployees.filter(e => e.id != employee.id)
    }
    else {
      this.selectedEmployees.push(employee)
    }
  }
  public checkAllSelected() {
    this.allSelected = this.employeeList != null && this.employeeList.every(t => t.selected);
  }

  public someComplete(): boolean {
    if (this.employeeList == null) {
      return false;
    }
    return this.employeeList.filter(t => t.selected).length > 0 && !this.allSelected;
  }

  public selectAll(selected: boolean) {
    this.allSelected = selected;
    if (this.employeeList == null) {
      return;
    }
    if (this.allSelected) {
      const listSelectedIds = this.selectedEmployees.map(employee => employee.id)
      const listEmployee = this.employeeList.filter(employee => !listSelectedIds.includes(employee.id))
      this.selectedEmployees.push(...listEmployee)
    }
    else {
      let listEmployeeIds = this.employeeList.map(x => x.id)
      this.selectedEmployees = this.selectedEmployees.filter(employee => !listEmployeeIds.includes(employee.id))
    }
    this.employeeList.forEach(t => (t.selected = selected));
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
    this.teamIds = teamsFilterInput?.teamIds
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

  public onDateSelectorChange(data){
    this.birthdayFromDate = data?.fromDate;
    this.birthdayToDate = data?.toDate;
    this.refresh();
  }

  public onMultiFilter(data: any) {

    this.dataMultiFilter = data;

    switch (this.dataMultiFilter.property) {

      case this.filterParamType.Status: {
        this.statusIds = this.dataMultiFilter.value;
        break;
      }

      case this.filterParamType.UserType: {
        this.userTypes = this.dataMultiFilter.value;
        break;
      }

      case this.filterParamType.UserLevel: {
        this.levelIds = this.dataMultiFilter.value;
        break;
      }

      case this.filterParamType.Branch: {
        this.branchIds = this.dataMultiFilter.value;
        break;
      }

      case this.filterParamType.JobPosition: {
        this.jobPositionIds = this.dataMultiFilter.value;
        break;
      }

    }
    this.refresh();

  }

  private removeFilterItem(filterItem: FilterDto) {
    this.filterItems = this.filterItems.filter(item => item.propertyName !== filterItem.propertyName)
  }

  public onDelete(employee: GetEmployeeDto) {
    this.confirmDelete(`Delete employee <strong>${employee.fullName}</strong>`, () => {
      this.employeeService.delete(employee.id).subscribe(() => {
        abp.notify.success(`Deleted employee ${employee.fullName}`)
        this.refresh();
      })
    })
  }
  public onUpdateAvatar(employee) {
    const dialogRef = this.dialog.open(UploadAvatarComponent, {
      width: '600px',
      data: employee.id
    })
    dialogRef.afterClosed().subscribe(res => {
      if (res) {
        this.employeeService.uploadAvatar(res, employee.id).subscribe(data => {
          if (data) {
            this.notify.success('Upload Avatar Successfully!');
            this.refresh();
            employee.avatar = data.result;
          } else { this.notify.error('Upload Avatar Failed!'); }
        });
      }
    });
  }

  public onExport() {
    this.requestInput.gridParam.maxResultCount = 50000;
    this.subscription.push(
      this.employeeService.exportEmployee(this.requestInput).subscribe((rs) => {
        const file = new Blob([this.convertFile(atob(rs.result.base64))], {
          type: "application/vnd.ms-excel;charset=utf-8"
        });
        FileSaver.saveAs(file, `List-Employees.xlsx`)
      })

    )
  }
  public getMetaDataToCreate(){
    this.subscription.push(
      this.employeeService.GetDataMetaToCreateEmployeeByFile().subscribe((rs) => {
        const file = new Blob([this.convertFile(atob(rs.result.base64))], {
          type: "application/vnd.ms-excel;charset=utf-8"
        });
        FileSaver.saveAs(file, `Create-Employees.xlsx`)
      })

    )
  }


  public getMetaDataToUpdate(){
    this.subscription.push(
      this.employeeService.GetDataMetaToUpdateEmployeeByFile().subscribe((rs) => {
        const file = new Blob([this.convertFile(atob(rs.result.base64))], {
          type: "application/vnd.ms-excel;charset=utf-8"
        });
        FileSaver.saveAs(file, `Update-Employees.xlsx`)
      })

    )
  }

  someChecked() {
    if (!this.employeeList.length) {
      return false
    }
    return this.employeeList.filter(e => e.selected).length > 0 && !this.allSelected
  }

  onDeselectEmployee(employee: GetEmployeeDto) {
    if (this.employeeList.some(e => e.id == employee.id)) {
      this.allSelected = false
    }
    const employeeInList = this.employeeList.find(x => x.id == employee.id)
    if (employeeInList) {
      employeeInList.selected = false;
    }
    this.selectedEmployees = this.selectedEmployees.filter(e => e.id != employee.id)
  }
  public onCreateEmployeeFromFile(){
    const dialog = this.dialog.open(CreateEmployeeFromFileComponent,{
    })
    dialog.afterClosed().subscribe((rs)=>{
      if(rs){
        this.refresh();
      }
    })
  }
  public onUpdateEmployeeFromFile(){
    const dialog = this.dialog.open(UpdateEmployeeFromFileComponent,{
    })
    dialog.afterClosed().subscribe((rs)=>{
      if(rs){
        this.refresh();
      }
    })
  }

  public exportStatistic(startDate:string, endDate:string){
    this.employeeService.ExportEmployeeStatistic(startDate, endDate).subscribe(rs=>{
      const file = new Blob([this.convertFile(atob(rs.result.base64))], {
        type: "application/vnd.ms-excel;charset=utf-8"
      });
      FileSaver.saveAs(file, `${rs.result.fileName}.xlsx`)
    })
  }
}
