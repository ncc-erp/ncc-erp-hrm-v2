import { Component, Injector, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { JobPositionService } from '@app/service/api/categories/jobPosition.service';
import { LevelService } from '@app/service/api/categories/level.service';
import { UserTypeService } from '@app/service/api/categories/userType.service';
import { EmployeeService } from '@app/service/api/employee/employee.service';
import { SalaryChangeRequestService } from '@app/service/api/salary-change-request/salary-change-request.service';
import { GetEmployeeDto } from '@app/service/model/employee/employee.dto';
import { ESalaryChangeRequestStatus, GetSalaryChangeRequestDto, SalaryChangeRequestStatusList } from '@app/service/model/salary-change-request/GetSalaryChangeRequestDto';
import { PagedListingComponentBase, PagedRequestDto } from '@shared/paged-listing-component-base';
import * as moment from 'moment';
import { forkJoin } from 'rxjs';
import { map, tap } from 'rxjs/operators';
import { AddUpdateEmployeeToSalaryChangeRequestDto } from '@app/service/model/salary-change-request/AddEmployeeToSalaryChangeRequestDto'
import { GetRequestEmployeeDto } from '@app/service/model/salary-change-request/GetRequestEmployee';
import { EmployeeContractService } from '@app/service/api/employee/employeeContract.service';
import { EmployeeContractDto } from '@app/modules/employees/employee-detail/employee-contract/employee-contract.component';
import { UploadContractFileComponent } from './upload-contract-file/upload-contract-file.component';
import { SelectOptionDto } from '@shared/dto/selectOptionDto';
import { UserTypeCode } from '@shared/AppConsts';
import { PERMISSIONS_CONSTANT } from '@app/permission/permission';
import { APP_ENUMS } from '@shared/AppEnums';
@Component({
  selector: 'app-add-employee-to-salary-change-request',
  templateUrl: './add-employee-to-salary-change-request.component.html',
  styleUrls: ['./add-employee-to-salary-change-request.component.css']
})
export class AddEmployeeToSalaryChangeRequestComponent extends PagedListingComponentBase<any> implements OnInit {
  protected list(request: PagedRequestDto, pageNumber: number, finishedCallback: Function): void {
  }
  public readonly ViewMode = ViewMode
  public salaryChangeRequestInfo: GetSalaryChangeRequestDto
  public ERequestStatus = ESalaryChangeRequestStatus
  public SALARY_CHANGE_REQUEST_STATUS = SalaryChangeRequestStatusList
  public levelList: SelectOptionDto[] = []
  public userTypeList: UserTypeOption[] = []
  public jobPositionList: SelectOptionDto[] = []
  public employeeList: SelectOptionDto[] = []
  public currentEmployee = {} as GetEmployeeDto
  public currentRequestEmployee = {} as GetRequestEmployeeDto
  public formGroup: FormGroup
  private formBuilder: FormBuilder = new FormBuilder()
  public requestEmployeeId: number
  public requestId: number
  public currentViewMode: number
  public currentContractInformation: EmployeeContractDto
  public showEndDate: boolean
  public listBreadCrumb = []
  constructor(
    injector: Injector,
    private salaryChangeRequestService: SalaryChangeRequestService,
    private employeeService: EmployeeService,
    private userTypeService: UserTypeService,
    private levelService: LevelService,
    private jobPositionService: JobPositionService,
    private contractService: EmployeeContractService
  ) {
    super(injector)
  }
  public initListBreadCrumb = [
    { name: '<i class="fa-solid fa-house fa-sm"></i>', url: '' },
    { name: ' <i class="fa-solid fa-chevron-right"></i> ' },
    { name: 'Salary change requests', url: '/app/salary-change-requests/list-request' },
    { name: '<i class="fa-solid fa-chevron-right"></i>' },
  ]
  ngOnInit(): void {
    this.requestId = this.activatedRoute.snapshot.queryParams["RequestId"]
    this.getAllSelectOption();
    this.listBreadCrumb = [...this.initListBreadCrumb]
    this.initForm()
    this.activatedRoute.queryParamMap.subscribe(rs => {
      this.requestEmployeeId = this.activatedRoute.snapshot.queryParams["RequestEmployeeId"]
      this.currentViewMode = this.activatedRoute.snapshot.queryParams["ViewMode"] || ViewMode.Create
      if (rs.has("ViewMode")) {
        this.listBreadCrumb = [...this.initListBreadCrumb]
        this.currentViewMode = Number(rs.get("ViewMode"))
        if (this.currentViewMode != ViewMode.Create) {
          this.getViewModeData()
          this.requestInformation.hasContract.disable()
          if (this.currentViewMode == ViewMode.Edit) {
            this.requestInformation.hasContract.enable()
          }
        }
        if (this.currentViewMode == ViewMode.Create) {
          this.getCreateModeData();
        }
        this.bindFormEvent()
      }
    })
  }

  getViewModeData() {
    this.isLoading = true;
    forkJoin(
      [
        this.salaryChangeRequestService.get(this.requestId),
        this.salaryChangeRequestService.getRequestEmployeeById(this.requestEmployeeId),
        this.contractService.getContractBySalaryRequest(this.requestEmployeeId)
      ]
    ).subscribe(rs => {
      this.salaryChangeRequestInfo = rs[0].result
      this.currentRequestEmployee = rs[1].result
      this.currentEmployee = {
        id: rs[1].result.employeeId,
        address: "",
        avatar: rs[1].result.avatar,
        avatarFullPath: rs[1].result.avatarFullPath,
        userType: rs[1].result.userType,
        fullName: rs[1].result.fullName,
        realSalary: rs[1].result.salary,
        jobPositionId: rs[1].result.jobPositionId,
        jobPositionInfo: rs[1].result.jobPositionInfo,
        levelId: rs[1].result.levelId,
        levelInfo: rs[1].result.levelInfo,
        userTypeInfo: rs[1].result.userTypeInfo,
        email: rs[1].result.email,
      } as GetEmployeeDto
      this.setRequestInformation(rs[1].result as GetRequestEmployeeDto)
      if (rs[2].result?.id) {
        this.currentContractInformation = rs[2].result
        this.setContractInformation(this.currentContractInformation)
        this.contractInformation.contractCode.addValidators(Validators.required)
        if (this.currentContractInformation.endDate) {
          this.contractInformation.contractEndDate.setValue(this.formatDateYMD(this.currentContractInformation.endDate))
        }
      } else {
        this.currentContractInformation = {} as EmployeeContractDto
      }
      this.currentRequestEmployee.contractCode = rs[2].result?.code
      this.listBreadCrumb = [...this.initListBreadCrumb];
      this.listBreadCrumb.push(...[
        { name: `${this.salaryChangeRequestInfo.name}`, url: '/app/salary-change-requests/list-request/request-detail', queryParams: { requestId: this.requestId } },
        { name: '<i class="fa-solid fa-chevron-right"></i>' },
        { name: `${this.currentRequestEmployee.fullName}`, url: '' }
      ])
      this.isLoading = false;
    })
  }

  getCreateModeData() {
    this.isLoading = true;
    this.salaryChangeRequestService.get(this.requestId).subscribe(rs => {
      this.salaryChangeRequestInfo = rs.result
      this.requestInformation.applyDate.setValue(this.formatDateYMD(new Date(rs.result.applyMonth)))
      this.listBreadCrumb = [...this.initListBreadCrumb,
      { name: `Add employee to ` },
      { name: `${this.salaryChangeRequestInfo.name}`, url: '/app/salary-change-requests/list-request/request-detail', queryParams: { requestId: this.requestId } },
      ];
      this.requestInformation.applyDate.setValue(this.formatDateYMD(this.salaryChangeRequestInfo.applyMonth))
      this.requestInformation.hasContract.setValue(true)
      this.isLoading = false;
    })
  }

  getAllSelectOption() {
    this.isLoading = true;
    this.subscription.push(
      forkJoin(
        [
          this.userTypeService.getAll(),
          this.levelService.getAll(),
          this.jobPositionService.getAll(),
          this.salaryChangeRequestService.getEmployeesNotInRequest(this.requestId)
        ]
      ).pipe(map(x => [x[0].result, x[1].result, x[2].result, x[3].result])).subscribe(rs => {
        this.userTypeList = rs[0].map(x => ({ key: x.name, value: x.id, color: x.color, contractPeriod: x.contractPeriodMonth, code: x.code }))
        this.levelList = rs[1].map(x => ({ key: x.name, value: x.id, color: x.color }))
        this.jobPositionList = rs[2].map(x => ({ key: x.name, value: x.id, color: x.color }))
        this.employeeList = rs[3].map(x => ({ key: x.fullName + " (" + x.email + ")", value: x.employeeId }))
        this.isLoading = false;
      })
    )
  }

  getRequestEmployee(id: number) {
    this.salaryChangeRequestService.getRequestEmployeeById(id).subscribe(rs => {
      this.currentEmployee = {
        id: rs.result.employeeId,
        email: rs.result.email,
        avatar: rs.result.avatar,
        avatarFullPath: rs.result.avatarFullPath,
        fullName: rs.result.fullName,
        realSalary: rs.result.salary,
        userType: rs.result.userType,
        userTypeInfo: rs.result.userTypeInfo,
        jobPositionId: rs.result.jobPositionId,
        jobPositionInfo: rs.result.jobPositionInfo,
        levelId: rs.result.levelId,
        levelInfo: rs.result.levelInfo,
      } as GetEmployeeDto
      this.currentRequestEmployee = rs.result;
      this.setRequestInformation(rs.result)
    })
  }

  initForm() {
    this.formGroup = this.formBuilder.group({
      employeeId: [null, Validators.required],
      requestInformation: this.formBuilder.group({
        userType: [null, Validators.required],
        levelId: [null, Validators.required],
        jobPosition: "",
        jobPositionId: [null, Validators.required],
        realSalary: 0,
        toUserType: [null, Validators.required],
        toJobPositionId: [null, Validators.required],
        toLevelId: [null, Validators.required],
        toRealSalary: [0, Validators.required],
        applyDate: [, Validators.required],
        note: "",
        hasContract: this.currentViewMode == ViewMode.Create ? true : false,
      }),
      contractInformation: this.formBuilder.group({
        contractCode: "",
        probationPercentage: 100,
        basicSalary: [0, Validators.required],
        startDate: this.formatDateYMD(new Date()),
        realSalary: 0,
        contractEndDate: null,
        file: "",
      })
    })
  }

  bindFormEvent() {
    this.formGroup.controls.employeeId.valueChanges.pipe(
      tap(value => {
        if (value) return this.employeeService.get(value).subscribe(value => {
          if (this.currentViewMode == ViewMode.Create) {
            this.currentEmployee = value.result.employeeInfo
            this.formGroup.patchValue({
              requestInformation: {
                userType: value.result.employeeInfo.userType,
                levelId: value.result.employeeInfo.levelId,
                jobPositionId: value.result.employeeInfo.jobPositionId,
                realSalary: value.result.employeeInfo.realSalary,
                note: "",
                jobPosition: value.result.employeeInfo.jobPositionInfo.name,
                toLevelId: value.result.employeeInfo.levelId,
                toUserType: value.result.employeeInfo.userType,
                toJobPositionId: value.result.employeeInfo.jobPositionId
              }
            })
          }
        })
      })).subscribe()

    this.requestInformation.applyDate.valueChanges.subscribe(value => {
      if (this.requestInformation.userType.value && this.requestInformation.toUserType.value) {
        const endDate = this.calculateContractEndDate(this.requestInformation.userType.value, this.requestInformation.toUserType.value, value)
        this.contractInformation.contractEndDate.setValue(endDate)
      }
    })
    this.requestInformation.toUserType.valueChanges.subscribe(value => {
      if (this.requestInformation.userType.value != null && this.requestInformation.applyDate.value) {
        const endDate = this.calculateContractEndDate(this.requestInformation.userType.value, value, this.requestInformation.applyDate.value)
        this.contractInformation.contractEndDate.setValue(endDate)
      }
    })
    this.contractInformation.probationPercentage.valueChanges.subscribe(value => {
      if (this.requestInformation.toRealSalary.value && value) {
        const basicSalary = Math.round(this.requestInformation.toRealSalary.value / (value / 100));
        this.contractInformation.basicSalary.setValue(basicSalary);
      }
      else {
        this.contractInformation.basicSalary.setValue(this.requestInformation.toRealSalary.value);
      }
    })
    this.requestInformation.toRealSalary.valueChanges.subscribe(value => {
      if (this.contractInformation.probationPercentage.value && value) {
        const basicSalary = Math.round(value / (this.contractInformation.probationPercentage.value / 100))
        this.contractInformation.basicSalary.setValue(basicSalary);
        this.contractInformation.realSalary.setValue(value);
      }
      else {
        this.contractInformation.basicSalary.setValue(value);
        this.contractInformation.realSalary.setValue(value);
      }
    })
    this.requestInformation.hasContract.valueChanges.subscribe(value => {
      if (!this.currentContractInformation?.id) {
        this.contractInformation.probationPercentage.setValue(100)
      }
      if (!value) {
        this.contractInformation.contractCode.removeValidators(Validators.required);
      }
    })
  }
  public get requestInformation() {
    return (this.formGroup.get('requestInformation') as FormGroup).controls
  }

  public get contractInformation() {
    return (this.formGroup.get('contractInformation') as FormGroup).controls
  }

  public getNewUserTypeName() {
    if (this.requestInformation.toUserType.value) {
      return this.userTypeList?.find(x => x.value == this.requestInformation.toUserType.value)?.key
    }
    return "";
  }

  public getNewLevelName() {
    if (this.requestInformation.toLevelId.value) {
      return this.levelList?.find(x => x.value == this.requestInformation.toLevelId.value)?.key
    }
    return "";
  }

  public getNewJobPositionName() {
    if (this.requestInformation.toJobPositionId.value) {
      return this.jobPositionList?.find(x => x.value == this.requestInformation.toJobPositionId.value)?.key
    }
    return "";
  }

  setContractInformation(contractDto: EmployeeContractDto) {
    this.formGroup.patchValue({
      contractInformation: {
        probationPercentage: contractDto?.probationPercentage,
        basicSalary: contractDto?.basicSalary || 0,
        startDate: this.formatDateYMD(contractDto?.startDate),
        realSalary: contractDto?.realSalary || 0,
        contractEndDate: contractDto?.endDate ? this.formatDateYMD(contractDto.endDate) : null,
        contractCode: contractDto?.code || "",
        file: contractDto?.fileName || ""
      }
    })
  }

  setRequestInformation(requestEmployeDto: GetRequestEmployeeDto) {
    this.formGroup.patchValue({
      employeeId: requestEmployeDto.employeeId,
      requestInformation: {
        userType: requestEmployeDto.fromUserType,
        levelId: requestEmployeDto.fromLevelId,
        jobPosition: requestEmployeDto.jobPositionName,
        jobPositionId: requestEmployeDto.fromJobPositionId,
        realSalary: requestEmployeDto.salary,
        toUserType: requestEmployeDto.toUserType,
        toJobPositionId: requestEmployeDto.toJobPositionId,
        toLevelId: requestEmployeDto.toLevelId,
        toRealSalary: requestEmployeDto.toSalary,
        applyDate: this.formatDateYMD(requestEmployeDto.applyDate),
        note: requestEmployeDto.note,
        contractCode: requestEmployeDto.contractCode,
        employeeId: requestEmployeDto.employeeId,
        hasContract: requestEmployeDto.hasContract,
      }
    })
  }

  public onSave() {
    // if(this.requestInformation.toUserType.value == APP_ENUMS.ProbationaryStaff){

    // }
    const salaryRequestEmployee: AddUpdateEmployeeToSalaryChangeRequestDto = {
      id: 0,
      applyDate: this.formatDateYMD(this.requestInformation.applyDate.value),
      employeeId: this.currentEmployee.id,
      contractEndDate: this.contractInformation.contractEndDate.value ? this.formatDateYMD(this.contractInformation.contractEndDate.value) : null,
      fromUserType: this.requestInformation.userType.value,
      jobPositionId: this.requestInformation.jobPositionId.value,
      levelId: this.requestInformation.levelId.value,
      note: this.requestInformation.note.value,
      salary: this.requestInformation.realSalary.value,
      contractCode: "" || this.contractInformation.contractCode.value,
      file: "" || this.contractInformation.file.value,
      status: this.salaryChangeRequestInfo.status,
      salaryChangeRequestId: this.salaryChangeRequestInfo.id,
      toUserType: this.requestInformation.toUserType.value,
      toLevelId: this.requestInformation.toLevelId.value,
      toJobPositionId: this.requestInformation.toJobPositionId.value,
      toSalary: this.contractInformation.realSalary.value,
      hasContract: this.requestInformation.hasContract.value,
      basicSalary: this.requestInformation.toUserType.value ? this.contractInformation.basicSalary.value : this.contractInformation.realSalary.value,
      probationPercentage: this.requestInformation.toUserType.value ? this.contractInformation.probationPercentage.value : 100,
    }
    if (this.requestEmployeeId) {
      salaryRequestEmployee.id = Number(this.requestEmployeeId);
    }
    this.save(salaryRequestEmployee)
  }

  public onEdit() {
    this.formGroup.reset({
      employeeId: this.requestInformation.employeeId,
      requestInformation: {
        toRealSalary: this.requestInformation.toRealSalary.value
      },
      contractInformation: {
        contractEndDate: this.currentContractInformation?.endDate
      }
    })
    this.router.navigate(
      ['/app', 'salary-change-requests', 'list-request', 'request-detail', 'request-employee'],
      { queryParams: { RequestId: this.requestId, RequestEmployeeId: this.requestEmployeeId, ViewMode: ViewMode.Edit } })
  }

  public onCancel() {
    this.formGroup.reset({
      employeeId: this.requestInformation.employeeId
    })
    this.router.navigate(
      ['/app', 'salary-change-requests', 'list-request', 'request-detail', 'request-employee'],
      { queryParams: { RequestId: this.requestId, RequestEmployeeId: this.requestEmployeeId, ViewMode: ViewMode.View } })
  }

  public onBack() {
    this.router.navigate(
      ['/app', 'salary-change-requests', 'list-request', 'request-detail'],
      { queryParams: { requestId: this.requestId} })
  }

  public openUploadDialog() {
    this.dialog.open(UploadContractFileComponent, {
      data: {
        id: this.currentContractInformation.id.toString()
      }
    }).afterClosed().subscribe(rs => {
      if (rs) {
        this.getViewModeData();
        this.router.navigate(
          ['/app', 'salary-change-requests','list-request', 'request-employee'],
          { queryParams: { RequestId: this.requestId, RequestEmployeeId: this.requestEmployeeId, ViewMode: this.currentViewMode } })
      }
    })
  }

  save(salaryRequestEmployee: AddUpdateEmployeeToSalaryChangeRequestDto) {
    if (salaryRequestEmployee.id) {
      this.update(salaryRequestEmployee)
    } else {
      this.create(salaryRequestEmployee)
    }
  }

  create(addEmployeeToRequest: AddUpdateEmployeeToSalaryChangeRequestDto) {
    this.isLoading = true;
    this.subscription.push(
      this.salaryChangeRequestService.addEmployeeToSalaryRequest(addEmployeeToRequest).subscribe(rs => {
        this.notify.success("Request employee created")
        const requestEmployeeId = rs.result;
        this.isLoading = false;
        this.router.navigate(
          [],
          {
            queryParamsHandling: "merge",
            replaceUrl: true,
            queryParams: {
              RequestId: this.requestId, RequestEmployeeId: requestEmployeeId, ViewMode: ViewMode.View
            }
          })
      },() => this.isLoading = false)
    )
  }

  update(updateEmployeeToSalaryRequestDto: AddUpdateEmployeeToSalaryChangeRequestDto) {
    this.isLoading = true;
    this.subscription.push(
      this.salaryChangeRequestService.updateRquestEmployee(updateEmployeeToSalaryRequestDto).subscribe(rs => {
        this.notify.success("Request employee updated")
        this.isLoading = false;
        this.router.navigate(
          [],
          {
            queryParamsHandling: "merge",
            replaceUrl: true, queryParams: { RequestId: this.requestId, RequestEmployeeId: updateEmployeeToSalaryRequestDto.id, ViewMode: ViewMode.View }
          })
      },() => this.isLoading = false)
    )
  }

  deleteContractFile(employeeContractId: number) {
    this.confirmDelete("Delete contract file", () => {
      this.subscription.push(
        this.contractService.deleleContractFile(employeeContractId).subscribe(rs => {
          this.notify.success("Contract file deleted")
          this.isLoading = false;
          this.getViewModeData();
        })
      )
    })
  }

  public isValid() {
    return this.formGroup.valid
  }

  public getUserTypeColor(id: number) {
    return this.userTypeList.find(item => item.value == id)?.color
  }

  public getUserTypeName(id: number){
    return this.userTypeList.find(item => item.value == id)?.key
  }

  public getLevelColor(id: number) {
    return this.levelList.find(item => item.value == id)?.color
  }

  public getLevelName(id: number) {
    return this.levelList.find(item => item.value == id)?.key
  }

  public getJobPositionColor(id: number) {
    return this.jobPositionList.find(item => item.value == id)?.color
  }

  public calculateContractEndDate(olduserType: number, newuserType: number, startDate: Date) {
    const userTypeInfo = this.userTypeList.find(item => item.value == newuserType)
    if (userTypeInfo) {
      if(userTypeInfo.contractPeriod){
        const endDate = moment(startDate).startOf('day').add(userTypeInfo.contractPeriod, 'months').subtract(1, 'day').toDate()
        return this.formatDateYMD(endDate);
      }
      return null;
    }
    return null;
  }
  isShowEditBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.SalaryChangeRequest_SalaryChangeRequestDetail_SalaryChangeRequestEmployeeDetail_Edit);
  }
  isShowUploadContractFileBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.SalaryChangeRequest_SalaryChangeRequestDetail_SalaryChangeRequestEmployeeDetail_UploadContractFile);
  }
  isShowDeleteContractFileBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.SalaryChangeRequest_SalaryChangeRequestDetail_SalaryChangeRequestEmployeeDetail_DeleteContractFile);
  }
}

export enum ViewMode {
  Create = 0,
  View = 1,
  Edit = 2
}
export interface UserTypeOption extends SelectOptionDto {
  contractPeriod: number,
  code: string
}