import { Component, Injector, OnInit } from '@angular/core';
import { BranchService } from '@app/service/api/categories/branch.service';
import { LevelService } from '@app/service/api/categories/level.service';
import { TeamService } from '@app/service/api/categories/team.service';
import { UserTypeService } from '@app/service/api/categories/userType.service';
import { DebtDto, DebtInputFilterDto, EDebtStatus } from '@app/service/model/debt/debt.dto'
import { PagedListingComponentBase, PagedRequestDto } from '@shared/paged-listing-component-base';
import { forkJoin } from 'rxjs';
import { finalize } from 'rxjs/operators';
import { DEBT_STATUS } from '../models/debtStatus';
import { EPaymentType, PAYMENT_METHOD } from '../models/paymentType';
import { DebtService } from '@app/service/api/debt/debt.service'
import * as moment from 'moment';
import { LayoutStoreService } from '@shared/layout/layout-store.service';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { GetInputFilterDto } from '@app/service/model/employee/GetEmployeeExcept.dto';
import { APP_ENUMS } from '@shared/AppEnums';
import { PERMISSIONS_CONSTANT } from '@app/permission/permission';
import { MailDialogComponent } from '@app/modules/admin/email-templates/mail-dialog/mail-dialog.component';
import { SendDebtMailToOneEmployeeDto } from '@app/service/model/mail/sendMail.dto';
@Component({
  selector: 'app-debt-list',
  templateUrl: './debt-list.component.html',
  styleUrls: ['./debt-list.component.css'],
  animations: [appModuleAnimation()]
})
export class DebtListComponent extends PagedListingComponentBase<DebtDto> implements OnInit {
  public debtList: DebtDto[] = [];
  public branchList: BaseFilter[] = [];
  public teamList: BaseFilter[] = [];
  public levelList: BaseFilter[] = [];
  public userTypeList: BaseFilter[] = [];
  public workingStatusList = [];
  public inputToFilter = {} as DebtInputFilterDto;
  public readonly PAYMENT_METHOD = PAYMENT_METHOD;
  public readonly DEBT_STATUS = DEBT_STATUS
  public readonly DEFAULT_FILTER_VALUE = this.APP_CONST.DEFAULT_ALL_FILTER_VALUE
  public readonly EPaymentType = EPaymentType;
  public readonly EDebtStatus = EDebtStatus;
  public filterMultipleTypeParamEnum = APP_ENUMS.FilterMultipleTypeParamEnum;
  public DEFAULT_FILTER = {
    EndDate: null,
    status: this.APP_CONST.DEFAULT_ALL_FILTER_VALUE,
    debtStatus: EDebtStatus.Inprogress,
    userTypeId: this.APP_CONST.DEFAULT_ALL_FILTER_VALUE,
    paymentType: this.APP_CONST.DEFAULT_ALL_FILTER_VALUE,
    teamId: this.APP_CONST.DEFAULT_ALL_FILTER_VALUE,
    branchId: this.APP_CONST.DEFAULT_ALL_FILTER_VALUE,
    levelId: this.APP_CONST.DEFAULT_ALL_FILTER_VALUE,
    sex: this.APP_CONST.DEFAULT_ALL_FILTER_VALUE
  }
  constructor(injector: Injector,
    private userTypeService: UserTypeService,
    private branchService: BranchService,
    private teamService: TeamService,
    private levelService: LevelService,
    private debtService: DebtService,
    private layoutStoreService: LayoutStoreService
  ) {
    super(injector);
  }
  protected list(request: PagedRequestDto, pageNumber: number, finishedCallback: Function): void {
    let input = {
      statusIds: this.statusIds,
      levelIds: this.levelIds,
      branchIds: this.branchIds,
      userTypes: this.userTypes,
      teamIds: this.teamIds,
      isAndCondition : this.isAndCondition,
      gridParam: request,
      jobPositionIds: this.jobPositionIds,
      debtstatusIds: this.debtstatusIds,
      paymentTypeIds: this.paymentTypeIds
    } as DebtInputFilterDto;
    this.inputToFilter = input;

    this.subscription.push(
      this.debtService.getAllDebtPagging(input)
        .pipe(finalize(() => {
          finishedCallback()
        })).subscribe(rs => {
          this.debtList = rs.result.items;
          this.debtList.map((debt) => {
            debt.interest = this.calculateInterest(debt.startDate, debt.endDate, debt.money, debt.interestRate)
            return debt;
          })
          this.showPaging(rs.result, pageNumber)
        })
    )
  }
  calculateInterest(startDate, endDate, money, interestRate): number {
    const dayDiff = Math.ceil(Math.abs(moment(startDate).diff(moment(endDate), 'day')))
    if (dayDiff < 0) return 0;
    const ratePerDay = (interestRate / 365) * dayDiff;
    const interest = (ratePerDay * money) / 100;
    return Math.ceil(interest)
  }
  public columnList = [
    {
      name: "Stt",
      displayName: "#",
      isShow: true,
      className: 'col-stt',
      sortable: false
    },
    {
      name: "fullName",
      displayName: "Employee",
      isShow: true,
      className: 'col-employee',
      sortable: true
    },
    {
      name: "money",
      displayName: "Principal",
      isShow: true,
      className: 'col-principal',
      sortable: true
    },
    {
      name: "interestRate",
      displayName: "APR(%)",
      isShow: true,
      className: 'col-interestRate',
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
      name: "paymentType",
      displayName: "Method",
      isShow: true,
      className: 'col-paymentType',
      sortable: true
    },
    {
      name: "startDate",
      displayName: "Start Date",
      isShow: true,
      className: 'col-startDate',
      sortable: true
    },
    {
      name: "endDate",
      displayName: "End Date",
      isShow: true,
      className: 'col-endDate',
      sortable: true
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
    },
  ]
  public paymentTypeFilter = [
    {
      key: PAYMENT_METHOD[EPaymentType.TruLuong].key,
      value: EPaymentType.TruLuong,
    },
    {
      key: PAYMENT_METHOD[EPaymentType.TienMat].key,
      value: EPaymentType.TienMat,
    }
  ]
  public statusFilter = [
    {
      key: DEBT_STATUS[EDebtStatus.Inprogress].key,
      value: EDebtStatus.Inprogress,
    },
    {
      key: DEBT_STATUS[EDebtStatus.Done].key,
      value: EDebtStatus.Done,
    },
  ]
  public genderFilter = [
    {
      key: 'All',
      value: this.APP_CONST.DEFAULT_ALL_FILTER_VALUE
    },
    {
      key: 'Male',
      value: this.APP_ENUM.Gender.Male,
    },
    {
      key: 'Female',
      value: this.APP_ENUM.Gender.Female
    },
  ]
  
  ngOnInit(): void {
    this.listBreadCrumb = [
      {name: '<i class="fa-solid fa-house fa-sm"></i>',url:''}, 
      {name: ' <i class="fa-solid fa-chevron-right"></i> '}, 
      {name:'Debt'}];
    this.getFilterValue()
    if (this.filterItems.length) {
      this.bindFilterValue()
    }
    this.refresh();
    this.workingStatusList = this.getListFormEnum(APP_ENUMS.UserStatus, true);
    this.debtstatusIds = [EDebtStatus.Inprogress];
  }
  
  ngOnDestroy(): void {
    this.layoutStoreService.setSidebarExpanded(false);
  }

  getFilterValue() {
    // const baseRequest: PagedRequestDto = { maxResultCount: 200000 } as PagedRequestDto;
    const defaultFilterObj = { key: 'All', value: this.APP_CONST.DEFAULT_ALL_FILTER_VALUE }
    this.subscription.push(
      forkJoin([
        this.branchService.getAll(),
        this.teamService.getAll(),
        this.levelService.getAll(),
        this.userTypeService.getAll()
      ]).subscribe((data) => {
        this.branchList = data[0].result.map((branch) => ({ key: branch.name, value: branch.id }));
        this.teamList = data[1].result.map((team) => ({ key: team.name, value: team.id }));
        this.levelList = data[2].result.map((level) => ({ key: level.name, value: level.id }));
        this.userTypeList = data[3].result.map((userType) => ({ key: userType.name, value: userType.id }))
      })
    )
  }

  bindFilterValue() {
    this.filterItems.forEach(filterItem => {
      this.DEFAULT_FILTER[filterItem.propertyName] = filterItem.value;
      if (filterItem.propertyName == 'EndDate') {
        this.DEFAULT_FILTER['EndDate'] = moment(filterItem.value).toISOString()
      }
    })
  }

  onDelete(debtId) {
    this.confirmDelete("Delete debt?", () => {
      this.debtService.delete(debtId).subscribe(rs => {
        this.notify.success("Debt deleted");
        this.refresh()
      })
    })
  }

  getPaymentType(value) {
    return this.paymentTypeFilter.find(type => type.value == value).key
  }

  isShowCreateBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Debt_Create);
  }

  isShowDeleteBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Debt_Delete);
  }

  isAllowRoutingDetail(){
    return this.isGranted(PERMISSIONS_CONSTANT.Debt_DebtDetail);
  }
  isAllowViewTabPersonalInfo(){
    return this.isGranted(PERMISSIONS_CONSTANT.Employee_EmployeeDetail_TabPersonalInfo_View);
  }
  isShowSendAllMailBtn(){
    return true;
  }
  isShowSendMailBtn(){
    return true;
  }
  public onSendAllMail(){
    this.inputToFilter.gridParam.maxResultCount = 500000;
    this.isLoading = true;
    this.subscription.push(
      this.debtService.sendAllMail(this.inputToFilter).subscribe((rs)=>{
        abp.message.success(rs.result);
        this.isLoading = false;
      },()=> this.isLoading = false)
    )
  }

  public onSendMail(id: number){
    this.subscription.push(
      this.debtService.getDebtTemplate(id).subscribe((rs)=>{
        const dialogData = {
          showEditButton: true,
          mailInfo: rs.result,
          showSendMailButton: true,
          showSendMailHeader: true,
          showDialogHeader: false,
        }
        const ref = this.dialog.open(MailDialogComponent,
          {
           data: dialogData,
           width: '1600px',
           panelClass: 'email-dialog',
          })
         ref.afterClosed().subscribe((rs)=>{
           if(rs){
             var input: SendDebtMailToOneEmployeeDto = {
               debtId: id,
               mailContent: rs
             }
             this.subscription.push(
               this.debtService.sendMail(input).subscribe((rs)=>{
                abp.message.success(`Mail sent to ${dialogData.mailInfo.sendToEmail}!`)
               })
             )
           }
         })
      })
    )
  }
}

export interface BaseFilter {
  key: string,
  value: any
}
