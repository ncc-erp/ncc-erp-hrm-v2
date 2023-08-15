import { Component, EventEmitter, Injector, Input, OnChanges, OnInit, Output } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { calculateTotalMoney } from '../utils/debtUtils';
import { DebtPaymentPlanService } from '@app/service/api/debt/debtPlan.service';
import { AppComponentBase } from '@shared/app-component-base';
import { DebtPlan } from '../models/debt'
import { EDebtStatus } from '@app/service/model/debt/debt.dto';
import { EPaymentType } from '../models/paymentType';
import { PERMISSIONS_CONSTANT } from '@app/permission/permission';
import { finalize } from 'rxjs/operators'
import { startWithTap } from '@shared/helpers/observerHelper';
@Component({
  selector: 'app-debt-plan',
  templateUrl: './debt-plan.component.html',
  styleUrls: ['./debt-plan.component.css']
})
export class DebtPlanComponent extends AppComponentBase implements OnInit, OnChanges {
  @Input('debtPlanList') debtPlanList: DebtPlan[] = []
  @Input('total') total: number = 0
  @Input('paymentType') paymentType: EPaymentType
  @Input('debtStatus') debtStatus: EDebtStatus
  @Output('onGetDebtPlan') onGetDebtPlan: EventEmitter<DebtPlan[]> = new EventEmitter()
  public totalDebtPlan = 0
  public currentDebtPlan = {} as DebtPlan
  public debtId: number
  public isEdit: boolean = false;
  public EDebtStatus = EDebtStatus
  public EPaymentType = EPaymentType
  constructor(
    injector: Injector,
    private debtPlanService: DebtPaymentPlanService,
    private activatedRoute: ActivatedRoute
  ) {
    super(injector)
  }

  ngOnInit(): void {
    this.debtId = this.activatedRoute.snapshot.params['id']
    this.totalDebtPlan = calculateTotalMoney(this.debtPlanList)
    this.getDebtPlan()
  }

  ngOnChanges(changes) {
    if (changes?.debtPlanList) {
      this.totalDebtPlan = calculateTotalMoney(this.debtPlanList)
    }
  }

  addNewPlan() {
    const newPlan = {
      money: 0,
      date: this.formatDateYMD(new Date()),
      isEdit: true,
      note: "",
      debtId: this.debtId,
      paymentType: this.paymentType
    } as DebtPlan
    this.isEdit = true;
    this.debtPlanList.unshift(newPlan)
  }

  onEdit(plan) {
    this.isEdit = true;
    plan.isEdit = true;
  }

  onCancel(index, item: DebtPlan) {
    if ('id' in item && item.id) {
      item.isEdit = false;
      this.getDebtPlan();
    } else {
      this.debtPlanList.splice(index, 1);
    }
  }

  getDebtPlan() {
    this.subscription.push(
      this.debtPlanService.getPaymentPlansByDebtId(this.debtId)
      .pipe(startWithTap(() => this.isLoading = true))
      .pipe(finalize(() => this.isLoading = false))
      .subscribe(rs => {
        this.debtPlanList = rs.result
        this.totalDebtPlan = calculateTotalMoney(rs.result)
        this.onGetDebtPlan.emit(rs.result)
      })
    )
  }

  onSave(debtPlan: DebtPlan) {
    this.isLoading = true;
    debtPlan.date = this.formatDateYMD(debtPlan.date)
    if (debtPlan.id) {
      this.debtPlanService.update(debtPlan)
      .pipe(finalize(() => { this.onCompletedCall(); this.getDebtPlan() }))
      .subscribe({
        next: (value) => {
          debtPlan.isEdit = false;
          this.notify.success("Debt plan updated");
          this.isEdit = false;
        },
      })
    } else {
      this.debtPlanService.create(debtPlan)
      .pipe(finalize(() => { this.onCompletedCall(); this.getDebtPlan() }))
      .subscribe({
        next: (rs) => {
          debtPlan.isEdit = false;
          this.notify.success("Debt plan saved");
          this.isEdit = false;
        },
      })
    }
  }
  onDelete(debtPlanId) {
    abp.message.confirm(`Delete debt plan id: ${debtPlanId}`, '', (result, info) => {
      if (result) {
        this.isLoading = true;
        this.subscription.push(this.debtPlanService.delete(debtPlanId)
          .pipe(finalize(() => {this.onCompletedCall(); this.getDebtPlan()}))
          .subscribe((rs) => {
            this.notify.success("Debt plan deleted");
          }))
      }
    })
  }

  isShowAddPaymentPlanBtn() {
    return this.isGranted(PERMISSIONS_CONSTANT.Debt_DebtDetail_AddPaymentPlan);
  }

  isShowEditPaymentPlanBtn() {
    return this.isGranted(PERMISSIONS_CONSTANT.Debt_DebtDetail_EditPaymentPlan);
  }

  isShowDeletePaymentPlanBtn() {
    return this.isGranted(PERMISSIONS_CONSTANT.Debt_DebtDetail_DeletePaymentPlan);
  }

  onCompletedCall() {
    this.isLoading = false;
  }
}
