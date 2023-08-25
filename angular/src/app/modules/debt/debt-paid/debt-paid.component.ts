import {
  Component,
  EventEmitter,
  Injector,
  Input,
  OnInit,
  Output,
} from "@angular/core";
import { ActivatedRoute } from "@angular/router";
import { DebtPaidService } from "@app/service/api/debt/debtPaid.service";
import { AppComponentBase } from "@shared/app-component-base";
import { DebtPaid } from "../models/debt";
import { PAYMENT_METHOD } from "../models/paymentType";
import { EDebtStatus } from "@app/service/model/debt/debt.dto";
import { finalize } from 'rxjs/operators'
import { PERMISSIONS_CONSTANT } from "@app/permission/permission";
@Component({
  selector: "app-debt-paid",
  templateUrl: "./debt-paid.component.html",
  styleUrls: ["./debt-paid.component.css"],
})
export class DebtPaidComponent extends AppComponentBase implements OnInit {
  @Input("debtPaidList") debtPaidList: DebtPaid[] = [];
  @Input("totalDebtPlan") totalDebtPlan: number = 0;
  @Input("totalDebtPaid") totalDebtPaid: number = 0;
  @Input('debtStatus') debtStatus: EDebtStatus
  @Input('paymentType') paymentType: number
  @Output("onSavePaid") onSavePaid: EventEmitter<boolean> = new EventEmitter();
  public PAYMENT_METHOD = PAYMENT_METHOD;
  public PAYMENT_METHOD_LIST = Object.values(PAYMENT_METHOD);
  public isEdit: boolean = false;
  public debtId: number;
  public EDebtStatus = EDebtStatus
  public isError: boolean = false
  constructor(
    injector: Injector,
    private debtPaidService: DebtPaidService,
    private activatedRoute: ActivatedRoute
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.debtId = this.activatedRoute.snapshot.params["id"];
  }

  onEdit(debtPaid) {
    debtPaid.isEdit = true;
    this.isEdit = true;
  }

  onCancel(index, item: DebtPaid) {
    if ('id' in item) {
      this.onSavePaid.emit(false)
    } else {
      this.debtPaidList.splice(index, 1);
    }
  }

  addNewDebt() {
    const newPayment = {
      date: this.formatDateYMD(new Date),
      money: null,
      note: "",
      paymentType: this.paymentType,
      isEdit: true,
      debtId: this.debtId,
    } as DebtPaid
    this.debtPaidList.unshift(newPayment)
    this.isEdit = true;
  }

  onSave(debtPaid: DebtPaid) {
    debtPaid.date = this.formatDateYMD(debtPaid.date)
    this.isLoading = true;
    if (debtPaid.id) {
      this.subscription.push(
        this.debtPaidService.update(debtPaid).pipe(finalize(() => this.onCompletedCall())).subscribe((rs) => {
          this.notify.success("Debtpaid updated");
          debtPaid.isEdit = false;
          this.isEdit = false;
          this.onSavePaid.emit(true);
        })
      );
    } else {
      this.subscription.push(
        this.debtPaidService.create(debtPaid).pipe(finalize(() => this.onCompletedCall())).subscribe((rs) => {
          this.notify.success("Debtpaid created");
          debtPaid.isEdit = false;
          this.isEdit = false;
          this.onSavePaid.emit(true);
        })
      );
    }
  }

  onDelete(debtPaid: DebtPaid) {
    abp.message.confirm(`Delete debt payment id:${debtPaid.id}`, "", (result, info) => {
      if (result) {
        this.isLoading = true;
        this.subscription.push(
          this.debtPaidService.delete(debtPaid.id)
            .pipe(finalize(() => {
              this.onCompletedCall()
            })).subscribe(rs => {
              debtPaid.isEdit = false;
              this.notify.success("Detbpaid deleted");
              this.onSavePaid.emit(true);
            }));
      }
    })
  }

  canChangePaymentType() {
    return this.debtPaidList.some(paid => paid.id)
  }

  canSave(item: DebtPaid) {
    if (this.isLoading) return false;
    if (Number.isNaN(item.money)) return false;
    if (!item.money || Number(item.money) == 0 || !item.date) return false;
    return true;
  }

  isShowAddDebtPaidBtn() {
    return this.isGranted(PERMISSIONS_CONSTANT.Debt_DebtDetail_AddDebtPaid);
  }

  isShowDeleteDebtPaidBtn() {
    return this.isGranted(PERMISSIONS_CONSTANT.Debt_DebtDetail_DeleteDebtPaid);
  }

  isShowEditDebtPaidBtn() {
    return this.isGranted(PERMISSIONS_CONSTANT.Debt_DebtDetail_EditDebtPaid);
  }

  onCompletedCall() {
    this.isLoading = false;
  }
}
