import {
  Component,
  EventEmitter,
  Injector,
  OnInit,
  Output,
  ViewChild,
} from "@angular/core";
import { FormBuilder, FormGroup, Validators } from "@angular/forms";
import { BankService } from "@app/service/api/categories/bank.service";
import { BranchService } from "@app/service/api/categories/branch.service";
import { JobPositionService } from "@app/service/api/categories/jobPosition.service";
import { LevelService } from "@app/service/api/categories/level.service";
import { SkillService } from "@app/service/api/categories/skill.service";
import { TeamService } from "@app/service/api/categories/team.service";
import { UserTypeService } from "@app/service/api/categories/userType.service";
import { EmployeeService } from "@app/service/api/employee/employee.service";
import { CreateUpdateEmployeeDto } from "@app/service/model/employee/employee.dto";
import { BaseEmployeeDto } from "@shared/dto/user-infoDto";
import { PagedRequestDto } from "@shared/paged-listing-component-base";
import {
  PagedListingComponentBase,
} from "@shared/paged-listing-component-base";
import { SelectOptionDto } from '@shared/dto/selectOptionDto'
import { UploadAvatarComponent } from "../../upload-avatar/upload-avatar.component";
import { ChangeBranchComponent } from "./change-branch/change-branch.component";
import { ChangeStatusToQuitComponent } from "./change-status/change-status-to-quit/change-status-to-quit.component";
import { ChangeStatusToPauseComponent } from "./change-status/change-status-to-pause/change-status-to-pause.component";
import { ChangeStatusToMaternityLeaveComponent } from "./change-status/change-status-to-maternity-leave/change-status-to-maternity-leave.component";
import { ChangeStatusToWorkingComponent } from "./change-status/change-status-to-working/change-status-to-working.component";
import { ExtendMaternityLeaveComponent } from "./change-status/extend-maternity-leave/extend-maternity-leave.component";
import { MatMenuTrigger } from "@angular/material/menu";
import { ExtendPausingComponent } from "./change-status/extend-pausing/extend-pausing.component";
import { APP_ENUMS } from "@shared/AppEnums";
import { PERMISSIONS_CONSTANT } from "@app/permission/permission";
import { WarningEmployeeService } from "@app/service/api/warning-employee/warning-employee.service";
import { TempEmployeeTalentDto } from "@app/service/model/warning-employee/WarningEmployeeDto";
import { GetRequestDetailDto, RejectChangeInfoDto, UpdateRequestDetailDto } from "@app/service/model/warning-employee/WarningEmployeeDto";
import { IssuedByService } from "@app/service/api/categories/issuedBy.service";

@Component({
  selector: "app-personal-info",
  templateUrl: "./personal-info.component.html",
  styleUrls: ["./personal-info.component.css"],
})
export class PersonalInfoComponent
  extends PagedListingComponentBase<PersonalInfo>
  implements OnInit {
  @Output() onUpdate: EventEmitter<boolean> = new EventEmitter<boolean>();
  @ViewChild(MatMenuTrigger)
  public menu: MatMenuTrigger;
  public contextMenuPosition = { x: '0px', y: '0px' };
  public personalInfo: PersonalInfo;
  public userId: number;
  public requestId:number;
  public isEdit = false;
  public userTypeList = [];
  public branchList: SelectOptionDto[] = [];
  public jobPositionList: SelectOptionDto[] = [];
  public teamList: SelectOptionDto[] = [];
  public levelList: SelectOptionDto[] = [];
  public bankList: SelectOptionDto[] = [];
  public isAllowWorkingStatus: boolean = true;
  public isAllowBranchStatus: boolean = true;
  public listIssuedBys : SelectOptionDto[] = [];

  public workingStatusList: SelectOptionDto[] = Object.entries(this.APP_ENUM.UserStatus).map(
    (x) => ({ key: x[0], value: x[1] })
  );
  public genderList: SelectOptionDto[] = Object.entries(this.APP_ENUM.Gender).map((x) => ({
    key: x[0],
    value: x[1],
  }));
  public insuranceStatusList = Object.entries(
    this.APP_ENUM.InsuranceStatus
  ).map((x) => ({ key: x[0], value: x[1] }));
  public skillList = [];
  public isAllowEdit: boolean = true;
  public formBuilder: FormBuilder = new FormBuilder();
  public formGroup: FormGroup;
  public isShowSubButtonChangeStatus: boolean = false;
  public isShowProbation:boolean = false;
  public isShowBasicSalary:boolean = false;
  public isShowRealSalary:boolean = false;
  public requestUpdateInfoDetail = {} as GetRequestDetailDto;
  public inputToUpdate = {} as RejectChangeInfoDto;
  public isUpdateRequestMode: boolean = false
  
  constructor(
    injector: Injector,
    private employeeService: EmployeeService,
    private userTypeService: UserTypeService,
    private branchService: BranchService,
    private jobPositionService: JobPositionService,
    private teamService: TeamService,
    private skilsSerivce: SkillService,
    private levelService: LevelService,
    private bankService: BankService,
    private warningEmployeeService:WarningEmployeeService,
    private issuedByService : IssuedByService,
  ) {
    super(injector);
    this.tempEmployeeId = Number(this.activatedRoute.snapshot.queryParamMap.get("tempEmployeeId"));
  }

  ngOnInit(): void {
    this.personalInfo = {} as PersonalInfo;
    this.getAllUserType()
    this.getAllTeam();
    this.getAllBranch();
    this.getAllJobPosition();
    this.getAllSkill();
    this.getAllLevel();
    this.getAllBank();
    this.getTempEmployeeInfo();
    this.getAllIssuedBy();
    this.initForm();
    this.formGroup.disable()
    this.userId = Number(this.activatedRoute.snapshot.queryParamMap.get("id"));
    this.isEdit = Boolean(this.activatedRoute.snapshot.queryParamMap.get("isEdit")) || false;
    this.requestId = Number(this.activatedRoute.snapshot.queryParamMap.get("requestId"));
    if (this.userId) {
      this.getEmployeeById(this.userId);
    } else {
      this.isEdit = true;
      this.formGroup.enable()
    }
    if (this.isEdit) {
      this.formGroup.enable()
    }

    if(this.requestId){
      this.isUpdateRequestMode = true;
      this.formGroup.enable();
      this.getRequestDetailById(this.requestId);
    }


  }
  getTempEmployeeInfo(){
    if(this.tempEmployeeId){
      this.subscription.push(
      this.warningEmployeeService.GetTempEmployeeTalentById(this.tempEmployeeId).subscribe(rs=>{
        this.tempEmployeetalentInfo = rs.result
        this.formGroup.controls['email'].setValue(this.tempEmployeetalentInfo.nccEmail);
        this.formGroup.controls['surname'].setValue(this.tempEmployeetalentInfo.fullName.split(" ").slice(0, -1).join(' '));
        this.formGroup.controls['name'].setValue(this.tempEmployeetalentInfo.fullName.split(" ").slice(-1).join(' '));
        this.formGroup.controls['phone'].setValue(this.tempEmployeetalentInfo.phone);
        this.formGroup.controls['userType'].setValue(this.tempEmployeetalentInfo.userType);
        this.formGroup.controls['branchId'].setValue(this.tempEmployeetalentInfo.branchId);
        this.formGroup.controls['jobPositionId'].setValue(this.tempEmployeetalentInfo.jobPositionId);
        this.formGroup.controls['levelId'].setValue(this.tempEmployeetalentInfo.levelId);
        this.formGroup.controls['status'].setValue(this.APP_ENUM.UserStatus.Working);
        this.formGroup.controls['salary'].patchValue(this.tempEmployeetalentInfo.salary);
        this.formGroup.controls['probationPercentage'].patchValue(this.tempEmployeetalentInfo.probationPercentage);
        this.formGroup.controls['realSalary'].patchValue((this.tempEmployeetalentInfo.salary * this.tempEmployeetalentInfo.probationPercentage) / 100);
        this.formGroup.controls['sex'].setValue(this.tempEmployeetalentInfo.sex);
        this.formGroup.controls['birthday'].setValue(this.tempEmployeetalentInfo.dateOfBirth);
        this.formGroup.controls['skills'].setValue(this.skillList.filter(x =>this.tempEmployeetalentInfo.skillStr.toLowerCase().includes(x.key.toLowerCase())));
        this.formGroup.controls['contractStartDate'].setValue(this.tempEmployeetalentInfo.onboardDate);
        this.formGroup.controls['personalEmail'].setValue(this.tempEmployeetalentInfo.email);
      })
    )}
  }

  isShowRecreateBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Employee_EmployeeDetail_TabPersonalInfo_SyncToOtherTool_ReCreateUserToOtherTool);
  }
  isShowEditBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Employee_EmployeeDetail_TabPersonalInfo_Edit);
  }
  isShowUploadAvatarBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Employee_EmployeeDetail_TabPersonalInfo_UploadAvatar);
  }
  isShowChangeBranchBtn(){
    return this.userId && !this.isUpdateRequestMode && !this.isEdit &&  this.isGranted(PERMISSIONS_CONSTANT.Employee_EmployeeDetail_TabPersonalInfo_ChangeBranch);
  }
  isShowChangeStatusBtn(){
    return this.userId && !this.isUpdateRequestMode && !this.isEdit &&  this.isGranted(PERMISSIONS_CONSTANT.Employee_EmployeeDetail_TabPersonalInfo_ChangeWorkingStatus);
  }
  isShowBackToWorkBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Employee_EmployeeDetail_TabPersonalInfo_ChangeWorkingStatus_BackToWork);
  }
  isShowExtendMaternityLeaveBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Employee_EmployeeDetail_TabPersonalInfo_ChangeWorkingStatus_ExtendMaternityLeave);
  }
  isShowExtendPausingBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Employee_EmployeeDetail_TabPersonalInfo_ChangeWorkingStatus_ExtendPausing);
  }
  isShowMaternityLeaveBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Employee_EmployeeDetail_TabPersonalInfo_ChangeWorkingStatus_MaternityLeave);
  }
  isShowPauseBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Employee_EmployeeDetail_TabPersonalInfo_ChangeWorkingStatus_Pause);
  }
  isShowQuitBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Employee_EmployeeDetail_TabPersonalInfo_ChangeWorkingStatus_Quit);
  }
  isAllowViewTabPersonalInfo(){
    return this.isGranted(PERMISSIONS_CONSTANT.Employee_EmployeeDetail_TabPersonalInfo_View);
  }
  isShowApproveBtn(){
    return this.isUpdateRequestMode && this.requestUpdateInfoDetail.requestStatus != APP_ENUMS.RequestUpdateInfoStatus.Approved &&
    this.isGranted(PERMISSIONS_CONSTANT.WarningEmployee_RequestChangeInfo_DetailRequest_Approve);
  }
  isShowRejectBtn(){
    return this.isUpdateRequestMode && this.requestUpdateInfoDetail.requestStatus == APP_ENUMS.RequestUpdateInfoStatus.Pending && this.isGranted(PERMISSIONS_CONSTANT.WarningEmployee_RequestChangeInfo_DetailRequest_Reject);
  }
  isShowSyncDataToOtherTool(){
    return this.userId && !this.isUpdateRequestMode && !this.isEdit && this.isGranted(PERMISSIONS_CONSTANT.Employee_EmployeeDetail_TabPersonalInfo_SyncToOtherTool)
  }
  isShowQuitJobBtn(){
    return this.personalInfo.status == APP_ENUMS.UserStatus.Quit && this.isGranted(PERMISSIONS_CONSTANT.Employee_EmployeeDetail_TabPersonalInfo_SyncToOtherTool_QuitJobUserToOtherTool);
  }
  isShowEditToOtherToolBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Employee_EmployeeDetail_TabPersonalInfo_SyncToOtherTool_EditUserToOtherTool)
  }

  protected list(
    request: PagedRequestDto,
    pageNumber: number,
    finishedCallback: Function
  ): void { }

  public getAllIssuedBy(){
    this.subscription.push(
      this.issuedByService.getAll().subscribe((rs)=>{
        this.listIssuedBys = rs.result.map((x) => ({ key: x.name, value: x.id }));
      })
    )
  }

  public getAllBranch() {
    this.subscription.push(
      this.branchService.getAll().subscribe((rs) => {
        this.branchList = rs.result.map((x) => ({ key: x.name, value: x.id }));
      })
    );
  }

  public getAllTeam() {
    this.subscription.push(
      this.teamService.getAll().subscribe((rs) => {
        this.teamList = rs.result.map((x) => ({ key: x.name, value: x.id }));
      })
    );
  }

  getRequestDetailById(id: number){
    this.subscription.push(
      this.warningEmployeeService.getRequestDetailById(id).subscribe((rs)=>{
        this.requestUpdateInfoDetail = rs.result;
        this.setValueToUpdateProfile(this.requestUpdateInfoDetail);
      })
    )
  }

  public getAllJobPosition() {
    this.subscription.push(
      this.jobPositionService.getAll().subscribe((rs) => {
        this.jobPositionList = rs.result.map((x) => ({
          key: x.name,
          value: x.id,
        }));
      })
    );
  }

  public getAllSkill() {
    this.subscription.push(
      this.skilsSerivce
        .getAll()
        .subscribe((rs) => {
          this.skillList = rs.result.map((x) => ({
            key: x.name,
            value: x.id,
          }));
        })
    );
  }

  public getAllLevel() {
    this.subscription.push(
      this.levelService.getAll().subscribe((rs) => {
        this.levelList = rs.result.map((x) => ({ key: x.name, value: x.id }));
      })
    );
  }

  public getAllBank() {
    this.subscription.push(
      this.bankService
        .getAll()
        .subscribe((rs) => {
          this.bankList = rs.result.map((x) => ({
            key: x.name,
            value: x.id,
          }));
        })
    );
  }

  public getAllUserType() {
    this.subscription.push(
      this.userTypeService.getAll().subscribe(rs => {
        this.userTypeList = rs.result.map(x => ({
          key: x.name,
          value: x.id
        }))
      })
    )
  }

  getAvatar(member) {
    if (member.avatarFullPath) {
      return member.avatarFullPath;
    }
    if (member.sex == this.APP_ENUM.Gender.Female) {
      return "assets/img/women.png";
    }
    return "assets/img/men.png";
  }

  getEmployeeById(id: number) {
    if(this.isAllowViewTabPersonalInfo()){
      this.isLoading = true;
      this.subscription.push(
      this.employeeService
        .get(id)
        .subscribe((rs) => {

          this.personalInfo = rs.result.employeeInfo;
          if(!this.requestId){
            this.setFormValue(rs.result.employeeInfo);
          }
          this.isAllowEdit = rs.result.isAllowEdit;
          this.isAllowBranchStatus = rs.result.isAllowEditBranch;
          this.isAllowWorkingStatus = rs.result.isAllowEditWorkingStatus;
          this.isLoading = false;
        },()=> this.isLoading = false)
    );
    }
  }

  initForm() {
    this.formGroup = this.formBuilder.group({
      id: 0,
      surname: ["", [Validators.required]],
      name: ["", [Validators.required]],
      email: ["", [Validators.required, Validators.email]],
      phone: [""],
      userType: ["", [Validators.required]],
      branchId: [null, Validators.required],
      jobPositionId: [null, [Validators.required]],
      levelId: [null, [Validators.required]],
      teams: [],
      status: [this.APP_ENUM.UserStatus.Working, [Validators.required]],
      skills: [],
      salary: [0, [Validators.required]],
      realSalary: [0, [Validators.required]],
      taxCode: "",
      probationPercentage: [100, [Validators.required]],
      bankId: "",
      bankAccountNumber: "",
      idCard: "",
      issuedOn: "",
      issuedBy: "",
      insuranceStatus: this.APP_ENUM.InsuranceStatus.NONE,
      birthday: null,
      sex: this.APP_ENUM.Gender.Male,
      placeOfOrigin: "",
      placeOfResidence: "",
      contractStartDate: [this.formatDateYMD(new Date), [Validators.required]],
      contractEndDate: "",
      remainLeaveDay: 0,
      contractCode: "",
      startWorkingDate: [this.formatDateYMD(new Date), [Validators.required]],
      personalEmail: ["", [Validators.email]],
    });
    if(this.isAllowEdit){
      this.bindSalaryChangeEvent(this.formGroup);
      this.bindProbationPercentageChangeEvent(this.formGroup);
      this.bindUserTypeChangeEvent(this.formGroup)
    }
  }

  toggleEdit() {
    this.isEdit = !this.isEdit;
    this.setFormValue(this.personalInfo)
    if (this.isEdit) {
      this.formGroup.enable();
      this.disableControls(this.personalInfo);
    } else {
      this.formGroup.disable();
    }
  }

  disableControls(personalInfo: PersonalInfo){
    if(personalInfo.userType == APP_ENUMS.UserType.ProbationaryStaff) {
      this.formGroup.controls.realSalary.disable();
    }
  }

  bindSalaryChangeEvent(formGroup:FormGroup){
    this.subscription.push(
      formGroup.controls.salary.valueChanges.subscribe(rs => {
      if(formGroup.controls.probationPercentage.value && rs) {
          let realSalary = Math.round(this.formGroup.controls.salary.value * (formGroup.controls.probationPercentage.value / 100))
          formGroup.controls.realSalary.setValue(realSalary)
        } else {
          formGroup.controls.realSalary.setValue(rs)
        }
      })
    )
  }

  bindProbationPercentageChangeEvent(formGroup: FormGroup){
    this.subscription.push(
      this.formGroup.controls.probationPercentage.valueChanges.subscribe(rs => {
        if (this.formGroup.controls.salary.value && rs) {
          let realSalary = Math.round(formGroup.controls.salary.value * (rs / 100))
          this.formGroup.controls.realSalary.setValue(realSalary)
        } else {
          if (!rs) {
            this.formGroup.controls.realSalary.setValue(this.formGroup.controls.salary.value)
          }
        }
      })
    )
  }

  bindUserTypeChangeEvent(formGroup: FormGroup) {
    this.subscription.push(
      formGroup.controls.userType.valueChanges.subscribe(rs => {
        if(rs == APP_ENUMS.UserType.ProbationaryStaff) {
          formGroup.controls.realSalary.disable();
          formGroup.controls.realSalary.setValue(0);
          if(!this.userId) {
            formGroup.controls.probationPercentage.setValue(85);
            return;
          }
        }
        if(rs != APP_ENUMS.UserType.ProbationaryStaff) {
          formGroup.controls.realSalary.setValue(0);
          formGroup.controls.realSalary.enable();
          return;
        }
      })
    )
  }
  setFormValue(info: PersonalInfo) {
    const { surname, name } = this.splitFullName(this.personalInfo.fullName);
    const selectedTeam = this.teamList.filter(team => this.personalInfo.teams.find(x => x.teamId == team.value))
    const selectedSkill = this.skillList.filter(skill => this.personalInfo.skills.find(x => x.skillId == skill.value))
    this.formGroup.patchValue({
      surname: surname,
      name: name,
      email: info.email,
      phone: info.phone,
      userType: info.userType,
      branchId: info.branchId,
      jobPositionId: info.jobPositionId,
      teams: selectedTeam,
      levelId: info.levelId,
      status: info.status,
      skills: selectedSkill,
      salary: info.salary,
      realSalary: info.realSalary,
      bankId: info.bankId,
      bankAccountNumber: info.bankAccountNumber,
      taxCode: info.taxCode,
      probationPercentage: info.probationPercentage,
      idCard: info.idCard,
      issuedOn: info.issuedOn,
      issuedBy: info.issuedBy,
      insuranceStatus: info.insuranceStatus,
      birthday: info.birthday,
      sex: info.sex,
      placeOfOrigin: info.placeOfPermanent,
      placeOfResidence: info.address,
      contractStartDate: info.contractStartDate,
      contractEndDate: info.contractEndDate,
      remainLeaveDay: info.remainLeaveDay,
      contractCode: info.contractCode,
      startWorkingDate: info.startWorkingDate,
      personalEmail: info.personalEmail
    });
  }

  setValueToUpdateProfile(newInfo){
    this.formGroup.patchValue({
      id: newInfo.id,
      phone : newInfo.phone,
      birthday : newInfo.birthday,
      bankId: newInfo.bankId,
      bankAccountNumber: newInfo.bankAccountNumber,
      idCard: newInfo.idCard,
      issuedOn: newInfo.issuedOn,
      issuedBy: newInfo.issuedBy,
      placeOfOrigin: newInfo.placeOfPermanent,
      placeOfResidence: newInfo.address,
      taxCode : newInfo.taxCode
    });
  }

  splitFullName(fullName: string) {
    const splited = fullName.split(" ");
    if (splited.length > 2) {
      let name = splited.splice(splited.length - 1, 1).join("");
      let surname = splited.join(" ").trim();
      return { surname, name };
    } else {
      return { surname: splited[0], name: splited[1] };
    }
  }

  onEdit() {
    this.toggleEdit();
  }

  onCancel() {
    if (this.isEdit) {
      if (this.userId) {
        this.toggleEdit();
      } else if(this.tempEmployeeId){
        this.router.navigate(["app", "warning-for-employee", "temp-employee-talent"]);
      }
      else {
        this.router.navigate(["app", "employees", "list-employee"]);
      }
    } else if(this.isUpdateRequestMode){
      this.router.navigate(["app", "warning-for-employee", "request-change-info"]);
    }
    else {
      this.router.navigate(["app", "employees", "list-employee"]);
    }
  }

  onSave() {
    var inputStartWorkingDate = "";
    if(this.userId && (this.formGroup.value.userType == APP_ENUMS.UserType.Staff)){
      inputStartWorkingDate = this.formatDateYMD(this.formGroup.value.startWorkingDate)
    }
    if(this.userId && (this.formGroup.value.userType != APP_ENUMS.UserType.Staff)){
      inputStartWorkingDate = this.personalInfo.startWorkingDate;

    }
    if(!this.userId){
      inputStartWorkingDate = this.formatDateYMD(this.formGroup.value.contractStartDate);
    }

    const employee: CreateUpdateEmployeeDto = {
      id: this.userId,
      address: this.formGroup.value.placeOfResidence,
      fullName: this.formGroup.value.surname.trim() + " " + this.formGroup.value.name.trim(),
      birthday: this.formGroup.controls.birthday.value ? this.formatDateYMD(this.formGroup.controls.birthday.value) : "",
      userType: this.formGroup.value.userType,
      jobPositionId: this.formGroup.value.jobPositionId,
      levelId: this.formGroup.value.levelId,
      branchId: this.formGroup.value.branchId,
      idCard: this.formGroup.value.idCard,
      issuedOn: this.formGroup.controls.issuedOn.value ? this.formatDateYMD(this.formGroup.controls.issuedOn.value) : "",
      issuedBy: this.formGroup.value.issuedBy,
      phone: this.formGroup.value.phone,
      bankAccountNumber: this.formGroup.value.bankAccountNumber,
      remainLeaveDay: this.formGroup.value.remainLeaveDay,
      email: this.formGroup.value.email,
      probationPercentage: this.formGroup.value.userType == APP_ENUMS.UserType.ProbationaryStaff ? this.formGroup.value.probationPercentage : 100,
      salary: this.formGroup.value.userType == APP_ENUMS.UserType.ProbationaryStaff ? this.formGroup.value.salary: this.formGroup.value.realSalary,
      realSalary: this.isAllowEdit ? this.formGroup.controls.realSalary.value : this.personalInfo.realSalary,
      status: this.formGroup.value.status,
      avatar: this.personalInfo.avatar || "",
      insuranceStatus: this.formGroup.value.insuranceStatus ? this.formGroup.value.insuranceStatus : this.APP_ENUM.InsuranceStatus.NONE,
      placeOfPermanent: this.formGroup.value.placeOfOrigin,
      startWorkingDate: inputStartWorkingDate,
      taxCode: this.formGroup.value.taxCode,
      teams: this.formGroup.value.teams?.map(team => team.value) || [],
      skills: this.formGroup.value.skills?.map(skill => skill.value) || [],
      sex: this.formGroup.value.sex,
      bankId: this.formGroup.value.bankId,
      contractStartDate: this.formatDateYMD(this.formGroup.value.contractStartDate),
      contractEndDate: this.formGroup.controls.contractEndDate.value ? this.formatDateYMD(new Date(this.formGroup.controls.contractEndDate.value)) : "",
      contractCode: this.formGroup.controls.contractCode.value || this.personalInfo.contractCode,
      personalEmail: this.formGroup.value.personalEmail
    };
    this.trimData(employee)
    if (this.userId) {
      this.update(employee)
    } else {
      this.create(employee);
    }
  }

  openUploadAvatar() {
    this.dialog
      .open(UploadAvatarComponent, {
        width: "600px",
      })
      .afterClosed()
      .subscribe((data) => {
        if (data) {
          this.subscription.push(
            this.employeeService
              .uploadAvatar(data, this.userId.toString())
              .subscribe((rs) => {
                this.notify.success("Avatar uploaded");
                this.getEmployeeById(this.userId);
                this.onUpdate.emit(true);
              })
          );
        }
      });
  }


  public updateEmployeeToOtherTool(){
    var input = {
      id: this.userId
     } as InputToSyncToOtherToolDto;
    this.isLoading = true;
    this.employeeService
      .updateEmployeeToOtherTool(input)
      .subscribe((rs) => {
        this.notify.success("Employee Updated to other tool successful");
        this.getEmployeeById(this.userId)
        this.isLoading = false;
      },() => this.isLoading = false);
  }

  update(employee: CreateUpdateEmployeeDto) {
    this.isLoading = true;
    this.employeeService
      .updateEmployee(employee)
      .subscribe((rs) => {
        this.notify.success("Employee Updated");
        this.getEmployeeById(this.userId)
        this.isLoading = false;
        this.isEdit = false;
      },() => this.isLoading = false);
  }

  create(employee: CreateUpdateEmployeeDto) {
    this.isLoading = true;
    this.employeeService.createEmployee(employee, this.tempEmployeeId).subscribe((rs) => {
      this.notify.success("Employee Created");
      this.isLoading = false;
      this.router.navigate(
        ["app", "employees", "list-employee", "employee-detail", "personal-info"],
        { queryParams: { id: rs.result.id } }
      );
    },() => this.isLoading = false);
  }

  get employeeTeamList() {
    return this.personalInfo.teams?.map(x => x.teamName)
  }

  get employeeSkillList() {
    return this.personalInfo.skills?.map(x => x.skillName)
  }

  get insuranceStatusString() {
    return Object.entries(this.APP_ENUM.InsuranceStatus).find(x => x[1] == this.personalInfo.insuranceStatus)[0]
  }

  public onChangeStatus() {
    this.isShowSubButtonChangeStatus = true;
  }

  public onChangeStatusToQuit() {
    const dialog = this.dialog.open(ChangeStatusToQuitComponent, {
      data: {
        personalInfo: this.personalInfo
      },
      minWidth: '75%',
      maxHeight: '99vh',
      panelClass: 'change-working-status'
    })
    dialog.afterClosed().subscribe((rs) => {
      if (rs) {
        this.getEmployeeById(this.userId);
      }
    })
  }

  public onChangeStatusToPause() {
    const dialog = this.dialog.open(ChangeStatusToPauseComponent, {
      data: {
        personalInfo: this.personalInfo
      },
      minWidth: '75%',
      maxHeight: '99vh',
      panelClass: 'change-working-status'
    })
    dialog.afterClosed().subscribe((rs) => {
      if (rs) {
        this.getEmployeeById(this.userId);
      }
    })
  }

  public onChangeStatusToMaternityLeave() {
    const dialog = this.dialog.open(ChangeStatusToMaternityLeaveComponent, {
      data: {
        personalInfo: this.personalInfo
      },
      minWidth: '75%',
      maxHeight: '99vh',
      panelClass: 'change-working-status'
    })
    dialog.afterClosed().subscribe((rs) => {
      if (rs) {
        this.getEmployeeById(this.userId);
      }
    })
  }

  public onChangeStatusToWorking() {
    const dialog = this.dialog.open(ChangeStatusToWorkingComponent, {
      data: {
        personalInfo: this.personalInfo
      },
      minWidth: '75%',
      maxHeight: '99vh',
      panelClass: 'change-working-status'
    })
    dialog.afterClosed().subscribe((rs) => {
      if (rs) {
        this.getEmployeeById(this.userId);
      }
    })
  }

  public onExtendMaterityLeave() {
    const dialog = this.dialog.open(ExtendMaternityLeaveComponent, {
      data: {
        personalInfo: this.personalInfo
      },
      minWidth: '75%',
      maxHeight: '99vh',
      panelClass: 'change-working-status'
    })
    dialog.afterClosed().subscribe((rs) => {
      if (rs) {
        this.getEmployeeById(this.userId);
      }
    })
  }

  public onExtendPausing() {
    const dialog = this.dialog.open(ExtendPausingComponent, {
      data: {
        personalInfo: this.personalInfo
      },
      minWidth: '75%',
      maxHeight: '99vh',
      panelClass: 'change-working-status'
    })
    dialog.afterClosed().subscribe((rs) => {
      if (rs) {
        this.getEmployeeById(this.userId);
      }
    })
  }
  public calculateBasicSalary(realSalary: number, probationPercentage) {
    if (probationPercentage == 0 || probationPercentage == 100) return realSalary
    let basicSalary = Math.round(realSalary / (probationPercentage / 100))
    if (basicSalary != this.personalInfo.salary) return this.personalInfo.salary
    return basicSalary;
  }

  public onChangeBranch() {
    const diaglog = this.dialog.open(ChangeBranchComponent, {
      data: {
        personalInfo: this.personalInfo
      },
      width: '800px'
    })
    diaglog.afterClosed().subscribe((rs) => {
      if (rs) {
        this.getEmployeeById(this.userId);
      }
    })
  }

  public get isStaff(){
    return this.formGroup.controls?.userType.value == APP_ENUMS.UserType.Staff
  }

  public get isProbationaryStaff(){
    return this.formGroup.controls?.userType.value == APP_ENUMS.UserType.ProbationaryStaff
  }

  public reCreateUserToOtherTool(){
    this.isLoading = true;
    this.subscription.push(
      this.employeeService.ReCreateEmployeeToOtherTool(this.userId).subscribe((rs)=>{
          abp.notify.success("Re-create employee successfull");
          this.isLoading = false;
      }, ()=> this.isLoading = false)
    );
  }

  public toQuitJob(){
    var input = {
     id: this.userId
    } as InputToSyncToOtherToolDto;
    this.isLoading = true;
    this.subscription.push(
      this.employeeService.quitJobToOtherTool(input).subscribe((rs)=>{
        abp.notify.success("Quit job to other tool successful");
        this.isLoading = false;
      },()=>this.isLoading = false)
    )
  }

  public onApprove(){
    const input: UpdateRequestDetailDto = {
      id: this.formGroup.value.id,
      employeeId: this.userId,
      address: this.formGroup.value.placeOfResidence,
      birthday: this.formGroup.controls.birthday.value ? this.formatDateYMD(this.formGroup.controls.birthday.value) : "",
      idCard: this.formGroup.value.idCard,
      issuedOn: this.formGroup.controls.issuedOn.value ? this.formatDateYMD(this.formGroup.controls.issuedOn.value) : "",
      issuedBy: this.formGroup.value.issuedBy,
      phone: this.formGroup.value.phone,
      bankAccountNumber: this.formGroup.value.bankAccountNumber,
      placeOfPermanent: this.formGroup.value.placeOfOrigin,
      taxCode: this.formGroup.value.taxCode,
      bankId: this.formGroup.value.bankId,
    };
    abp.message.confirm("Are you sure to approve request change employee?", "",
    (rs)=>{
      if(rs){
        this.isLoading = true;
        this.warningEmployeeService.approveRequestUpdateInfo(input).subscribe((rs)=>{
          abp.notify.success("Approved request successful");
          this.getRequestDetailById(this.requestId);
          this.isLoading = false;
          this.router.navigate(
            ["app", "employees", "list-employee", "employee-detail", "personal-info"],
            { queryParams: { id: this.userId } }
          ).then();
          this.isUpdateRequestMode = false;
          this.getEmployeeById(this.userId)
        },()=> this.isLoading = false)
      }
    })
  }

  public onReject(){
    this.inputToUpdate.id = this.requestId;
    abp.message.confirm("Are you sure to reject request change employee?", "",
    (rs)=>{
      if(rs){
        this.isLoading = true;
        this.warningEmployeeService.rejectRequestUpdateInfo(this.inputToUpdate).subscribe((rs)=>{
          abp.notify.success("Rejected request successful");
          this.isLoading = false;
          this.getRequestDetailById(this.requestId);
          this.getEmployeeById(this.userId)
        },()=> this.isLoading = false)
      }
    })
  }
  public onPickIssuedBy(item: string){
    this.formGroup.controls.issuedBy.setValue(item);
  }
}
interface PersonalInfo extends BaseEmployeeDto {
  surname: string;
  name: string;
  email: string;
  phone: string;
  branchId: number;
  skills: PersonalSkillDto[];
  salary: number;
  realSalary: number;
  idCard: string;
  issuedOn: string;
  issuedBy: string;
  insuranceStatus: number;
  birthday: string;
  placeOfPermanent: string;
  placeOfResidence: string;
  contractStartDate: string;
  contractEndDate: string;
  address: string;
  startWorkingDate: string;
  taxCode: string;
  bankId: number;
  bankAccountNumber: string;
  bank?: string;
  probationPercentage: number;
  levelId: number;
  contractCode: string;
  remainLeaveDay: number;
  personalEmail:string;
}

interface PersonalSkillDto {
  skillId: number,
  skillName: string,
}

interface InputToSyncToOtherToolDto{
  id: number
}
