import { Component, Injector, LOCALE_ID, OnInit } from "@angular/core";
import { Inject } from "@angular/core";
import { GetEmployeeDto } from "@app/service/model/employee/employee.dto";
import {
  AbstractControl,
  FormControl,
  FormGroup,
  Validators,
} from "@angular/forms";
import { EmployeeService } from "@app/service/api/employee/employee.service";
import { DEBT_STATUS } from "../models/debtStatus";
import { EPaymentType, PAYMENT_METHOD } from "../models/paymentType";
import { AppComponentBase } from "@shared/app-component-base";
import { ActivatedRoute, Router } from "@angular/router";
import * as moment from "moment";
import { DebtPlan, DebtPaid, DebtCreateDto } from "../models/debt";
import { DebtDto, EDebtStatus } from "@app/service/model/debt/debt.dto";
import { DebtService } from "@app/service/api/debt/debt.service";
import { DebtPaymentPlanService } from "@app/service/api/debt/debtPlan.service";
import { DebtPaidService } from "@app/service/api/debt/debtPaid.service";
import { mergeMap } from "rxjs/operators";
import {
  calculateTotalMoney,
  validatePaymentDate,
} from "../utils/debtUtils";
import { formatCurrency } from "@angular/common";
import { appModuleAnimation } from "@shared/animations/routerTransition";
import { LayoutStoreService } from "@shared/layout/layout-store.service";
import { PERMISSIONS_CONSTANT } from "@app/permission/permission";
import { finalize } from 'rxjs/operators'
import { cloneDeep } from "lodash-es";
@Component({
  selector: "app-create-edit-view-debt",
  templateUrl: "./create-edit-view-debt.component.html",
  styleUrls: ["./create-edit-view-debt.component.css"],
  animations: [appModuleAnimation()],
})
export class CreateEditViewDebtComponent extends AppComponentBase
  implements OnInit {
  public debt: DebtCreateDto = {} as DebtCreateDto;
  public baseDebt: DebtDto = {} as DebtDto;
  public employeeFilterControl: FormControl = new FormControl();
  public employeeControl: FormControl = new FormControl();
  public employeeList: GetEmployeeDto[] = [];
  public filteredEmployeeList: GetEmployeeDto[] = [];
  public statusOptions = Object.values(DEBT_STATUS);
  public paymentTypes = Object.values(PAYMENT_METHOD);
  public PAYMENT_METHOD = PAYMENT_METHOD;
  public PAYMENT_STATUS = DEBT_STATUS;
  public readonly EDebtStatus = EDebtStatus
  public readonly EPaymentType = EPaymentType;
  public searchEmployee = "";
  public disabled = true;
  public debtPlanList: DebtPlan[] = [];
  public debtPaidList: DebtPaid[] = [];
  public title = "Create new Debt";
  public isEdit = true;
  public debtPlan: DebtPlan[] = [];
  public totalDebt: number;
  public totalDebtPlan: number;
  public totalDebtPaid: number;
  public initListBreadCrumb = [
    { name: '<i class="fa-solid fa-house fa-sm"></i>', url: '' },
    { name: ' <i class="fa-solid fa-chevron-right"></i> ', url: null },
    { name: 'Debt', url: '/app/debt/list-debt' },
    { name: ' <i class="fa-solid fa-chevron-right"></i> ', url: null },
  ];
  public listBreadCrumb = []
  public formGroup: FormGroup
  public isLoading: boolean = false;
  constructor(
    injector: Injector,
    private employeeService: EmployeeService,
    private debtService: DebtService,
    private debtPaymentPlanService: DebtPaymentPlanService,
    private debtPaidService: DebtPaidService,
    private activatedRoute: ActivatedRoute,
    private router: Router,
    private layoutService: LayoutStoreService,
    @Inject(LOCALE_ID) public locale: string,
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.layoutService.setSidebarExpanded(true)
    const id = this.activatedRoute.snapshot.params["id"];
    const employeeParam = this.activatedRoute.snapshot.queryParams["employeeId"]
    this.initForm()
    if (id) {
      this.isEdit = false;
      this.getDebtById(id);
      this.getDebtPaymentPlansByDebtId(id);
      this.getDebtPaidsByDebtId(id);
    } else {
      this.getEmployeeList();
      this.debt.debtStatus = EDebtStatus.Inprogress
      this.debt.interest = 0;
      this.debt.interestRate = 0;
      this.debt.money = 0;
      this.formGroup.patchValue({
        paymentType: EPaymentType.TruLuong,
        status: this.debt.debtStatus,
        interest: this.debt.interest,
        principal: this.debt.money,
        interestRate: this.debt.interestRate,
        employeeId: employeeParam ? Number(employeeParam): null
      });
      if (employeeParam) {
        this.formGroup.controls.employeeId.disable()
      }
      this.listBreadCrumb = [...this.initListBreadCrumb, { name: 'Create Debt', url: null }]
    }
    this.formGroup.valueChanges.subscribe((data) => {
      this.setInterest(this.calculateInterest());
    });
  }

  ngOnDestroy(): void {
    this.layoutService.setSidebarExpanded(false);
  }
  initForm() {
    this.formGroup = new FormGroup({
      startDate: new FormControl(this.formatDateYMD(new Date()), {
        validators: Validators.required,
      }),
      endDate: new FormControl(moment().add(1, "month").toISOString(), {
        validators: Validators.required,
      }),
      employeeId: new FormControl(null, { validators: Validators.required }),
      interestRate: new FormControl(null, { validators: Validators.required }),
      interest: new FormControl(0, { validators: Validators.required }),
      status: new FormControl("", { validators: Validators.required }),
      paymentType: new FormControl(null , { validators: Validators.required }),
      note: new FormControl(""),
      principal: new FormControl(null, { validators: Validators.compose([Validators.required, Validators.min(1)]) }),
    }, {
      validators: [this.checkDates]
    });

  }
  get startDate() {
    return this.formGroup.get("startDate");
  }
  get endDate() {
    return this.formGroup.get("endDate");
  }
  get interestRate() {
    return this.formGroup.get("interestRate");
  }
  get principal() {
    return this.formGroup.get("principal");
  }
  get paymentType() {
    return this.formGroup.get("paymentType");
  }
  get status() {
    return this.formGroup.get("status");
  }
  get interest() {
    return this.formGroup.get("interest");
  }
  get note() {
    return this.formGroup.get("note");
  }
  get employeeId() {
    return this.formGroup.get("employeeId");
  }
  get totalMoney() {
    return this.principal.value + this.interest.value;
  }
  public getDebtById(id) {
    this.subscription.push(
      this.debtService.get(id).subscribe((rs) => {
        this.debt = rs.result;
        this.baseDebt = rs.result;
        this.formGroup.patchValue({
          employeeId: this.debt.employeeId,
          paymentType: this.debt.paymentType,
          status: this.debt.debtStatus,
          principal: this.debt.money,
          interestRate: this.debt.interestRate,
          note: this.debt.note,
          startDate: moment(this.debt.startDate).toISOString(),
          endDate: moment(this.debt.endDate).toISOString(),
        });
        this.listBreadCrumb = [
          ...this.initListBreadCrumb, 
          { 
            name: `#${this.debt.id} ${this.debt.fullName} vay ${formatCurrency(this.debt.money, this.locale, "", "VND", ".0")} VND - ${PAYMENT_METHOD[this.debt.paymentType].key}` 
          }, 
          {
            name: `<span class="ml-2 badge badge-pill ${this.debt.debtStatus == EDebtStatus.Done?"bg-danger":"bg-success"}">${this.debt.debtStatus == EDebtStatus.Done?"Done":"Inprogress"}</span>`
          }
        ]

      })
    );
  }
  public getDebtPaymentPlansByDebtId(debtId) {
    this.subscription.push(
      this.debtPaymentPlanService.getPaymentPlansByDebtId(debtId)
        .subscribe((rs) => {
          this.debtPlanList = rs.result;
          this.totalDebtPlan = calculateTotalMoney(rs.result);
        }))
  }
  public getEmployeeList() {
    this.subscription.push(
      this.employeeService.getAll().subscribe((rs) => {
        this.employeeList = rs.result;
        this.filteredEmployeeList = rs.result;
      })
    );
  }
  public getDebtPaidsByDebtId(debtId) {
    this.subscription.push(
      this.debtPaidService.getDebtPaidsByDebtId(debtId).subscribe((rs) => {
        this.debtPaidList = rs.result;
        this.totalDebtPaid = calculateTotalMoney(rs.result);
      })
    )
  }
  setInterest(value: number) {
    this.debt.interest = value;
  }

  setFormValue(debt: DebtDto) {
    this.startDate.setValue(debt.startDate);
    this.endDate.setValue(debt.endDate);
    this.principal.setValue(debt.money);
    this.note.setValue(debt.note);
    this.employeeId.setValue(debt.employeeId);
    this.status.setValue(debt.status);
    this.paymentType.setValue(debt.paymentType);
    this.interestRate.setValue(debt.interestRate);
    this.interest.setValue(debt.interest || this.calculateInterest());
    this.status.setValue(debt.debtStatus)
  }

  filterEmployees(event) {
    this.filteredEmployeeList = this.employeeList.filter(
      (employee) =>
        this.l(employee.fullName.toLowerCase()).includes(
          this.l(event.toLowerCase())
        ) || employee.email.includes(event)
    );
  }

  calculateInterest(): number {
    if (
      this.startDate.value &&
      this.endDate.value &&
      this.principal.value &&
      this.interestRate.value
    ) {
      const dayDiff = Math.abs(
        Math.ceil(moment(this.startDate.value).diff(moment(this.endDate.value), 'days'))
      );
      if (dayDiff < 0) return 0;
      const rate = (this.interestRate.value / 365) * dayDiff;
      const interest = (rate * this.principal.value) / 100;
      return Math.round(interest);
    }
    return 0;
  }

  generateDebtPlan() {
    abp.message.confirm(
      `Generate payment plan ${formatCurrency(this.debt.money + this.debt.interest, this.locale, "", "VND", "")
      } VND?`,
      "",
      (result) => {
        if (result) {
          this.isLoading = true;
          this.subscription.push(
            this.debtPaymentPlanService
              .generatePaymentPlan(
                this.formatDateYMD(this.startDate.value),
                this.formatDateYMD(this.endDate.value),
                this.debt.money + this.debt.interest,
                this.debt.id
              )
              .pipe(
                mergeMap(() =>
                  this.debtPaymentPlanService.getPaymentPlansByDebtId(
                    this.debt.id
                  )
                )
              )
              .subscribe((rs) => {
                this.isLoading = false
                this.debtPlanList = rs.result;
                this.totalDebtPlan = calculateTotalMoney(this.debtPlanList)
              })
          );
        }
      }
    );
  }
  checkDates(formGroup: FormGroup) {
    const startDate = formGroup.controls.startDate
    const endDate = formGroup.controls.endDate
    if (startDate && endDate) {
      return validatePaymentDate(startDate, endDate)
    }
    return null
  }
  startDateValidator(control: AbstractControl) {
    if (!control.value || !control.parent || !control.touched) return null;
    if (control.parent.get("endDate").value) {
      return validatePaymentDate(
        control.value,
        control.parent.get("endDate").value
      );
    }
  }
  endDateValidator(control: AbstractControl) {
    if (!control.value || !control.parent || !control.touched) return null;
    if (control.parent.get("startDate").value) {
      return validatePaymentDate(
        control.parent.get("startDate").value,
        control.value
      );
    }
  }

  onSave() {
    this.debt.debtStatus = this.status.value;
    this.debt.money = Number(this.principal.value);
    this.debt.employeeId = this.employeeId.value;
    this.debt.interestRate = this.interestRate.value;
    this.debt.paymentType = this.paymentType.value;
    this.debt.note = this.note.value;
    this.debt.startDate = this.formatDateYMD(this.startDate.value);
    this.debt.endDate = this.formatDateYMD(this.endDate.value);
    this.isLoading = true;
    if (this.debt.id) {
      this.subscription.push(this.debtService.update(this.debt).pipe(finalize(() => this.onCompletedCall())).subscribe((rs) => {
        this.notify.success("Debt updated");
        this.isEdit = false;
        this.getDebtById(this.debt.id)
      }))
    } else {
      this.subscription.push(this.debtService.create(this.debt).pipe(finalize(() => this.onCompletedCall())).subscribe((rs) => {
        this.notify.success("Debt created");
        this.router.navigateByUrl("/app/debt/list-debt/detail/" + rs.result);
        this.isEdit = false;
      }));
    }
  }
  onDone() {
    abp.message.confirm("Change status to done ?", "", (rs) => {
      if(rs) {
        this.subscription.push(
          this.debtService
            .setDone(this.debt.id)
            .pipe(mergeMap(() => this.debtService.get(this.debt.id)))
            .subscribe((rs) => {
              this.debt.debtStatus = EDebtStatus.Done;
              this.notify.success("Debt done");
            })
        );
      }
    })
  }
  onCancel() {
    this.isEdit = false;
    if (this.debt.id) {
      this.setFormValue(this.baseDebt);
    } else {
      this.router.navigate(['app', 'debt', 'list-debt'])
    }
  }
  onDelete() {
    abp.message.confirm(`Delete debt #${this.debt.id}`, "", (result, info) => {
      if (result) {
        this.debtService.delete(this.debt.id).subscribe(() => {
          this.message.success("Debt deleted");
          this.router.navigate(['app', 'debt', 'list-debt'])
        })
      }
    })
  }
  setDebtPlanList(debtPlanList: DebtPlan[]) {
    this.debtPlanList = cloneDeep(debtPlanList)
    this.totalDebtPlan = calculateTotalMoney(this.debtPlanList);
    this.isEdit = false;
  }
  setDebtPaidList() {
    this.subscription.push(
      this.debtPaidService
        .getDebtPaidsByDebtId(this.debt.id)
        .subscribe((rs) => {
          this.debtPaidList = rs.result;
          this.totalDebtPaid = calculateTotalMoney(this.debtPaidList);
          this.isEdit = false;
        })
    );
  }
  isShowEditBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Debt_DebtDetail_Edit);
  }

  isShowDeleteBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Debt_DebtDetail_Delete);
  }

  isShowSetDoneBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Debt_DebtDetail_SetDone);
  }
  isShowGeneratePaymentPlanBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Debt_DebtDetail_GeneratePaymentPlan);
  }

  isAllowEditDebt() {
    return this.debtPaidList.length == 0;
  }

  isDisableGeneratePaymentPlan() {
    return (this.debtPlanList.length>0) || (!this.isEdit && this.debtPlanList.length < 0) || this.isLoading
  }

  onCompletedCall() {
    this.isLoading = false;
  }
}
