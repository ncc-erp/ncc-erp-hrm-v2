import { Component, Injector, OnInit, ViewChild } from '@angular/core';
import { MatMenuTrigger } from '@angular/material/menu';
import { PERMISSIONS_CONSTANT } from '@app/permission/permission';
import { WarningEmployeeService } from '@app/service/api/warning-employee/warning-employee.service';
import { TempEmployeeTsDto } from '@app/service/model/employee/employee.dto';
import { GetInputFilterDto, GetInputFilterRequestUpdateInfoDto } from '@app/service/model/employee/GetEmployeeExcept.dto';
import { APP_ENUMS } from '@shared/AppEnums';
import { PagedListingComponentBase, PagedRequestDto } from '@shared/paged-listing-component-base';

@Component({
  selector: 'app-temp-employee-ts',
  templateUrl: './temp-employee-ts.component.html',
  styleUrls: ['./temp-employee-ts.component.css']
})
export class TempEmployeeTsComponent extends PagedListingComponentBase<TempEmployeeTsComponent> implements OnInit {
  constructor(injector: Injector,
    private warningEmployeeService: WarningEmployeeService) {
    super(injector);
  }
  @ViewChild(MatMenuTrigger)
  public menu: MatMenuTrigger;
  public contextMenuPosition = { x: '0px', y: '0px' };
  public listEmployeeRequests: TempEmployeeTsDto[] = [];
  public listRequestStatuses = [];
  public requestStatuses = [];
  public filterMultipleTypeParamEnum = APP_ENUMS.FilterMultipleTypeParamEnum;

  protected list(request: PagedRequestDto, pageNumber: number, finishedCallback: Function): void {
    let input = {
      gridParam: request,
      requestStatuses: this.requestStatuses
    } as GetInputFilterRequestUpdateInfoDto;
    this.isLoading = true;
    this.subscription.push(
      this.warningEmployeeService.getRequestUpdateInfo(input)
        .subscribe((rs) => {
          this.listEmployeeRequests = rs.result.items;
          this.showPaging(rs.result, pageNumber);
          this.isLoading = false;
        }, () => this.isLoading = false)
    )
  }

  ngOnInit(): void {
    this.refresh();
    this.listRequestStatuses = this.getListFormEnum(APP_ENUMS.RequestUpdateInfoStatus, true);
    this.listBreadCrumb = [
      { name: '<i class="fa-solid fa-house fa-sm"></i>', url: '' },
      { name: ' <i class="fa-solid fa-chevron-right"></i> ' },
      { name: ' Request Change Info' }];
    this.bindDefaultFilter()
  }

  public onTableMultiSelectFilter(listData: any, property: number) {
    let filterParam = {
      value: listData,
      property: property
    }
    this.onMultiFilter(filterParam);
  }

  public onMultiFilter(dataMultiFilter: any) {
    switch (dataMultiFilter.property) {

      case this.filterParamType.RequestUpdateInfoStatus: {
        this.requestStatuses = dataMultiFilter.value;
        break;
      }
    }
    this.refresh();
  }
  
  
  public bindDefaultFilter(){
    this.requestStatuses = [APP_ENUMS.RequestUpdateInfoStatus.Pending];

    if(this.filterItems.length) {
      this.filterItems.forEach(filterItem => {
        this.defaultFilterValue[filterItem.propertyName] = filterItem.value;
      })
    } else {
      this.sortDirection = this.APP_ENUM.SortDirectionEnum.Descending;
      this.sortProperty = 'creationTime'; 
    }
  }
  

  isAllowViewTabPersonalInfo() {
    return this.isGranted(PERMISSIONS_CONSTANT.Employee_EmployeeDetail_TabPersonalInfo_View);
  }
  isShowViewDetailRequestBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.WarningEmployee_RequestChangeInfo_DetailRequest_View);
  }
}
