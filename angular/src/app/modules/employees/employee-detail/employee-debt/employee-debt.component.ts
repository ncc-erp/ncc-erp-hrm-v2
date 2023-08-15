import { Component, Injector, OnInit } from '@angular/core';
import { DebtService } from '@app/service/api/debt/debt.service';
import { DebtDto } from '@app/service/model/debt/debt.dto';
import { PagedListingComponentBase, PagedRequestDto } from '@shared/paged-listing-component-base';
import { PAYMENT_METHOD, EPaymentType } from '@app/modules/debt/models/paymentType'
import { DEBT_STATUS } from '@app/modules/debt/models/debtStatus';
import { EDebtStatus } from '@app/service/model/debt/debt.dto'
import * as moment from 'moment';
import { PERMISSIONS_CONSTANT } from '@app/permission/permission';
import { finalize } from 'rxjs/operators';
@Component({
  selector: 'app-employee-debt',
  templateUrl: './employee-debt.component.html',
  styleUrls: ['./employee-debt.component.css']
})
export class EmployeeDebtComponent extends PagedListingComponentBase<DebtDto> implements OnInit {
  public employeeDebtList: DebtDto[] = []
  public employeeId: number
  PAYMENT_TYPE = EPaymentType
  PAYMENT_METHOD = PAYMENT_METHOD
  PAYMENT_METHOD_FILTER = Object.values(PAYMENT_METHOD)
  EDEBT_STATUS = EDebtStatus
  DEBT_STATUS_LIST = DEBT_STATUS
  STATUS_FILTER = Object.values(DEBT_STATUS)
  public columnList = [
    {
      name: "money",
      displayName: "Principal",
      isShow: true,
      className: 'col-principal',
      sortable: true
    },
    {
      name: "APR",
      displayName: "APR(%)",
      isShow: true,
      className: 'col-apr',
      sortable: false
    },
    {
      name: "Interest",
      displayName: "Interest",
      isShow: true,
      className: 'col-interest',
      sortable: false
    },
    {
      name: "Paid",
      displayName: "Paid",
      isShow: true,
      className: 'col-paid',
      sortable: false
    },
    {
      name: "Remaining",
      displayName: "Remaining",
      isShow: true,
      className: 'col-remaining',
      sortable: false
    },
    {
      name: "Payment method",
      displayName: "Method",
      isShow: true,
      className: 'col-paymentMethod',
      sortable: false
    },
    {
      name: "Start date",
      displayName: "Start date",
      isShow: true,
      className: 'col-startDate',
      sortable: false
    },
    {
      name: "End date",
      displayName: "End date",
      isShow: true,
      className: 'col-endDate',
      sortable: false
    },
    {
      name: "Status",
      displayName: "Status",
      isShow: true,
      className: 'col-status',
      sortable: false
    },
    {
      name: "Note",
      displayName: "Note",
      isShow: true,
      className: 'col-note',
      sortable: false
    }
  ]
  constructor(injector: Injector, private debtService: DebtService) {
    super(injector)
  }
  protected list(request: PagedRequestDto, pageNumber: number, finishedCallback: Function): void {
    if(this.isAllowViewTabDebt()){
      this.subscription.push(
        this.debtService.getAllDebtByEmployee(this.employeeId,request).pipe(finalize(() => finishedCallback())).subscribe(rs => {
          this.employeeDebtList = rs.result.items.map(debt => { debt.interest = this.calculateInterest(debt); return debt })
          this.showPaging(rs.result, pageNumber)
        })
      )
    }
  }
  calculateInterest(debt: DebtDto): number {
    const dayDiff = Math.ceil(Math.abs(moment(debt.startDate).diff(moment(debt.endDate), 'day')))
    if (dayDiff < 0) return 0;
    const ratePerDay = (debt.interestRate / 365) * dayDiff;
    const interest = (ratePerDay * debt.money) / 100;
    return Math.ceil(interest)
  }
  ngOnInit(): void {
    this.employeeId = this.activatedRoute.snapshot.queryParams["id"]
    this.filterItems = [...this.filterItems, {
      propertyName: 'debtStatus',
      comparision: this.APP_ENUM.filterComparison.EQUAL,
      value: EDebtStatus.Inprogress
    }]
    this.PAYMENT_METHOD_FILTER.unshift({ key: 'All', value: this.APP_CONST.DEFAULT_ALL_FILTER_VALUE })
    this.STATUS_FILTER.unshift({
      key: 'All', value: this.APP_CONST.DEFAULT_ALL_FILTER_VALUE,
      color: ''
    })
    this.refresh()
  }
  
  isShowAddBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Employee_EmployeeDetail_TabDebt_Add);
  }

  isAllowViewTabDebt(){
    return this.isGranted(PERMISSIONS_CONSTANT.Employee_EmployeeDetail_TabDebt_View);
  }

}
