import { Component, Injector, OnInit, ViewChild } from '@angular/core';
import { MatMenuTrigger } from '@angular/material/menu';
import { ChangeStatusToWorkingComponent } from '@app/modules/employees/employee-detail/personal-info/change-status/change-status-to-working/change-status-to-working.component';
import { PERMISSIONS_CONSTANT } from '@app/permission/permission';
import { WarningEmployeeService } from '@app/service/api/warning-employee/warning-employee.service';
import { GetEmployeeDto } from '@app/service/model/employee/employee.dto';
import { GetInputFilterDto, SeniorityFilterDto } from '@app/service/model/employee/GetEmployeeExcept.dto';
import { UpdateEmployeeBackDateDto } from '@app/service/model/warning-employee/WarningEmployeeDto';
import { APP_ENUMS } from '@shared/AppEnums';
import { FilterDto, PagedListingComponentBase, PagedRequestDto, TeamsFilterInputDto } from '@shared/paged-listing-component-base';
import { finalize } from 'rxjs/operators';
import { UpdateEmployeeBackDateComponent } from './update-employee-back-date/update-employee-back-date.component';


@Component({
  selector: 'app-back-to-work',
  templateUrl: './back-to-work.component.html',
  styleUrls: ['./back-to-work.component.css']
})
export class BackToWorkComponent extends PagedListingComponentBase<GetEmployeeDto> implements OnInit {
  protected list(request: PagedRequestDto, pageNumber: number, finishedCallback: Function): void {
    let input = {
      statusIds: this.statusIds,
      gridParam: request
    } as GetInputFilterDto;
    this.subscription.push(
      this.warningEmployeeService.getAllEmployeesBackToWork(input)
      .pipe(finalize(() => {
        finishedCallback();
      }))
      .subscribe((rs)=>{
        this.employeeList = rs.result.items;
        this.showPaging(rs.result, pageNumber)
      })
    )
  }

  constructor(
    injector: Injector,
    private warningEmployeeService : WarningEmployeeService) {super(injector) }
    @ViewChild(MatMenuTrigger)
    public menu: MatMenuTrigger;
    public contextMenuPosition = { x: '0px', y: '0px' };
    public employeeList: GetEmployeeDto[] = []
    public statusList = [];
    public filterMultipleTypeParamEnum = APP_ENUMS.FilterMultipleTypeParamEnum;
  ngOnInit(): void {
    this.refresh();
    this.listBreadCrumb = [
      {name: '<i class="fa-solid fa-house fa-sm"></i>',url:''},
      {name: ' <i class="fa-solid fa-chevron-right"></i> '},
      {name:' List employees back to work '}];
      this.statusList = this.getListFormEnum(APP_ENUMS.UserStatus, true).filter(item => item.value != APP_ENUMS.UserStatus.Quit && item.value != APP_ENUMS.UserStatus.Working);
      this.statusIds = [APP_ENUMS.UserStatus.MaternityLeave , APP_ENUMS.UserType.Staff]
    }

  public onBackToWork(employee: GetEmployeeDto){
    const dialog = this.dialog.open(ChangeStatusToWorkingComponent, {
      data:{
        personalInfo: employee,
        isBackToWork: true
      },
      width: '98vw',
      maxHeight: '99vh'
    })
    dialog.afterClosed().subscribe((rs)=>{
      if(rs){
        this.refresh();
      }
    })
  }

  public updateEmployeeBackDate(employee){
    var inputToUpdate = {
      employeeId : employee.id,
      backDate: employee.backDate
    } as UpdateEmployeeBackDateDto;

    var dialog = this.dialog.open(UpdateEmployeeBackDateComponent,
      {
        data : {
          inputToUpdate: inputToUpdate,
          employeeName : employee.fullName
        },
        width: "600px"
      }
    )
    dialog.afterClosed().subscribe((rs)=>{
      this.refresh();
    })
  }

  isShowBackToWorkBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.WarningEmployee_BackToWork_BackToWork);
  }
  isAllowViewTabPersonalInfo(){
    return this.isGranted(PERMISSIONS_CONSTANT.Employee_EmployeeDetail_TabPersonalInfo_View);
  }
  isShowUpdateEmployeeBackDate(){
    return this.isGranted(PERMISSIONS_CONSTANT.WarningEmployee_BackToWork_UpdateEmployeeBackDate);
  }
}
