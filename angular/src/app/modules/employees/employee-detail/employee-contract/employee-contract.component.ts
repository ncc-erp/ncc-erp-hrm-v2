import { Component, Injector, OnInit, ViewChild } from '@angular/core';
import { PagedListingComponentBase, PagedRequestDto } from '@shared/paged-listing-component-base';
import { EmployeeContractService } from '@app/service/api/employee/employeeContract.service'
import { BadgeInfoDto } from '@shared/dto/user-infoDto';
import { EmployeeContractEditNoteComponent } from './employee-contract-edit-note/employee-contract-edit-note.component';
import { LayoutStoreService } from '@shared/layout/layout-store.service';
import { UploadContractFileComponent } from './upload-contract-file/upload-contract-file.component';
import { finalize } from 'rxjs/operators';
import { ESalaryChangeRequestStatus, SalaryChangeRequestStatusList } from '@app/service/model/salary-change-request/GetSalaryChangeRequestDto';
import { ViewMode } from '@app/modules/salary-change-requests/salary-change-request-detail/add-employee-to-salary-change-request/add-employee-to-salary-change-request.component';
import { PERMISSIONS_CONSTANT } from '@app/permission/permission';
import { MatMenuTrigger } from '@angular/material/menu';
import { EditEmployeeContractComponent } from './edit-employee-contract/edit-employee-contract.component';
import { ExportContractComponent } from './export-contract/export-contract.component';
import { EmailFunc } from '@shared/AppEnums';
@Component({
  selector: 'app-employee-contract',
  templateUrl: './employee-contract.component.html',
  styleUrls: ['./employee-contract.component.css']
})
export class EmployeeContractComponent extends PagedListingComponentBase<EmployeeContractDto> implements OnInit {
  @ViewChild(MatMenuTrigger)
  public menu: MatMenuTrigger;
  public contextMenuPosition = { x: '0px', y: '0px' };
  public employeeContractList:EmployeeContractDto[] = []
  public employeeId: number
  public SalaryRequestStatus =  ESalaryChangeRequestStatus
  public SalaryChangeRequestStatusList = SalaryChangeRequestStatusList
  public ViewMode = ViewMode
  public columnList = [
    {
      name: "contractCode",
      displayName: "Contract Code",
      isShow: true,
      className: 'col-contractCode',
      sortable: false
    },
    {
      name: "userTypeId",
      displayName: "User Type",
      isShow: true,
      className: 'col-userType',
      sortable: false,
    },
    {
      name: "startDate",
      displayName: "Start date",
      isShow: true,
      className: 'col-startDate',
      sortable: true
    },
    {
      name: "endDate",
      displayName: "End date",
      isShow: true,
      className: 'col-endDate',
      sortable: true
    },
    {
      name: "salary",
      displayName: "Basic Salary",
      isShow: true,
      className: 'col-salary',
      sortable: false
    },
    {
      name: "probationPercentage",
      displayName: "Probation (%)",
      isShow: true,
      className: 'col-probation',
      sortable: false
    },
    {
      name: "realSalary",
      displayName: "Real salary",
      isShow: true,
      className: 'col-realSalary',
      sortable: false
    },
    {
      name: "jobPositionId",
      displayName: "Position",
      isShow: true,
      className: 'col-position',
      sortable: false
    },
    {
      name: "levelId",
      displayName: "Level",
      isShow: true,
      className: 'col-level',
      sortable: false
    },
    {
      name: "contractFile",
      displayName: "Contract file",
      isShow: true,
      className: 'col-contractFile',
      sortable: false
    },
    {
      name: "note",
      displayName: "Note",
      isShow: true,
      className: 'col-note',
      sortable: false
    },
    {
      name: "request",
      displayName: "Request",
      isShow: true,
      className: 'col-request',
      sortable: false
    },
    {
      name: "updatedTime",
      displayName: "Updated Time",
      isShow: true,
      className: 'col-updatedTime',
      sortable: false
    },
    {
      name: "action",
      displayName: "Action",
      isShow: true,
      className: 'col-action',
      sortable: false
    },
  ]
  protected list(request: PagedRequestDto, pageNumber: number, finishedCallback: Function): void {
    if(this.isAllowViewTabContract()){
      request.filterItems = [
        {
          propertyName: 'EmployeeId',
          value: this.employeeId,
          comparision: this.APP_ENUM.filterComparison.EQUAL
        },
      ],
      this.subscription.push(
        this.employeeContractService.getAllPagging(request).pipe(finalize(()=>finishedCallback())).subscribe(rs => {
          this.employeeContractList = rs.result.items
          this.showPaging(rs.result,pageNumber)
        })
      )
    } 
  }

  constructor(injector: Injector,
    private employeeContractService: EmployeeContractService,
    private layoutService: LayoutStoreService,
  ) {
    super(injector);
  }


  ngOnInit(): void {
    this.layoutService.setSidebarExpanded(true);
    this.employeeId = this.activatedRoute.snapshot.queryParams["id"];
    if(!this.sortDirection){
      this.sortDirection = this.sortDirectionEnum.Descending;
      this.sortProperty = "startDate"
    }
    this.refresh();
  }
  isShowEditNoteBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Employee_EmployeeDetail_TabContract_EditNote);
  }
  isShowImportBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Employee_EmployeeDetail_TabContract_ImportContractFile);
  }
  isShowDeleteBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Employee_EmployeeDetail_TabContract_Delete);
  }
  isShowDeleteContractFileBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Employee_EmployeeDetail_TabContract_DeleteContractFile);
  }
  isAllowViewTabContract(){
    return this.isGranted(PERMISSIONS_CONSTANT.Employee_EmployeeDetail_TabContract_View);
  }
  isShowEditBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Employee_EmployeeDetail_TabContract_Edit);
  }


  openEditNoteDialog(item: EmployeeContractDto){
    this.dialog.open(
      EmployeeContractEditNoteComponent,
      {
        width: '600px',data: {note: item.note, code: item.code, contractId: item.id}
      }).afterClosed().subscribe((data)=>{
        this.refresh()
      })
  }

  public onUploadFile(id: number){
    const dialog = this.dialog.open(UploadContractFileComponent,{
      data: {
        id: id
      }
    });
    dialog.afterClosed().subscribe((rs)=>{
      if(rs){
        this.refresh()
      }
    })
  }

  onDeleteFile(id:number){
    this.confirmDelete(`Delete file?`,
      () => this.employeeContractService.deleleContractFile(id).toPromise().then(rs => {
        abp.notify.success("Delete file successful");
      }))
  }
  deleteContract(id:number){
    this.confirmDelete(`Delete contract?`,
    ()=> this.employeeContractService.deleteContract(id).subscribe(rs =>{
      abp.notify.success("Contract Deleted");
      this.refresh()
    })
    )
  }
  public onPrintLaborContract(contract){
    this.openTemp(contract, EmailFunc.ContractLD,`Hợp đồng lao động`);
  }
  public onPrintCollaboratorContract(contract){
    this.openTemp(contract, EmailFunc.ContractCTV,`Hợp đồng cộng tác viên`);
  }
  public onPrintTrainingContract(contract){
    this.openTemp(contract, EmailFunc.ContractDT,`Hợp đồng đào tạo`);
  }
  public onPrintProbationaryContract(contract){
    this.openTemp(contract, EmailFunc.ContractTV,`Hợp đồng thử việc`);
  }
  public onPrintConfidentialContract(contract){
    this.openTemp(contract, EmailFunc.ContractBM,`Hợp đồng bảo mật`);
  }

  public isShowPrintConfidentialContractBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Employee_EmployeeDetail_TabContract_PrintConfidentialContract)
  }
  public isShowPrintLaborContractBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Employee_EmployeeDetail_TabContract_PrintLaborContract)
  }
  public isShowPrintCollaboratorContractBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Employee_EmployeeDetail_TabContract_PrintCollaboratorContract)
  }
  public isShowPrintTrainingContractBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Employee_EmployeeDetail_TabContract_PrintTrainingContract)
  }
  public isShowPrintProbationaryContractBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Employee_EmployeeDetail_TabContract_PrintProbationaryContract)
  }

  public openTemp(contract, tempType,title: string): void {
    this.dialog.open(ExportContractComponent,{
      data: {
        contractId: contract.id,
        tempType: tempType,
        title: title
      },
      width: '85%',
      maxWidth: '85%',
      panelClass: 'email-dialog',
    });
  }
  
  public onEdit(contract : EmployeeContractDto){
    var dialogRes = this.dialog.open(EditEmployeeContractComponent, {
      data: {
        ...contract
      },
      width: '600px',
      panelClass: 'employee-contract-dialog'
    });
    dialogRes.afterClosed().subscribe((rs)=>{
      if(rs){
        this.refresh()
      }
    })
  }

  
}

export interface EmployeeContractDto {
    id: number,
    employeeId: number,
    startDate: string,
    endDate: string,
    file: string,
    fileName: string,
    fullFilePath: string,
    code: string,
    userType: number,
    userTypeInfo: BadgeInfoDto,
    jobPositionId: number,
    jobPositionInfo: BadgeInfoDto,
    levelId: number,
    levelInfo: BadgeInfoDto,
    basicSalary: number,
    realSalary: number,
    probationPercentage: number,
    updatedTime: string,
    creationTime: string,
    updatedUser: string,
    note: string
    salaryChangeRequestEmployeeId: number
    request: RequestDto
}

export interface FileDto{
  bytes: any;
  fileName: string;
}

export interface RequestDto{
  id: number,
  name: string,
  status: ESalaryChangeRequestStatus,
}