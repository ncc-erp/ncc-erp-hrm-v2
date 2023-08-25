import { FilterDto } from '@shared/paged-listing-component-base';
import { finalize, catchError } from 'rxjs/operators';
import { ActivatedRoute } from '@angular/router';
import { BonusService } from '@app/service/api/bonuses/bonus.service';
import { BonusEmployeeDto, BonusDto, AddBonusEmployeeDto, EditBonusEmployeeDto, EmployeeBonusDetailDto } from '@app/service/model/bonuses/bonus.dto';
import { EmployeeService } from '@app/service/api/employee/employee.service';
import { PagedListingComponentBase, PagedRequestDto } from '@shared/paged-listing-component-base';
import { Component, OnInit, Injector } from '@angular/core';
import { GetEmployeeDto } from '@app/service/model/employee/employee.dto';
import { AddEmployeeBonusDialogComponent } from './add-employee-bonus-dialog/add-employee-bonus-dialog.component';
import { APP_ENUMS } from '@shared/AppEnums';
import { AppConsts } from '@shared/AppConsts';
import { BranchService } from '@app/service/api/categories/branch.service';
import { LevelService } from '@app/service/api/categories/level.service';
import { UserTypeService } from '@app/service/api/categories/userType.service';
import { TeamService } from '@app/service/api/categories/team.service';
import { DefaulEmployeeFilterDto } from '@shared/components/employee/employee-filter/employee-filter.component';
import { JobPositionService } from '@app/service/api/categories/jobPosition.service';
import { ImportEmployeeToBonusComponent } from './import-employee-to-bonus/import-employee-to-bonus.component';
import { GetInputFilterDto } from '@app/service/model/employee/GetEmployeeExcept.dto';
import { PERMISSIONS_CONSTANT } from '@app/permission/permission';
import { startWithTap } from '@shared/helpers/observerHelper';
import { MailDialogComponent } from '@app/modules/admin/email-templates/mail-dialog/mail-dialog.component';
import { SendBonusMailToOneEmployeeDto } from '@app/service/model/mail/sendMail.dto';

@Component({
  selector: 'app-bonus-employee',
  templateUrl: './bonus-employee.component.html',
  styleUrls: ['./bonus-employee.component.css']
})
export class BonusEmployeeComponent extends PagedListingComponentBase<BonusEmployeeDto> implements OnInit {
  protected list(request: PagedRequestDto, pageNumber: number, finishedCallback: Function): void {
    if(this.isAllowRoutingTabEmployee()){
      let input = {
        statusIds: this.statusIds,
        levelIds: this.levelIds,
        branchIds: this.branchIds,
        userTypes: this.userTypes,
        teamIds: this.teamIds,
        isAndCondition : this.isAndCondition,
        gridParam: request,
        jobPositionIds: this.jobPositionIds,
      } as GetInputFilterDto;
      this.inputToFilter = input;
      this.subscription.push(
        this.bonusService.getAllBonusEmployee(this.bonusId, input)
          .pipe(finalize(() => {
            finishedCallback();
          }))
          .subscribe(rs => {
            this.bonusEmployeeList = rs.result.items;
            this.showPaging(rs.result, pageNumber)
          })
      );
      this.isAddingEmployee = false;
      this.isEditingEmployee = false;
    }
  }


  constructor(injector: Injector,
    private employeeService: EmployeeService,
    private bonusService: BonusService,
    private branchService: BranchService,
    private levelService: LevelService,
    private teamService: TeamService,
    private userTyService: UserTypeService,
    private positionService: JobPositionService,
    private route: ActivatedRoute) {
    super(injector)
  }

  private bonusId: any;
  public bonus = {} as BonusDto;
  public employeeList: GetEmployeeDto[] = [];
  public bonusEmployeeList: BonusEmployeeDto[] = [];
  public isEditingEmployee: boolean = false;
  public isAddingEmployee: boolean = false;
  public searchEmployee: string = "";
  public isFocusing:boolean = false;
  public inputToFilter = {} as GetInputFilterDto;


  public userTypeList = []
  public userLevelList = []
  public branchList = []
  public positionList = []
  public teamList = []
  public filterTypeEnum = APP_ENUMS.FilterTypeEnum
  public statusList = []
  public genderList = []
  public defaultValue = {} as DefaulEmployeeFilterDto
  public moneyList = []

  public defaultMoneyTo;
  public defaultMoneyFrom;
  public filterMultipleTypeParamEnum = APP_ENUMS.FilterMultipleTypeParamEnum;
  ngOnInit(): void {
    this.bonusId = this.route.snapshot.queryParamMap.get('id');


    this.genderList = this.getListFormEnum(APP_ENUMS.Gender)
    this.statusList = this.getListFormEnum(APP_ENUMS.UserStatus, true)
  
    this.getAllUserType()
    this.getAllLevel()
    this.getAllBranch()
    this.getAllTeam()
    this.getAllJobPositon()
    this.setDefaultValue()



    this.getBonusById();
    this.getAllUser();
    this.refresh();
  }


  // onTableFilter(filterValue: any, property: string) {
  //   let filterItem = {
  //     comparision: 0,
  //     propertyName: property,
  //     value: filterValue
  //   }
  //   this.onFilter(filterItem)
  // }
  // onSearchFilter(searchValue: string) {
  //   this.onSearch(searchValue)
  // }
  
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

  private getAllJobPositon() {
    this.subscription.push(this.positionService.getAll().subscribe(rs => {
      this.positionList = this.mapToFilter(rs.result, true)
    }))
  }

  private getAllTeam() {
    this.subscription.push(this.teamService.getAll().subscribe(rs => {
      this.teamList = this.mapToFilter(rs.result, true)
    }))
  }

  public setDefaultValue() {
    this.defaultValue = {
      branch: AppConsts.DEFAULT_ALL_FILTER_VALUE,
      userLevel: AppConsts.DEFAULT_ALL_FILTER_VALUE,
      status: APP_ENUMS.UserStatus.Working,
      jobPosition: AppConsts.DEFAULT_ALL_FILTER_VALUE,
      team: AppConsts.DEFAULT_ALL_FILTER_VALUE,
      gender: AppConsts.DEFAULT_ALL_FILTER_VALUE,
      userType: AppConsts.DEFAULT_ALL_FILTER_VALUE
    } as DefaulEmployeeFilterDto
  }



  public getAllUser() {
    this.subscription.push(
      this.bonusService.GetAllEmployeeNotInBonus(this.bonusId)
      .pipe(startWithTap(() => {
        this.isLoading = true;
      }))
      .pipe(finalize(() => {
        this.isLoading = false;
      }))
      .subscribe((res) => {
        this.employeeList = res.result;
      })
    )
  }

  public getBonusById() {
    this.bonusService.getBonusDetail(Number(this.bonusId)).subscribe((res) => {
      this.bonus = res.result;
    })
  }

  public onUpdate(employee: BonusEmployeeDto) {
    employee.createMode = true;
    this.isEditingEmployee = true;
    this.isFocusing = true;
  }

  public focusOut(){
    this.isFocusing= false;
  }

  public onSave(employee: BonusEmployeeDto) {
    employee.bonusId = this.bonusId;
    if (this.isEditingEmployee) {
      let editEmployee = {} as EditBonusEmployeeDto;
      editEmployee = {
        id: employee.id,
        bonusId: employee.bonusId,
        money: employee.money,
        note: employee.note
      }
      this.subscription.push(
        this.bonusService.updateEmployeeInBonus(editEmployee)
        .pipe(startWithTap(() => {
          this.isLoading = true
        }))
        .pipe(finalize(() => {
          this.isLoading = false;
          this.isAddingEmployee = false;
          this.isEditingEmployee = false;
          this.refresh();
        }))
        .subscribe((res) => {
          abp.notify.success("Update employee in bonus successful");
          employee.createMode = false;
        })
      )
    } else {
      let addEmployee = {} as AddBonusEmployeeDto;
      addEmployee = {
        id: 0,
        bonusId: employee.bonusId,
        money: employee.money,
        note: employee.note,
        employeeIds: [employee.employeeId]
      }
      this.subscription.push(
        this.bonusService.quickAddEmployeeToBonus(addEmployee)
      .pipe(startWithTap(() => {
        this.isLoading = true
      }))
      .pipe(finalize(() => {
        this.isLoading = false;
        this.isAddingEmployee = false;
        this.isEditingEmployee = false;
        this.getAllUser()
        this.refresh();
      }))
      .subscribe(res => {
        abp.notify.success("Add employee to bonus successful");
        employee.createMode = false;
      })
      )
    }


  }

  public onCancel(employee: BonusEmployeeDto) {
    this.refresh();
  }

  public onDelete(employee: BonusEmployeeDto) {
    this.confirmDelete(`Delete employee <strong>${employee.fullName}</strong>`,
      () => {
        this.isLoading = true;
        this.bonusService.deleteEmployeeFromBonus(employee.id, employee.bonusId).pipe(finalize(() => {
          this.isLoading = false;
          this.getAllUser();
        })).subscribe(rs => {
          abp.notify.success(`Deleted bonus ${employee.fullName}`);
          this.refresh()
        })
      }
  )}
  public onFilter(filterItem: FilterDto): void {
    let existFilterItem = this.filterItems.find(x => x.propertyName === filterItem.propertyName)
    if (existFilterItem) {
      existFilterItem.value = filterItem.value
    }
    else {
      this.filterItems.push(filterItem)
    }
    this.refresh()
  }

  onMultipleAddEmployee() {
    const dialog = this.dialog.open(AddEmployeeBonusDialogComponent, {
      data: {...this.bonus},
      width: '700px'
    })
    dialog.afterClosed().subscribe(rs => {
      if(rs) {
        this.refresh();
        this.getAllUser();
      }
    })
  }

  public onAddEmployee() {
    let employee = {} as BonusEmployeeDto;
    employee.createMode = true;
    employee.note = this.bonus.name;
    this.isAddingEmployee = true;
    this.bonusEmployeeList.unshift(employee);
  }

  public onImportFile(){
    const dialog = this.dialog.open(ImportEmployeeToBonusComponent,{
      data:{
        id: this.bonusId
      }
    })
    dialog.afterClosed().subscribe((rs)=>{
      if(rs){
        this.refresh();
      }
    })
  }

  public onSendAllMail(){
    this.inputToFilter.gridParam.maxResultCount = 500000;
    abp.message.confirm("Are you sure to send mail to all?","", ((rs)=>{
      if(rs){
        this.isLoading = true;
        this.subscription.push(
          this.bonusService.sendAllMail(this.bonusId, this.inputToFilter).subscribe((rs)=>{
            abp.message.success(rs.result);
            this.isLoading = false;
          },()=> this.isLoading = false)
        )
      }
    }))
  }

  public onSendMail(id: number){
    this.subscription.push(
      this.bonusService.getBonusTemplate(id).subscribe((rs)=>{
        const dialogData = {
          showEditButton: true,
          mailInfo: rs.result,
          showDialogHeader: false,
          showSendMailButton: true,
          showSendMailHeader: true
        }
        const ref = this.dialog.open(MailDialogComponent,
          {
           data: dialogData,
           width: '1600px',
           panelClass: 'email-dialog',
          })
         ref.afterClosed().subscribe((rs)=>{
           if(rs){
             var input: SendBonusMailToOneEmployeeDto = {
               bonusEmployeeId: id,
               mailContent: rs
             }
             this.subscription.push(
               this.bonusService.sendMail(input).subscribe((rs)=>{
                abp.message.success(`Mail sent to ${dialogData.mailInfo.sendToEmail}!`)
               })
             )
           }
         })
      })
    )
  }
  isShowAddBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Bonus_BonusDetail_TabEmployee_Add);
  }
  isShowQuickAddBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Bonus_BonusDetail_TabEmployee_QuickAdd);
  }
  isShowEditBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Bonus_BonusDetail_TabEmployee_Edit);
  }
  isShowDeleteBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Bonus_BonusDetail_TabEmployee_Delete);
  }
  isShowImportBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Bonus_BonusDetail_TabEmployee_Import);
  }
  isShowDownloadTemplateBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Bonus_BonusDetail_TabEmployee_DownloadTemplate);
  }
  isAllowViewTabBonus(){
    return this.isGranted(PERMISSIONS_CONSTANT.Employee_EmployeeDetail_TabBonus_View);
  } 
  isAllowRoutingTabEmployee(){
    return this.isGranted(PERMISSIONS_CONSTANT.Bonus_BonusDetail_TabEmployee_View)
  }
  isAllowViewTabPersonalInfo(){
    return this.isGranted(PERMISSIONS_CONSTANT.Employee_EmployeeDetail_TabPersonalInfo_View);
  }
  isShowSendMailBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Bonus_BonusDetail_TabEmployee_SendMail);
  }
  isShowSendAllMailBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Bonus_BonusDetail_TabEmployee_SendAllMail)
  }
}