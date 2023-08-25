import { Component, OnInit, Injector } from '@angular/core';
import { LevelService } from '@app/service/api/categories/level.service';
import { UserTypeService } from '@app/service/api/categories/userType.service';
import { SalaryChangeRequestService } from '@app/service/api/salary-change-request/salary-change-request.service';
import { LevelDto } from '@app/service/model/categories/level.dto';
import { EmployeeSalaryHistoryDto, UpdateSalaryHistoryDto } from '@app/service/model/history/EmployeeSalaryHistory.dto';
import { DialogComponentBase } from '@shared/dialog-component-base';
import { UpdateRequestEmployeeInfoDto } from '@app/service/model/history/UpdateRequestEmployeeInfoDto'
@Component({
  selector: 'app-salary-changes-edit-dialog',
  templateUrl: './salary-changes-edit-dialog.component.html',
  styleUrls: ['./salary-changes-edit-dialog.component.css']
})
export class SalaryChangesEditDialogComponent extends DialogComponentBase<EmployeeSalaryHistoryDto> implements OnInit {
  public updateSalaryHistory: UpdateSalaryHistoryDto = {} as UpdateSalaryHistoryDto
  public userTypeList = Object.entries(this.APP_ENUM.UserType).map((item) => ({key: item[0], value: item[1]}))
  public levelList: LevelDto[] = []
  public changeTypeList = Object.entries(this.APP_ENUM.ESalaryRequestEmployeeType).map((item) => ({key: item[0], value: item[1]}))
  constructor(
    private injector: Injector,
    private userTypeService: UserTypeService,
    private levelService: LevelService,
    private salaryChangeRequestService: SalaryChangeRequestService
    ) {
    super(injector)
   }

  ngOnInit(): void {
    this.getAllLevel();
    this.bindData();

  }

  getAllLevel(){
    this.subscription.push(
      this.levelService.getAll().subscribe(rs => {
        this.levelList = this.mapToFilter(rs.result, true)
      })
    )
  }

  bindData() {
    const salaryHistory = this.dialogData
    this.updateSalaryHistory = {
       ...salaryHistory,
       applyDate: this.formatDateYMD(new Date(this.dialogData.applyDate)),
       type: this.dialogData.type
    }
  }

  onSave(){
    abp.message.confirm("Update Employee salary history?", "", (rs) => {
      if(rs) {
        const data: UpdateRequestEmployeeInfoDto = {
          applyDate: this.formatDateYMD(new Date(this.updateSalaryHistory.applyDate)),
          salaryChangeRequestId: this.dialogData.request?.id,
          levelId: this.updateSalaryHistory.fromLevelId,
          jobPositionId: this.updateSalaryHistory.fromJobPositionId,
          salary: this.updateSalaryHistory.fromSalary,
          hasContract: this.updateSalaryHistory.hasContract,
          employeeId: this.updateSalaryHistory.employeeId,
          fromUserType: this.updateSalaryHistory.fromUserType,
          note: this.updateSalaryHistory.note,
          toJobPositionId: this.updateSalaryHistory.toJobPositionId,
          toLevelId: this.updateSalaryHistory.toLevelId,
          toSalary: this.updateSalaryHistory.toSalary,
          toUserType: this.updateSalaryHistory.toUserType,
          type: this.updateSalaryHistory.type,
          id: this.updateSalaryHistory.id
        }
       this.subscription.push(
        this.salaryChangeRequestService.updateRequestEmployeeInfo(data).subscribe(rs => {
          abp.message.success("Salary change history updated")
          this.dialogRef.close()
        })
       )
      }
    })
  }
}