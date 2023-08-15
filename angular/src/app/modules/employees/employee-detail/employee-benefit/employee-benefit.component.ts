import { Component, Injector, OnInit, ViewChild } from '@angular/core';
import { BenefitService } from '@app/service/api/benefits/benefit.service';
import { BenefitEmployeeDto, BenefitOfEmployeeDto } from '@app/service/model/benefits/benefitEmployee.dto';
import { PagedListingComponentBase, PagedRequestDto } from '@shared/paged-listing-component-base';
import { AppConsts, BenefitType } from '@shared/AppConsts'
import { benefitDto } from '@app/service/model/benefits/beneft.dto';
import { AddUpdateBenefitemployeeDto } from '@app/service/model/benefits/updateBenefitEmployee.dto'
import { PERMISSIONS_CONSTANT } from '@app/permission/permission';
import { FormControl, NgModel } from '@angular/forms';
import { startWithTap } from '@shared/helpers/observerHelper';
import { finalize } from 'rxjs/operators'
@Component({
  selector: 'app-employee-benefit',
  templateUrl: './employee-benefit.component.html',
  styleUrls: ['./employee-benefit.component.css']
})
export class EmployeeBenefitComponent extends PagedListingComponentBase<BenefitEmployeeDto> implements OnInit {
  @ViewChild('benefitEndDate') benefitEndDate: NgModel
  public employeeId: number
  public listBenefitOfEmployee: BenefitOfEmployeeDto[] = []
  public listBenefitStatus = []
  public listBenefitType = []
  public listBenefit: benefitDto[] = []
  public choosableBenefits = []
  public readonly BenefitTypeNames = Object.entries(BenefitType).reduce((prev, cur) => { prev[cur[1]] = cur[0]; return prev }, {})
  public BenefitMoneys = {}
  public isEdit: boolean = false;
  public DEFAULT_FILTER_VALUE: EmployeeBenefitDefaultFilter = {
    status: true,
    benefitType: AppConsts.DEFAULT_ALL_FILTER_VALUE
  }
  public selectBenefit: FormControl = new FormControl()
  protected list(request: PagedRequestDto, pageNumber: number, finishedCallback: Function): void {
    if(this.isAllowViewTabBenefit()){
      this.isLoading = true;
      this.subscription.push(
        this.benefitService.GetBenefitByEmployeeId(request, this.employeeId).pipe(finalize(() => finishedCallback())).subscribe(rs => {
          this.listBenefitOfEmployee = rs.result.items
          this.showPaging(rs.result, pageNumber)
        })
      )
    }
  }

  constructor(injector: Injector, private benefitService: BenefitService) {
    super(injector)
  }

  ngOnInit(): void {
    this.listBenefitType = this.getListFormEnum(BenefitType)
    this.listBenefitStatus = this.getListFormEnum(this.APP_ENUM.IsActive)
    this.employeeId = this.activatedRoute.snapshot.queryParams["id"]
    this.bindFilterValue();
    this.getAllBenefit()
    this.refresh()
  }

  getAllBenefit() {
    this.subscription.push(
      this.benefitService.getAll().subscribe(rs => {
        this.listBenefit = rs.result
        this.choosableBenefits = rs.result.filter(item => item.isActive == this.APP_ENUM.IsActive.Active).map((item) => ({key: item.name, value: item.id, money: item.money, benefitType: item.type}))
        this.BenefitMoneys = this.convertToConstant(rs.result, 'id', 'money')
      })
    )
  }

  getBenefitMoney(id: number) {
    return this.listBenefit.find(item => item.id == id).money
  }

  addNewBenefitToEmployee() {
    this.listBenefitOfEmployee.unshift(
      {
        isEdit: true,
        benefitId: 0,
        benefitName: "",
        benefitType: 0,
        endDate: "",
        id: 0,
        money: 0,
        startDate: this.listBenefit[0].type == this.APP_ENUM.BenefitType.CheDoChung ? "" : this.formatDateYMD(this.listBenefit[0].applyDate),
        status: true
      })
  }

  onEdit(item) {
    this.isEdit = true;
    item.isEdit = true
  }

  onSave(item: BenefitOfEmployeeDto) {
    if (item.id) {
      const updateBenefitEmployee: AddUpdateBenefitemployeeDto = {
        id: item.id,
        benefitId: item.benefitId,
        employeeId: this.employeeId,
        endDate: item.endDate ? this.formatDateYMD(item.endDate) : null,
        startDate: item.startDate ? this.formatDateYMD(item.startDate) : null
      }
      this.subscription.push(
        this.benefitService.UpdateBenefitEmployee(updateBenefitEmployee)
        .pipe(startWithTap(() => { this.isLoading = true}))
        .pipe(finalize(() => { this.isLoading = false}))
        .subscribe(rs => {
          this.notify.success("Employee's benefit updated")
          item.isEdit = false;
          this.isEdit = false;
          this.refresh()
        })
      )
    } else {
      const addEmployeeToBenefit: AddUpdateBenefitemployeeDto = {
        id: null,
        benefitId: item.benefitId,
        endDate: item.endDate ? this.formatDateYMD(item.endDate) : null,
        employeeId: this.employeeId,
        startDate: item.startDate ? this.formatDateYMD(item.startDate) : null
      }
      this.subscription.push(
        this.benefitService.QuickAddEmployee(addEmployeeToBenefit)
        .pipe(startWithTap(() => { this.isLoading = true}))
        .pipe(finalize(() => { this.isLoading = false}))
        .subscribe((rs) => {
          this.notify.success("Employee added to benefit")
          item.isEdit = false;
          this.isEdit = false;
          this.refresh()
        })
      )
    }
  }

  onCancel() {
    this.refresh()
    this.selectBenefit.setValue(null)
    this.isEdit = false;
  }

  onDelete(benefit: BenefitOfEmployeeDto) {
    this.confirmDelete(`Remove this employee from <b>${benefit.benefitName}</b> benefit?`, () => {
      this.benefitService.RemoveEmployeeFromBenefit(benefit.id).subscribe((rs) => {
        this.notify.success("Employee removed from benefit")
        this.refresh()
      })
    })
  }

  bindFilterValue() {
    if (!this.router.url.includes('filterItems')) {
      this.filterItems = [
        {
          propertyName: "status",
          value: this.DEFAULT_FILTER_VALUE.status,
          comparision: this.APP_ENUM.filterComparison.EQUAL
        }
      ]
      return;
    }
    if (this.filterItems.length) {
      this.filterItems.forEach((item) => {
        this.DEFAULT_FILTER_VALUE[item.propertyName] = item.value
      })
      if (!this.filterItems.find(item => item.propertyName.toLocaleLowerCase() == 'status')) {
        this.DEFAULT_FILTER_VALUE.status = this.APP_CONST.DEFAULT_ALL_FILTER_VALUE
      }
      return;
    }
    this.DEFAULT_FILTER_VALUE = {
      status: this.APP_CONST.DEFAULT_ALL_FILTER_VALUE,
      benefitType: this.APP_CONST.DEFAULT_ALL_FILTER_VALUE
    }
  }

  onBenefitChange(benefitId: number, benefit: BenefitOfEmployeeDto) {
    const newBenefit = this.listBenefit.find(b => b.id == benefitId)
    if (newBenefit) {
      benefit.benefitId = benefitId
      benefit.benefitType = newBenefit.type
      benefit.startDate = newBenefit.type == this.APP_ENUM.BenefitType.CheDoChung ? "" : this.formatDateYMD(newBenefit.applyDate)
      benefit.money = newBenefit.money;
    }
  }

  isShowAddBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Employee_EmployeeDetail_TabBenefit_Add);
  }

  isShowEditBtn(benefit: BenefitOfEmployeeDto){
    return this.isGranted(PERMISSIONS_CONSTANT.Employee_EmployeeDetail_TabBenefit_Edit) && benefit.benefitType != this.APP_ENUM.BenefitType.CheDoChung;
  }

  isShowDeleteBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Employee_EmployeeDetail_TabBenefit_Delete);
  }

  isAllowViewTabBenefit(){
    return this.isGranted(PERMISSIONS_CONSTANT.Employee_EmployeeDetail_TabBenefit_View);
  }

  onCompletedCall() {
    this.isLoading = false;
  }

  testDate(endDate: NgModel){
    if(endDate) {
      if(endDate.value == null && endDate.valid) return false;
      if(endDate.control.invalid) return true
    }
    return false;
  }
}


export interface EmployeeBenefitDefaultFilter {
  status: boolean | number,
  benefitType: number
}
