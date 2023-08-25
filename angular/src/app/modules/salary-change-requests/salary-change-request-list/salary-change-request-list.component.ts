import { Component, Injector, OnInit } from '@angular/core';
import { PERMISSIONS_CONSTANT } from '@app/permission/permission';
import { SalaryChangeRequestService } from '@app/service/api/salary-change-request/salary-change-request.service';
import { ESalaryChangeRequestStatus, GetSalaryChangeRequestDto, SalaryChangeRequest, SalaryChangeRequestStatusList } from '@app/service/model/salary-change-request/GetSalaryChangeRequestDto';
import { UpdateChangeRequestDto } from '@app/service/model/salary-change-request/UpdateChangeRequestDto';
import { DEFAULT_FILTER_VALUE, PagedListingComponentBase, PagedRequestDto } from '@shared/paged-listing-component-base';
import * as moment from 'moment';
import { CreateSalaryChangeRequestComponent } from '../create-salary-change-request/create-salary-change-request.component'
@Component({
  selector: 'app-salary-change-request-list',
  templateUrl: './salary-change-request-list.component.html',
  styleUrls: ['./salary-change-request-list.component.css']
})
export class SalaryChangeRequestListComponent extends PagedListingComponentBase<GetSalaryChangeRequestDto> implements OnInit {
  public listRequest: GetSalaryChangeRequestDto[] = []
  public SALARY_CHANGE_REQUEST_STATUS = ESalaryChangeRequestStatus
  public SalaryChangeRequestStatus = SalaryChangeRequestStatusList
  public listStatusFilter = Object.values(SalaryChangeRequestStatusList)
  public listDate = []
  public defaultFilterValue = {
    status: this.APP_CONST.DEFAULT_ALL_FILTER_VALUE,
    applyMonth: this.APP_CONST.DEFAULT_ALL_FILTER_VALUE
  }
  listBreadCrumb = [
    { name: '<i class="fa-solid fa-house fa-sm"></i>', url: '' },
    { name: ' <i class="fa-solid fa-chevron-right"></i> ' },
    { name: 'Salary change request' }
  ]
  protected list(request: PagedRequestDto, pageNumber: number, finishedCallback: Function): void {
    this.subscription.push(
      this.salaryChangeRequestService.getAllPagging(request).subscribe(rs => {
        this.listRequest = rs.result.items;
        this.showPaging(rs.result, pageNumber)
        this.isLoading = false;
      }))
  }

  constructor(injector: Injector, private salaryChangeRequestService: SalaryChangeRequestService) {
    super(injector)
  }

  ngOnInit(): void {
    this.bindDefaultFilter()
    this.refresh()
    this.listStatusFilter.unshift({ key: 'All', value: this.APP_CONST.DEFAULT_ALL_FILTER_VALUE, color: '', class: '' })
    this.salaryChangeRequestService.getListDateFromSalaryRequest().subscribe(rs => {
      this.listDate = rs.result.map(x => {
        return {
          key: moment(x).format("MM/YYYY"),
          value: moment(x).format("YYYY/MM/DD"),
        }
      })
      this.listDate.unshift({ key: 'All', value: this.APP_CONST.DEFAULT_ALL_FILTER_VALUE })
    })
  }

  onCreate() {
    this.dialog.open(CreateSalaryChangeRequestComponent, {
      width: '600px'
    }).afterClosed().subscribe(rs => {
      this.refresh()
    })
  }

  onEdit(request: SalaryChangeRequest) {
    this.dialog.open(CreateSalaryChangeRequestComponent, {
      data: request,
      width: '600px'
    }).afterClosed().subscribe(rs => {
      this.refresh()
    })
  }

  update(request: SalaryChangeRequest, status: ESalaryChangeRequestStatus) {
    let payload: UpdateChangeRequestDto = {
     requestId: request.id,
     status: status
    }
    if (status == ESalaryChangeRequestStatus.Executed) {
      abp.message.confirm(`Execute ${request.name}`, "", (result) => {
        if (result) {
          this.salaryChangeRequestService.updateRequestStatus(payload).subscribe(rs => {
            this.notify.success("Salary change request executed")
            this.refresh()
          })
        }
      })
    }
    else {
      this.salaryChangeRequestService.updateRequestStatus(payload).subscribe(rs => {
        this.notify.success("Salary change request updated")
        this.refresh()
      })
    }
  }
  delete(request: GetSalaryChangeRequestDto) {
    this.confirmDelete(`Delete request <strong>${request.name}</strong>`, () => {
      this.subscription.push(
        this.salaryChangeRequestService.delete(request.id).subscribe(rs => {
          this.notify.success("Salary change request deleted")
          this.refresh()
        })
      )
    })
  }

  isShowCreateBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.SalaryChangeRequest_Create);
  }
  isShowEditBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.SalaryChangeRequest_Edit);
  }
  isShowDeleteBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.SalaryChangeRequest_Delete);
  }
  isShowSendBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.SalaryChangeRequest_Send);
  }
  isShowApproveBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.SalaryChangeRequest_Approve);
  }
  isShowRejectBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.SalaryChangeRequest_Reject);
  }
  isShowExecuteBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.SalaryChangeRequest_Execute);
  }
  isAllowRoutingDetail(){
    return this.isGranted(PERMISSIONS_CONSTANT.SalaryChangeRequest_SalaryChangeRequestDetail_View);
  }

  bindDefaultFilter() {
    if(this.filterItems.length) {
      this.filterItems.forEach(filterItem => {
        console.log(filterItem)
        this.defaultFilterValue[filterItem.propertyName] = filterItem.value;
      })
    } else {
      this.sortDirection = this.APP_ENUM.SortDirectionEnum.Descending;
      this.sortProperty = 'applyMonth'
    }
  }
}
