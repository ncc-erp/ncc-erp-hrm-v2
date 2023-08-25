import { AppConsts } from './../../../../shared/AppConsts';
import { finalize } from 'rxjs/operators';
import { RefundsService } from './../../../service/api/refunds/refunds.service';
import { RefundDto } from './../../../service/model/refunds/refund.dto';
import { Injector } from '@angular/core';
import { Component, OnInit } from '@angular/core';
import { PagedListingComponentBase, PagedRequestDto } from '@shared/paged-listing-component-base';
import { BreadCrumbDto, filterDataDto } from '@app/service/model/common.dto';
import { APP_ENUMS } from '@shared/AppEnums';
import { CreateEditRefundDialogComponent } from '../create-edit-refund-dialog/create-edit-refund-dialog.component';
import { PERMISSIONS_CONSTANT } from '@app/permission/permission';
import * as moment from 'moment';

@Component({
  selector: 'app-list-refund',
  templateUrl: './list-refund.component.html',
  styleUrls: ['./list-refund.component.css']
})
export class ListRefundComponent extends PagedListingComponentBase<RefundDto> implements OnInit {
  public refundList: RefundDto[] = []
  listBreadCrumb: BreadCrumbDto[] = [
    { name: '<i class="fa-solid fa-house fa-sm"></i>', url: '' },
    { name: ' <i class="fa-solid fa-chevron-right"></i> ' },
    { name: ' Refund ' }
  ];
  public DEFAULT_FILTER = {
    isActive: APP_ENUMS.PunishmentStatus.Active,
    date: -1
  }

  public listDate: filterDataDto[] = []

  public refundStatusList: filterDataDto[] = [
    {
      key: "All",
      value: -1
    },
    {
      key: "Active",
      value: 1
    },
    {
      key: "InActive",
      value: 0
    }
  ]

  protected list(request: PagedRequestDto, pageNumber: number, finishedCallback: Function): void {
    this.subscription.push(
      this.refundService.getAllPagging(request)
        .pipe(finalize(() => {
          finishedCallback();
        }))
        .subscribe(rs => {
          this.refundList = rs.result.items;
          this.showPaging(rs.result, pageNumber)
        })
    )
  }

  constructor(injector: Injector, private refundService: RefundsService) {
    super(injector);
    this.setDefaultFilter({ propertyName: 'isActive', value: 1, comparision: 0 })
  }

  ngOnInit(): void {
    this.getListRefundDate()
    this.refresh()

    this.bindFilterValue()

  }


  private getListRefundDate() {
    this.subscription.push(
      this.refundService.GetListRefundDate().subscribe(rs => {
        this.listDate = rs.result.map(x => {
          return {
            key: x,
            value: x
          } as filterDataDto
        })
        this.AddFilterAll(this.listDate)
      }))
  }

  public onCreate() {
    let ref = this.dialog.open(CreateEditRefundDialogComponent, {
      width: "500px"
    })
    ref.afterClosed().subscribe(rs => {
      if (rs) {
        this.refresh()
        this.getListRefundDate()
      }
    })

  }

  public onUpdate(refund: RefundDto) {
    var data = {...refund} as RefundDto
    let ref = this.dialog.open(CreateEditRefundDialogComponent, {
      width: "500px",
      data: data
    })
    ref.afterClosed().subscribe(rs => {
      if (rs) {
        this.refresh()
        this.getListRefundDate()
      }
    })

  }

  public onDelete(refund: RefundDto) {
    abp.message.confirm(`Delete refund: <strong>${refund.name}<strong>?`, "", (rs) => {
      if (rs) {
        this.subscription.push(this.refundService.delete(refund.id).subscribe(() => {
          abp.notify.success(`Deleted refund <strong>${refund.name}</strong>`)
          this.refresh()
        }))
      }
    }, true)
  }

  private bindFilterValue() {
    this.filterItems.forEach(filterItem => {
      this.DEFAULT_FILTER[filterItem.propertyName] = filterItem.value;
    })
  }

  private AddFilterAll(listData: any[]) {
    listData.unshift({ key: "All", value: AppConsts.DEFAULT_ALL_FILTER_VALUE })
  }

  public onActive(refund:RefundDto){
    abp.message.confirm(`Active refund <strong>${refund.name}</strong>`,"", (rs)=>{
      if(rs){
        this.refundService.ActiveRefund(refund.id).subscribe(rs=>{
          abp.notify.success(`Successful active refund: ${refund.name}`)
          this.refresh()
        })
      }
    },true)
  }

  public onDeactive(refund:RefundDto){
    abp.message.confirm(`Deactive refund: <strong>${refund.name}</strong>`,"", (rs)=>{
      if(rs){
        this.refundService.DeActiveRefund(refund.id).subscribe(rs=>{
          abp.notify.success(`Successful deactive refund: ${refund.name}`)
          this.refresh()
        })
      }
    }, true)
  }

  isShowCreateBtn() {
    return this.isGranted(PERMISSIONS_CONSTANT.Refund_Create);
  }

  isShowDeactiveBtn() {
    return this.isGranted(PERMISSIONS_CONSTANT.Refund_Deactive);
  }

  isShowActiveBtn() {
    return this.isGranted(PERMISSIONS_CONSTANT.Refund_Active);
  }

  isShowEditBtn() {
    return this.isGranted(PERMISSIONS_CONSTANT.Refund_Edit);
  }

  isShowDeleteBtn() {
    return this.isGranted(PERMISSIONS_CONSTANT.Refund_Delete);
  }
  isViewDetail(){
    return this.isGranted(PERMISSIONS_CONSTANT.Refund_RefundDetail);
  }
}
