import { ImportCheckpointComponent } from './import-checkpoint/import-checkpoint.component';
import { Component, Injector, OnInit } from '@angular/core';
import { JobPositionService } from '@app/service/api/categories/jobPosition.service';
import { LevelService } from '@app/service/api/categories/level.service';
import { UserTypeService } from '@app/service/api/categories/userType.service';
import { SalaryChangeRequestService } from '@app/service/api/salary-change-request/salary-change-request.service';
import { GetRequestEmployeeDto } from '@app/service/model/salary-change-request/GetRequestEmployee'
import { ESalaryChangeRequestStatus, GetSalaryChangeRequestDto, SalaryChangeRequestStatusList } from '@app/service/model/salary-change-request/GetSalaryChangeRequestDto';
import { map } from 'rxjs/operators';
import { forkJoin } from 'rxjs';
import { EmployeeContractService } from '@app/service/api/employee/employeeContract.service';
import { PagedListingComponentBase, PagedRequestDto } from '@shared/paged-listing-component-base';
import { ViewMode } from './add-employee-to-salary-change-request/add-employee-to-salary-change-request.component';
import { UpdateChangeRequestDto } from '@app/service/model/salary-change-request/UpdateChangeRequestDto';
import { PERMISSIONS_CONSTANT } from '@app/permission/permission';
import { BranchService } from '@app/service/api/categories/branch.service';
import { InputGetEmployeeInSalaryRequestDto } from 'app/service/model/salary-change-request/InputGetEmployeeInSalaryRequestDto'
import { MailDialogComponent } from '@app/modules/admin/email-templates/mail-dialog/mail-dialog.component';
import { SendCheckpointMailToOneEmployeeDto } from '@app/service/model/mail/sendMail.dto';
import * as FileSaver from 'file-saver';
@Component({
  selector: 'app-salary-change-request-detail',
  templateUrl: './salary-change-request-detail.component.html',
  styleUrls: ['./salary-change-request-detail.component.css']
})
export class SalaryChangeRequestDetailComponent extends PagedListingComponentBase<GetRequestEmployeeDto> implements OnInit {
  protected list(request: PagedRequestDto, pageNumber: number, finishedCallback: Function): void {
    let input = {
      gridParam: request,
      branchIds: this.branchIds,
      toUsertypes: this.toUserTypeIds,
      toLevelIds: this.toLevelIds,
      toJobPositionIds: this.toJobPositionIds
    } as InputGetEmployeeInSalaryRequestDto;
    this.inputToFilter = input;
    this.isLoading = true;
    this.subscription.push(
      this.salaryChangeRequestService.getEmployeesInSalaryRequest(this.requestId, input)
        .subscribe({
          next: (rs) => {
            this.listRequestEmployee = rs.result.items
            this.showPaging(rs.result, pageNumber)
            this.getContractInfo(this.listRequestEmployee.map(rq => rq.id))
          },
          complete: () => {
            this.isLoading = false
          }
        })
    )
  }
  public listRequestEmployee: GetRequestEmployeeDto[] = []
  public salaryChangeRequestInfo: GetSalaryChangeRequestDto
  public ERequestStatus = ESalaryChangeRequestStatus
  public SALARY_CHANGE_REQUEST_STATUS = SalaryChangeRequestStatusList
  public userTypeList = []
  public branchList = []
  public levelList = []
  public jobPositionList = []
  public requestId: number
  public ViewMode = ViewMode
  public initListBreadCrumb = [
    { name: '<i class="fa-solid fa-house fa-sm"></i>', url: '' },
    { name: ' <i class="fa-solid fa-chevron-right"></i> ' },
    { name: 'Salary change request', url: `/app/salary-change-requests/list-request` },
    { name: '<i class="fa-solid fa-chevron-right"></i>' }
  ]
  public listBreadCrumb = []
  public filterMultipleTypeParamEnum = this.APP_ENUM.FilterMultipleTypeParamEnum;
  public inputToFilter = {} as InputGetEmployeeInSalaryRequestDto;

  public defaultValue = {
    userType: null,
    level: null,
    branch: null,
    position: null
  }
  constructor(
    injector: Injector,
    private salaryChangeRequestService: SalaryChangeRequestService,
    private userTypeService: UserTypeService,
    private levelService: LevelService,
    private jobPositionService: JobPositionService,
    private contractService: EmployeeContractService,
    private branchService: BranchService
  ) {
    super(injector)
  }
  ngOnInit(): void {
    this.requestId = Number(this.activatedRoute.snapshot.queryParamMap.get("requestId"))
    this.getSalaryChangeRequestInfo(this.requestId)
    this.getEmployeeInRequest(this.requestId)
    this.getAllSelectOption()
    this.listBreadCrumb = [...this.initListBreadCrumb]
    this.refresh()
  }
  getSalaryChangeRequestInfo(id: number) {
    this.isLoading = true;
    this.subscription.push(
      this.salaryChangeRequestService.get(id).subscribe(rs => {
        this.salaryChangeRequestInfo = rs.result
        this.listBreadCrumb =
          [
            ...this.initListBreadCrumb,
            {
              name: `${this.salaryChangeRequestInfo.name} <span class='badge badge-pill text-white ml-1 ${this.SALARY_CHANGE_REQUEST_STATUS[this.salaryChangeRequestInfo.status].class}'>${this.SALARY_CHANGE_REQUEST_STATUS[this.salaryChangeRequestInfo.status].key}</span>`,
              url: null
            }
          ]
        this.isLoading = false
      })
    )
  }

  getEmployeeInRequest(id: number) {

  }
  getContractInfo(listId: number[]) {
    forkJoin(listId.map(x => this.contractService.getContractBySalaryRequest(x))).subscribe(result => {
      result.forEach(response => {
        let index = this.listRequestEmployee.findIndex(item => response.result?.salaryRequestEmployeeId == item.id)
        if (index != -1) {
          this.listRequestEmployee[index].contractCode = response.result.code
          this.listRequestEmployee[index].contractEndDate = response.result.endDate
        }
      })
    })
  }
  getAllSelectOption() {
    this.subscription.push(
      forkJoin(
        [
          this.userTypeService.getAll(),
          this.levelService.getAll(),
          this.jobPositionService.getAll(),
          this.branchService.getAll()
        ]
      ).pipe(map(x => [x[0].result, x[1].result, x[2].result, x[3].result])).subscribe(rs => {
        this.userTypeList = rs[0].map(x => ({ key: x.name, value: x.id }))
        this.levelList = rs[1].map(x => ({ key: x.name, value: x.id }))
        this.jobPositionList = rs[2].map(x => ({ key: x.name, value: x.id }))
        this.branchList = rs[3].map(x => ({key: x.name, value: x.id}))
      })
    )
  }
  update(status: ESalaryChangeRequestStatus) {
    let payload: UpdateChangeRequestDto = {
      requestId: this.requestId,
      status: status
    }
    if (status == ESalaryChangeRequestStatus.Executed) {
      abp.message.confirm(`Execute ${this.salaryChangeRequestInfo.name}?`, "", (result, info) => {
        if (result) {
          this.salaryChangeRequestService.updateRequestStatus(payload).subscribe(rs => {
            if(rs.success) {
              abp.notify.success(`<b>${this.salaryChangeRequestInfo.name}<b/> executed`,"", {isHtml: true})
            }
            this.getSalaryChangeRequestInfo(this.requestId)
          })
        }
      })
    } else {
      this.salaryChangeRequestService.updateRequestStatus(payload).subscribe(rs => {
        if(rs.success) {
          abp.notify.success(`<b>${this.salaryChangeRequestInfo.name}</b> status updated to ${this.SALARY_CHANGE_REQUEST_STATUS[status].key}`, "", {isHtml: true})
          this.getSalaryChangeRequestInfo(this.requestId)
        }
      })
    }
  }

  delete(requestEmployee: GetRequestEmployeeDto) {
    this.confirmDelete(`Delete salary change request for <strong>${requestEmployee.fullName}</strong>`, () => {
      this.subscription.push(
        this.salaryChangeRequestService.deleteSalaryRequestEmployee(requestEmployee.id).subscribe(rs => {
          this.notify.success("Request deleted")
          this.isLoading = false;
          this.refresh()
        })
      )
    })
  }

  public getUserTypeName(userTypeId: number) {
    if (userTypeId) {
      return this.userTypeList?.find(x => x.value == userTypeId)?.key
    }
    return "";
  }

  public getLevelName(levelId: number) {
    if (levelId) {
      return this.levelList?.find(x => x.value == levelId)?.key
    }
    return "";
  }

  public importCheckpoint(){
    let ref = this.dialog.open(ImportCheckpointComponent, {
      width: "500px",
      data: this.requestId
    })
    ref.afterClosed().subscribe(rs => {
      this.refresh();
    })
  }

  public onSendAllMail(){
    abp.message.confirm("Are you sure to send mail to all?","", ((rs)=>{
      if(rs){
        this.isLoading = true;
        this.inputToFilter.gridParam.maxResultCount = 500000;
        this.subscription.push(
          this.salaryChangeRequestService.sendAllMail(this.requestId, this.inputToFilter).subscribe((rs)=>{
            abp.message.success(rs.result);
            this.isLoading = false;
          },()=> this.isLoading = false)
        )
      }
    }))
  }

  public onSendMail(id: number){
    this.subscription.push(
      this.salaryChangeRequestService.getCheckpointTemplate(id).subscribe((rs)=>{
        const dialogData = {
          showEditButton: true,
          mailInfo: rs.result,
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
             var input: SendCheckpointMailToOneEmployeeDto = {
               requestId: id,
               mailContent: rs
             }
             this.subscription.push(
               this.salaryChangeRequestService.sendMail(input).subscribe((rs)=>{
                abp.message.success(`Mail sent to ${dialogData.mailInfo.sendToEmail}!`)
               })
             )
           }
         })
      })
    )
  }

  public onDownloadTemps(){
    this.subscription.push(
      this.salaryChangeRequestService.getTemplateToImportCheckpoint().subscribe((rs) => {
        const file = new Blob([this.convertFile(atob(rs.result.base64))], {
          type: "application/vnd.ms-excel;charset=utf-8"
        });
        FileSaver.saveAs(file, `${rs.result.fileName}.xlsx`)
      })

    )
  }


  isShowAddBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.SalaryChangeRequest_SalaryChangeRequestDetail_Add);
  }
  isShowDeleteBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.SalaryChangeRequest_SalaryChangeRequestDetail_Delete);
  }
  isShowSendBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.SalaryChangeRequest_SalaryChangeRequestDetail_Send);
  }
  isShowApproveBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.SalaryChangeRequest_SalaryChangeRequestDetail_Approve);
  }
  isShowRejectBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.SalaryChangeRequest_SalaryChangeRequestDetail_Reject);
  }
  isShowExecuteBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.SalaryChangeRequest_SalaryChangeRequestDetail_Execute);
  }
  isAllowRoutingDetail(){
    return this.isGranted(PERMISSIONS_CONSTANT.SalaryChangeRequest_SalaryChangeRequestDetail_SalaryChangeRequestEmployeeDetail_View);
  }
  isAllowViewTabPersonalInfo(){
    return this.isGranted(PERMISSIONS_CONSTANT.Employee_EmployeeDetail_TabPersonalInfo_View);
  }
  isShowSendAllMailBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.SalaryChangeRequest_SalaryChangeRequestDetail_SendAllMail)
    && ( this.salaryChangeRequestInfo.status == this.ERequestStatus.Pending
      || this.salaryChangeRequestInfo.status == this.ERequestStatus.Approved
      || this.salaryChangeRequestInfo.status == this.ERequestStatus.Executed);
  }
  isShowSendMailBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.SalaryChangeRequest_SalaryChangeRequestDetail_SendMail)
     && ( this.salaryChangeRequestInfo.status == this.ERequestStatus.Pending
     || this.salaryChangeRequestInfo.status == this.ERequestStatus.Approved
     || this.salaryChangeRequestInfo.status == this.ERequestStatus.Executed);
  }
  isShowImportCheckpoint(){
    return this.isGranted(PERMISSIONS_CONSTANT.SalaryChangeRequest_SalaryChangeRequestDetail_ImportCheckpoint) && this.salaryChangeRequestInfo.status != this.ERequestStatus.Executed;
  }
  isShowDownloadTemplate(){
    return this.isGranted(PERMISSIONS_CONSTANT.SalaryChangeRequest_SalaryChangeRequestDetail_DownloadTemplate) && this.salaryChangeRequestInfo.status != this.ERequestStatus.Executed;
  }

}
