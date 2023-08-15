import { MatDialog } from '@angular/material/dialog';
import { CalculateResultComponent } from './../payslip-detail/calculate-result/calculate-result.component';
import { SendMailOneemployeeDto } from '@app/service/model/mail/sendMail.dto';
import { PaySlipDto, SummaryInfomationDto } from '../../../service/model/payslip/payslip.dto';
import { PayslipService } from '../../../service/api/payslip/payslip.service';
import { PagedListingComponentBase, PagedRequestDto } from '@shared/paged-listing-component-base';
import { Component, OnInit, Injector, ViewChild } from '@angular/core';
import { UserTypeDto } from '@app/service/model/categories/userType.dto';
import { LevelDto } from '@app/service/model/categories/level.dto';
import { BranchDto } from '@app/service/model/categories/branch.dto';
import { JobPositionDto } from '@app/service/model/categories/jobPosition.dto';
import { TeamDto } from '@app/service/model/categories/team.dto';
import { APP_ENUMS } from '@shared/AppEnums';
import { BranchService } from '@app/service/api/categories/branch.service';
import { LevelService } from '@app/service/api/categories/level.service';
import { TeamService } from '@app/service/api/categories/team.service';
import { UserTypeService } from '@app/service/api/categories/userType.service';
import { JobPositionService } from '@app/service/api/categories/jobPosition.service';
import { PayRollService } from '@app/service/api/pay-roll/pay-roll.service';
import { MatMenuTrigger } from '@angular/material/menu';
import { ActivatedRoute } from '@angular/router';
import { PayRollDto } from '@app/service/model/salaries/salaries.dto';
import * as moment from 'moment';
import { EmployeeService } from '@app/service/api/employee/employee.service';
import { GetEmployeeDto } from '@app/service/model/employee/employee.dto';
import { AddEmployeeComponent } from '@shared/components/employee/add-employee/add-employee.component';
import { FormControl, FormGroup } from '@angular/forms';
import { finalize, switchMap } from 'rxjs/operators';
import { AddEmployeeToPayroll } from '@app/service/model/payslip/AddEmployeeToPayroll';
import { CollectPayslipDto } from '@app/service/model/payslip/CollectPayslipDto';
import { MailDialogComponent, MailDialogData } from '@app/modules/admin/email-templates/mail-dialog/mail-dialog.component';
import { PERMISSIONS_CONSTANT } from '@app/permission/permission';
import { GetInputFilterDto } from '@app/service/model/employee/GetEmployeeExcept.dto';
import { SignalRAspNetCoreHelper } from '@shared/helpers/SignalRAspNetCoreHelper';
import { CalculateResultDialogComponent } from './calculate-result-dialog/calculate-result-dialog.component';
import { ConfirmMailDialogComponent } from './confirm-mail-dialog/confirm-mail-dialog.component';
import * as FileSaver from 'file-saver';
import { ImportEmployeeRemainLeaveDaysAfterCalculatingSalaryComponent } from './import-employee-remain-leave-days-after-calculating-salary/import-employee-remain-leave-days-after-calculating-salary.component';
import { of } from 'rxjs';
import { PenaltyUserDialogComponent } from './penalty-user-dialog/penalty-user-dialog.component';

@Component({
  selector: 'app-payslip',
  templateUrl: './payslip.component.html',
  styleUrls: ['./payslip.component.css']
})
export class PayslipComponent extends PagedListingComponentBase<any> implements OnInit {
  protected list(request: PagedRequestDto, pageNumber: number, finishedCallback: Function): void {
    let input = {
      statusIds: this.statusIds,
      levelIds: this.levelIds,
      branchIds: this.branchIds,
      userTypes: this.userTypes,
      teamIds: this.teamIds,
      isAndCondition: this.isAndCondition,
      gridParam: request,
      jobPositionIds: this.jobPositionIds,
    } as GetInputFilterDto;

        this.subscription.push(this.payslipService.getPayslipEmployeePaging(this.payrollId, input)
        .pipe(finalize(() => finishedCallback())).subscribe((data) => {
          this.listPayslips = data.result.items;
          this.inputExportPayroll = input
          this.showPaging(data.result, pageNumber);
        }, () => this.isLoading = false))
  }

  constructor(injector: Injector,
    private payrollService: PayRollService,
    private payslipService: PayslipService,
    private employeeService: EmployeeService,
    private branchService: BranchService,
    private levelService: LevelService,
    private teamService: TeamService,
    private userTyService: UserTypeService,
    private positionService: JobPositionService,
    public route: ActivatedRoute,
    public dialog : MatDialog
  ) {
    super(injector);
  }
  @ViewChild(MatMenuTrigger)
  public menu: MatMenuTrigger;
  public contextMenuPosition = { x: '0px', y: '0px' };
  public payrollId: number = 0;
  public payrollStatus: number = 0;
  public payroll = {} as PayRollDto;
  public listPayslips: PaySlipDto[] = [];
  public employeeList: GetEmployeeDto[] = [];
  public userTypeList: UserTypeDto[] = [];
  public userLevelList: LevelDto[] = [];
  public branchList: BranchDto[] = [];
  public positionList: JobPositionDto[] = [];
  public teamList: TeamDto[] = [];
  public statusList: Object[] = [];
  public genderList: Object[] = [];
  public searchEmployee: string = "";
  public summaryInfomations: SummaryInfomationDto[] = [];
  public sending: boolean = false;
  public filterMultipleTypeParamEnum = APP_ENUMS.FilterMultipleTypeParamEnum;
  public calculateProcess: string = ""
  public calculateStatus: string = ""
  public isCalculating: boolean = false
  private inputExportPayroll: GetInputFilterDto
  private calculateResultRef
  public confirmMailFilters = [
    {
      key: "All",
      value: -1
    },
    {
      key: "Not Confirmed",
      value: 0
    },
    {
      key: "Confirm right",
      value: 1
    },
    {
      key: "Confirm wrong",
      value: 2
    }
  ]

  public DEFAULT_FILTER = {
    branch: this.APP_CONST.DEFAULT_ALL_FILTER_VALUE,
    team: this.APP_CONST.DEFAULT_ALL_FILTER_VALUE,
    userLevel: this.APP_CONST.DEFAULT_ALL_FILTER_VALUE,
    userType: this.APP_CONST.DEFAULT_ALL_FILTER_VALUE,
    status: this.APP_CONST.DEFAULT_ALL_FILTER_VALUE,
    jobPosition: this.APP_CONST.DEFAULT_ALL_FILTER_VALUE,
    gender: this.APP_CONST.DEFAULT_ALL_FILTER_VALUE,
    confirmMail: -1
  };
  public formGroup = new FormGroup({
    teams: new FormControl('teams'),
  });

  ngOnInit(): void {
    this.pageSizeType = 5;
    this.pageSize = 5;
    this.payrollId = Number(this.route.snapshot.queryParamMap.get('id'));
    this.genderList = this.getListFormEnum(APP_ENUMS.Gender);
    this.statusList = this.getListFormEnum(APP_ENUMS.UserStatus, true);
    this.payrollStatus = Number(this.route.snapshot.queryParamMap.get('status'));
    this.getPayrollById();
    this.getAllEmployee();
    this.getAllUserType();
    this.getAllLevel();
    this.getAllBranch();
    this.getAllTeam();
    this.getAllJobPositon();
    this.refresh();
    this.getSummaryInfo();

    this.subscription.push(
    this.APP_CONST.calSalaryProcess.asObservable().subscribe(rs => {
      let lastNoti = rs
      this.calculateProcess = lastNoti?.process ?? ""
      this.calculateStatus = lastNoti?.status ?? ""
      if (lastNoti?.status == 'Start') {
        this.isCalculating = true
      }
      if(lastNoti?.status == 'Error' && lastNoti?.message.length>0){
        if(this.calculateResultRef){
          this.calculateResultRef.close()
        }
        this.dialog.open(CalculateResultComponent, {
          width: "700px",
          data: lastNoti?.message
        })
        this.isCalculating = false
        this.APP_CONST.calSalaryProcess.next({})
      }

      if (lastNoti?.status == 'Done') {
        this.refresh()
        this.getSummaryInfo()
        this.isCalculating = false
      }
    })
    )
  }

  
  public OnGetNotPaidInfo()
  {
    this.dialog.open(PenaltyUserDialogComponent,{
      width:"auto",
      height:"auto",
      data:{
        id:this.payrollId,      
      }      
    })
  }

  public getSummaryInfo() {
    this.subscription.push(
      this.payslipService.GetSumaryInfomation(this.payrollId).subscribe((rs) => {
        this.summaryInfomations = rs.result;
      })
    )
  }


  public async onAdd() {
    const listEmpoyeeIds = await this.payslipService.GetEmployeeIdsInPayroll(this.payrollId).toPromise();

    let ref = this.dialog.open(AddEmployeeComponent, {
      width: "92vw",
      height: "97vh",
      maxWidth: "100vw",
      data: {
        title: `Add employee to payroll: <strong>${moment(this.payroll.applyMonth).format('MM/YYYY')}</strong>`,
        addedEmployeeIds: listEmpoyeeIds.result
      }
    })
    ref.afterClosed().subscribe((listEmployeeId: number[]) => {
      if (listEmployeeId.length) {
        let input = {
          payrollId: this.payrollId,
          employeeIds: listEmployeeId
        } as AddEmployeeToPayroll
        this.subscription.push(
          this.payslipService.AddEmployeeToPayroll(input).subscribe((rs) => {
            if(rs.result.errorList != null && rs.result.errorList.length > 0){
              this.dialog.open(CalculateResultComponent,
                {
                  width: "700px",
                  data: rs.result.errorList
                })
                return
            }
            abp.message.success("Add employee succesful");
            this.refresh()
          },
            () => this.isCalculating = false)
        )
      }
    })
  }
  public calculateAllSalary() {
    this.isLoading = true;
    let payload: CollectPayslipDto = {
      payrollId: this.payrollId,
      employeeIds: null
    }


    this.subscription.push(
      this.payslipService.CalculateSalaryForAllPayslip(payload).subscribe(rs => {
        this.isLoading = false;
        this.refresh();
      })
    )
  }
  public getTeams(listTeams) {
    return listTeams.map((team) => {
      return team.teamName
    }).join(", ").trimEnd()
  }

  public getPayrollById() {
    this.subscription.push(
      this.payrollService.get(this.payrollId).subscribe((data) => {
        this.payroll = data.result;
        this.payrollStatus = data.result.status;
        if (this.payroll) {
          this.listBreadCrumb = [
            { name: '<i class="fa-solid fa-house fa-sm"></i>', url: '' },
            { name: ' <i class="fa-solid fa-chevron-right"></i> ' },
            { name: ' Payroll ', url: '/app/payroll/list-payroll' },
            { name: ' <i class="fa-solid fa-chevron-right"></i> ' },
            { name: `Payroll -  ${moment(this.payroll.applyMonth).format('MM/YYYY')}`, url: '' }];
        }

      })
    )
  }

  public getAllEmployee() {
    this.subscription.push(
      this.employeeService.getAll().subscribe((data) => {
        this.employeeList = data.result;
      })
    )
  }

  private getAllUserType() {
    this.subscription.push(this.userTyService.getAll().subscribe(rs => {
      this.userTypeList = this.mapToFilter(rs.result, true)
    }))
  }

  private getAllLevel() {
    this.subscription.push(this.levelService.getAll().subscribe(rs => {
      this.userLevelList = this.mapToFilter(rs.result, true)
    }))
  }

  private getAllBranch() {
    this.subscription.push(this.branchService.getAll().subscribe(rs => {
      this.branchList = this.mapToFilter(rs.result, true)
    }))
  }

  private getAllTeam() {
    this.subscription.push(this.teamService.getAll().subscribe(rs => {
      this.teamList = this.mapToFilter(rs.result, true)
    }))
  }

  private getAllJobPositon() {
    this.subscription.push(this.positionService.getAll().subscribe(rs => {
      this.positionList = this.mapToFilter(rs.result, true)
    }))
  }

  public onChangeStatus(status: number) {
    let input = {
      payrollId: this.payrollId,
      status: status
    }
    this.subscription.push(
      this.payrollService.changeStatus(input).subscribe((rs) => {
        abp.message.success(`Change status successful`+ `<p style="margin-top: 10px;word-wrap: normal;font-weight: 400;font-size: 20px;color: #dc3545;">${rs.result}</p>`);
        this.refresh();
        this.getPayrollById();
      })
    )
  }

  public onCollectData() {
    abp.message.confirm("Are you want to calculate salary?", "", (result) => {
      if (result) {
        const payload = {
          payrollId: this.payrollId
        } as CollectPayslipDto

        this.APP_CONST.calSalaryProcess.next({})

        this.viewCalculateProcess();

        this.subscription.push(this.payslipService.collectPayslip(payload).subscribe(rs => {
        }))
      }
    })
  }

  public viewCalculateProcess() {
    this.calculateResultRef = this.dialog.open(CalculateResultDialogComponent, {
      width: "60vw",
      height: "82vh",
      data: {
        payrollApplyMonth: this.payroll.applyMonth
      }
    })
  }

  public onCollectEmployeeData(employeeId: number) {
    this.isLoading = true;
    const payload = {
      payrollId: this.payrollId,
      employeeIds: [employeeId]
    } as CollectPayslipDto
    this.subscription.push(this.payslipService.collectPayslip(payload).subscribe(rs => {
      this.isLoading = false;
      abp.message.success(`Employee data collected`)
      this.refresh()
    }))
  }

  public onCalculateEmployeeSalary(payslip: PaySlipDto) {
    this.isLoading = true;
    const payload: CollectPayslipDto = {
      payrollId: this.payroll.id,
      employeeIds: [payslip.id]
    }
    this.subscription.push(this.payslipService.CalculateSalaryForOnePayslip(payload).subscribe(rs => {
      this.isLoading = false;
      abp.message.success(`Employee ${payslip.fullName} salary calculated`)
      this.refresh()
    }))
  }
  onDeletePayslip(payslip: PaySlipDto) {
    this.confirmDelete(`Delete payslip of <strong>${payslip.fullName}</strong>`, () => {
      this.subscription.push(
        this.payslipService.delete(payslip.id).subscribe(rs => {
          abp.notify.success(`Deleted payslip of ${payslip.fullName} (id ${payslip.id}) successfull`)
          this.refresh()
        })
      )
    })
  }

  onSendMail(payslip: PaySlipDto) {
    this.dialog.open(ConfirmMailDialogComponent,
      {
        width: "600px",
        data: {
          payslipId: payslip.id,
          email: payslip.email
        }
      })
  }

  onSendMailAll() {
    this.dialog.open(ConfirmMailDialogComponent, {
      width: "600px",
      disableClose: true,
      data: {
        payrollId: this.payrollId
      }
    })
  }

  executePayroll() {
    const dateString = moment(this.payroll.applyMonth).format("MM/YYYY").toString()
    abp.message.confirm(`Execute payroll <strong>${dateString}</strong>`, "", (rs) => {
      if (rs) {
        this.subscription.push(
          this.payrollService.ExecuatePayroll(this.payrollId).subscribe(rs => {
            abp.message.success("Payroll executed" + `<p style="margin-top: 10px; word-wrap: normal;font-weight: 400;font-size: 20px;color: #dc3545;">${rs.result}</p>`);
            this.getPayrollById()
          })
        )
      }
    }, { isHTML: true })
  }

  public async createFinfastOutcomeEntry() {
    abp.message.confirm(`Create finfast outcomingEntry for payroll: <strong>${this.formatDateMY(this.payroll.applyMonth)}</strong>`, "", (rs) => {
      if (rs) {
        let api1 = this.payrollService.ValidFinfastBranch(this.payrollId)
        let api2 = this.payrollService.CreateFinfastOutcomeEntry(this.payrollId)

        this.subscription.push(
        api1.pipe(
          switchMap(rs => {
            if (rs.result.failList.length > 0) {
              let message = `${rs.result.failList.join("<br/>")}`
              abp.message.error(message, "Send to finfast error", {isHTML:true})
              return of(null);
            } else {
              abp.notify.success("Created finfast outcomeEntry")
              return api2;
            }
          })
        ).subscribe(rs2 => {
        }))
      }
    }, { isHTML: true }
    )
  }

  public exportTechcombank(){
    this.subscription.push(
      this.payslipService.exportTechcombank(this.payrollId).subscribe(rs=>{
        const file = new Blob([this.convertFile(atob(rs.result.base64))], {
          type: "application/vnd.ms-excel;charset=utf-8"
        });
        FileSaver.saveAs(file, `${rs.result.fileName}.xlsx`)
      })
    )
  }

  public exportPayroll(){
    this.subscription.push(
      this.payslipService.exportPayroll(this.payrollId, this.inputExportPayroll).subscribe(rs=>{
        const file = new Blob([this.convertFile(atob(rs.result.base64))], {
          type: "application/vnd.ms-excel;charset=utf-8"
        });
        FileSaver.saveAs(file, `${rs.result.fileName}.xlsx`)
      })
    )
  }

  public exportPayrollIncludeLastMonth(){
    this.subscription.push(
      this.payslipService.ExportPayrollIncludeLastMonth(this.payrollId).subscribe(rs=>{
        const file = new Blob([this.convertFile(atob(rs.result.base64))], {
          type: "application/vnd.ms-excel;charset=utf-8"
        });
        FileSaver.saveAs(file, `${rs.result.fileName}.xlsx`)
      })
    )
  }

  public exportOutsideTech(){
    this.subscription.push(
      this.payslipService.exportOutsideTech(this.payrollId).subscribe(rs=>{
        const file = new Blob([this.convertFile(atob(rs.result.base64))], {
          type: "application/vnd.ms-excel;charset=utf-8"
        });
        FileSaver.saveAs(file, `${rs.result.fileName}.xlsx`)
      })
    )
  }

  public onImportEmployeeRemainLeaveDays(){
    let dl = this.dialog.open(ImportEmployeeRemainLeaveDaysAfterCalculatingSalaryComponent,{
      data: this.payrollId
    });
    dl.afterClosed().subscribe((rs)=>{
     if(rs){
      this.refresh();
     }
    })

  }

  public expanColumn(column) {
    column.expanded = true
    column.width = 500
  }

  public collapseColumn(column) {
    column.expanded = false
    column.width = 200
  }



  isShowCalculateSalaryBtn() {
    return this.isGranted(PERMISSIONS_CONSTANT.Payroll_Payslip_CalculateSalary) && (this.payrollStatus == APP_ENUMS.PayrollStatus.New
      || this.payrollStatus == APP_ENUMS.PayrollStatus.RejectedByKT || this.payrollStatus == APP_ENUMS.PayrollStatus.RejectedByCEO);
  }
  isShowSendMailAllBtn() {
    return this.isGranted(PERMISSIONS_CONSTANT.Payroll_Payslip_SendMailAll) && this.payrollStatus != APP_ENUMS.PayrollStatus.Executed;
  }
  isShowAddBtn() {
    return this.isGranted(PERMISSIONS_CONSTANT.Payroll_Payslip_Add) && (this.payrollStatus == APP_ENUMS.PayrollStatus.New
      || this.payrollStatus == APP_ENUMS.PayrollStatus.RejectedByKT || this.payrollStatus == APP_ENUMS.PayrollStatus.RejectedByCEO);
  }
  isShowExportBtn() {
    return this.isGranted(PERMISSIONS_CONSTANT.Payroll_Payslip_Export);
  }
  isShowDeleteBtn() {
    return this.isGranted(PERMISSIONS_CONSTANT.Payroll_Payslip_Delete) && (this.payrollStatus == APP_ENUMS.PayrollStatus.New
      || this.payrollStatus == APP_ENUMS.PayrollStatus.RejectedByKT || this.payrollStatus == APP_ENUMS.PayrollStatus.RejectedByCEO);
  }
  isShowSendMailBtn() {
    return this.isGranted(PERMISSIONS_CONSTANT.Payroll_Payslip_SendMail) && this.payrollStatus != APP_ENUMS.PayrollStatus.Executed;
  }
  isShowSendToFinfastBtn() {
    return this.payrollStatus != APP_ENUMS.PayrollStatus.ApprovedByCEO ||
    this.payrollStatus != APP_ENUMS.PayrollStatus.Executed
  }
  isShowSendToAccountantBtn() {
    return this.isGranted(PERMISSIONS_CONSTANT.Payroll_Payslip_SendToAccountant) && (this.payrollStatus == APP_ENUMS.PayrollStatus.New || this.payrollStatus == APP_ENUMS.PayrollStatus.RejectedByKT);
  }
  isShowApproveAndSendtToCEOBtn() {
    return this.isGranted(PERMISSIONS_CONSTANT.Payroll_Payslip_ApproveAndSendtToCEO) && (this.payrollStatus == APP_ENUMS.PayrollStatus.PendingKT || this.payrollStatus == APP_ENUMS.PayrollStatus.RejectedByCEO);
  }
  isShowRejectByKTBtn() {
    return this.isGranted(PERMISSIONS_CONSTANT.Payroll_Payslip_RejectByKT) && (this.payrollStatus == APP_ENUMS.PayrollStatus.PendingKT || this.payrollStatus == APP_ENUMS.PayrollStatus.RejectedByCEO);
  }
  isShowApproveByKTBtn() {
    return this.isGranted(PERMISSIONS_CONSTANT.Payroll_Payslip_ApproveByKT) && this.payrollStatus == APP_ENUMS.PayrollStatus.RejectedByKT;
  }
  isShowRejectByCEOBtn() {
    return this.isGranted(PERMISSIONS_CONSTANT.Payroll_Payslip_RejectByCEO) && (this.payrollStatus == APP_ENUMS.PayrollStatus.PendingCEO ||  this.payrollStatus == APP_ENUMS.PayrollStatus.ApprovedByCEO);
  }
  isShowApproveByCEOBtn() {
    return this.isGranted(PERMISSIONS_CONSTANT.Payroll_Payslip_ApproveByCEO) && (this.payrollStatus == APP_ENUMS.PayrollStatus.PendingCEO || this.payrollStatus == APP_ENUMS.PayrollStatus.RejectedByCEO);
  }
  isShowExecuteBtn() {
    return this.isGranted(PERMISSIONS_CONSTANT.Payroll_Payslip_Execute) && this.payrollStatus == APP_ENUMS.PayrollStatus.ApprovedByCEO;
  }
  isAllowRoutingDetail() {
    return this.isGranted(PERMISSIONS_CONSTANT.Payroll_Payslip_PayslipDetail);
  }
  isAllowViewTabPersonalInfo() {
    return this.isGranted(PERMISSIONS_CONSTANT.Employee_EmployeeDetail_TabPersonalInfo_View);
  }
  isShowActions() {
    return this.payrollStatus != APP_ENUMS.PayrollStatus.Executed
  }
  isShowDownloadTemplateBtn(){
    return this.permission.isGranted(PERMISSIONS_CONSTANT.Payroll_Payslip_DownloadTemplateUpdateRemainLeaveDaysAfter);
  }
  isShowUpdateRemainLeaveDaysAndDownloadTemplateBtn(){
    return this.payrollStatus != APP_ENUMS.PayrollStatus.Executed && (this.isShowDownloadTemplateBtn() || this.isShowUpdateRemainLeaveDaysBtn());
  }
  isShowUpdateRemainLeaveDaysBtn(){
    return this.permission.isGranted(PERMISSIONS_CONSTANT.Payroll_Payslip_UpdateRemainLeaveDaysAfter);
  }
  isShowExportPayrollIncludeLastMonth(){
    return this.permission.isGranted(PERMISSIONS_CONSTANT.Payroll_Payslip_ExportPayrollIncludeLastMonth);
  }
  isShowExportPayrollBtn(){
    return this.permission.isGranted(PERMISSIONS_CONSTANT.Payroll_Payslip_ExportPayroll);
  }
  isShowExportOutsideTech(){
    return this.permission.isGranted(PERMISSIONS_CONSTANT.Payroll_Payslip_ExportOutsideTech);
  }
  isShowExportTechcombank(){
    return this.permission.isGranted(PERMISSIONS_CONSTANT.Payroll_Payslip_ExportTechcombank);
  }

  public columnList = [
    {
      name: "no",
      displayName: "#",
      isShow: true,
      sortable: false,
      className: "",
      width: 50
    },
    {
      name: "email",
      displayName: "Employee",
      isShow: true,
      sortable: true,
      className: "",
      width: 400
    },
    {
      name: "NormalSalary",
      displayName: "Normal salary",
      isShow: false,
      sortable: true,
      className: "",
      width: 100
    },
    {
      name: "realSalary",
      displayName: "Real salary",
      isShow: true,
      sortable: true,
      className: "",
      width: 100
    },
    {
      name: "InputSalary",
      displayName: "Input Salary",
      isShow: true,
      sortable: false,
      className: "",
      width: 100
    },
    {
      name: "NormalDay",
      displayName: "Normal day",
      isShow: true,
      sortable: false,
      className: "",
      width: 70
    },
    {
      name: "OTHour",
      displayName: "OT hour",
      isShow: false,
      sortable: false,
      className: "",
      width: 70
    },
    {
      name: "OffDay",
      displayName: "Off day",
      isShow: true,
      sortable: false,
      className: "",
      width: 70
    },
    {
      name: "Opentalk",
      displayName: "Opentalk",
      isShow: false,
      sortable: false,
      className: "",
      width: 70
    },
    {
      name: "LeaveDayBefore",
      displayName: "Leave day before",
      isShow: false,
      sortable: false,
      className: "",
      width: 70
    },
    {
      name: "AddedLeaveDay",
      displayName: "Added leave day",
      isShow: false,
      sortable: false,
      className: "",
      width: 70
    },
    {
      name: "RefundDay",
      displayName: "Refund day",
      isShow: true,
      sortable: false,
      className: "",
      width: 70
    },
    {
      name: "remainLeaveDays",
      displayName: "Remain leave day after",
      isShow: true,
      sortable: true,
      className: "",
      width: 70
    },
    {
      name: "branch",
      displayName: "Branch",
      isShow: false,
      sortable: false,
      className: "",
      width: 70
    },
    {
      name: "userType",
      displayName: "User type",
      isShow: false,
      sortable: false,
      className: "",
      width: 70
    },
    {
      name: "Position",
      displayName: "Position",
      isShow: false,
      sortable: false,
      className: "",
      width: 70
    },
    {
      name: "level",
      displayName: "Level",
      isShow: false,
      sortable: false,
      className: "",
      width: 70
    },
    {
      name: "bankInfo",
      displayName: "Bank info",
      isShow: false,
      sortable: false,
      className: "",
      width: 70
    },
    {
      name: "Team",
      displayName: "Team",
      isShow: false,
      sortable: false,
      className: "",
      width: null
    },
    {
      name: "OT salary",
      displayName: "OT salary",
      isShow: false,
      sortable: false,
      className: "",
      width: 100
    },
    {
      name: "Bonus",
      displayName: "Bonus",
      isShow: true,
      sortable: false,
      className: "",
      width: 100
    },
    {
      name: "Punishment",
      displayName: "Punishment",
      isShow: true,
      sortable: false,
      className: "",
      width: 100
    },
    {
      name: "Debt",
      displayName: "Debt",
      isShow: true,
      sortable: false,
      className: "",
      width: 100
    },
    {
      name: "UpdatedTime",
      displayName: "Updated time",
      isShow: false,
      sortable: false,
      className: "",
      width: 120
    },
    {
      name: "Benefit",
      displayName: "Benefit",
      isShow: true,
      sortable: false,
      className: "benefit-col",
      width: null,
      allowExpan: true
    },
    {
      name: "Complain",
      displayName: "Complain",
      isShow: true,
      sortable: false,
      className: "",
      width: null,
      allowExpan: true
    },
  ]
}
