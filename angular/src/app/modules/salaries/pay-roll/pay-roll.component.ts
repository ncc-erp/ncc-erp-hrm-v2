import { MatMenuTrigger } from '@angular/material/menu';
import { APP_ENUMS } from '@shared/AppEnums';
import { PaymentType } from './../../debt/models/paymentType';
import { AddEditPayRollComponent } from './add-edit-pay-roll/add-edit-pay-roll.component';
import { PayRollDto } from './../../../service/model/salaries/salaries.dto';
import { PayRollService } from './../../../service/api/pay-roll/pay-roll.service';
import { PagedListingComponentBase, PagedRequestDto } from '@shared/paged-listing-component-base';
import { Component, OnInit, Injector, ViewChild } from '@angular/core';
import * as moment from 'moment';
import { AppConsts } from '@shared/AppConsts';
import { PERMISSIONS_CONSTANT } from '@app/permission/permission';

@Component({
  selector: 'app-pay-roll',
  templateUrl: './pay-roll.component.html',
  styleUrls: ['./pay-roll.component.css']
})
export class PayRollComponent extends PagedListingComponentBase<any> implements OnInit {
  @ViewChild(MatMenuTrigger)
  public menu: MatMenuTrigger;
  public contextMenuPosition = { x: '0px', y: '0px' };
  public listPayRolls: PayRollDto[] = [];
  public listDates: Object[] = [];
  public DEFAULT_FILTER = {
    applyMonth: AppConsts.DEFAULT_ALL_FILTER_VALUE
  }
  protected list(request: PagedRequestDto, pageNumber: number, finishedCallback: Function): void {
    this.subscription.push(
      this.payRollService.getAllPagging(request).subscribe((rs) => {
        this.listPayRolls = rs.result.items;
        this.showPaging(rs.result, pageNumber)
      })
    )
  }
  constructor(injector: Injector, private payRollService: PayRollService) { super(injector) }

  ngOnInit(): void {
    this.refresh();
    this.getListDateFromPayroll();
    this.listBreadCrumb = [
      { name: '<i class="fa-solid fa-house fa-sm"></i>', url: '' },
      { name: ' <i class="fa-solid fa-chevron-right"></i> ' },
      { name: ' Payroll ' }];
  }

  public onCreate() {
    const dialog = this.dialog.open(AddEditPayRollComponent, {
      width: "700px"
    })
    dialog.afterClosed().subscribe(res => {
      if (res) {
        this.refresh()
      }
    })
  }
  public onUpdate(payRoll: PayRollDto) {
    this.dialog.open(AddEditPayRollComponent, {
      data: {
        payRoll: payRoll,
      },
      width: "700px"
    }).afterClosed().subscribe(rs => {
      if(rs) {
        this.refresh()
      }
    })
  }

  public onDelete(payRoll: PayRollDto) {
    let applyMonth = moment(payRoll.applyMonth).format('MM/YYYY');
    this.confirmDelete(`Delete payroll <strong>${applyMonth}</strong>`,
      () => this.payRollService.delete(payRoll.id).toPromise().then(rs => {
        abp.notify.success(`Deleted payroll ${applyMonth}`);
      }))
  }

  public onChangeStatus(id: number, status: number) {
    let input = {
      payrollId: id,
      status: status
    }
    this.subscription.push(
      this.payRollService.changeStatus(input).subscribe((rs) => {
        abp.message.success(`Change status successful`+ `<p style="margin-top: 10px;word-wrap: normal;font-weight: 400;font-size: 20px;color: #dc3545;">${rs.result}</p>`);
        this.refresh();
      })
    )
  }

  public getListDateFromPayroll() {
    this.subscription.push(
      this.payRollService.GetListDateFromPayroll().subscribe((data) => {
        this.listDates = data.result.map(x => {
          return {
            key: moment(x).format("MM/YYYY"),
            value: moment(x).format("YYYY/MM"),
          }
        });
        this.AddFilterAll(this.listDates);
      })
    )
  }

  private AddFilterAll(listData: Object[]) {
    listData.unshift({ key: "All", value: AppConsts.DEFAULT_ALL_FILTER_VALUE })
  }

  public executePayroll(payroll:PayRollDto){
    abp.message.confirm(
      `Execute payroll: <strong>${moment(payroll.applyMonth).format("MM/YYYY")}</strong>`,
      "",
      async (result: boolean) => {
        if (result) {
          this.subscription.push(
            this.payRollService.ExecuatePayroll(payroll.id).subscribe(rs=> {
              abp.message.success("Payroll executed" + `<p style="margin-top: 10px; word-wrap: normal;font-weight: 400;font-size: 20px;color: #dc3545;">${rs.result}</p>`);
              this.refresh();
            }))
        }
      },
      true
    )
  }

  isShowCreateBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Payroll_Create);
  }
  isShowEditBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Payroll_Edit);
  }
  isShowDeleteBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Payroll_Delete);
  }
  isShowSendToAccountantBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Payroll_SendToAccountant);
  }
  isShowApproveAndSendtToCEOBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Payrol_ApproveAndSendtToCEO);
  }
  isShowRejectByKTBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Payroll_RejectByKT);
  }
  isShowApproveByKTBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Payroll_ApproveByKT);
  }
  isShowRejectByCEOBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Payroll_RejectByCEO);
  }
  isShowApproveByCEOBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Payroll_ApproveByCEO);
  }
  isShowExecuteBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Payroll_Execute);
  }
  isAllowRoutingDetail(){
    return this.isGranted(PERMISSIONS_CONSTANT.Payroll_Payslip_View);
  }
}
