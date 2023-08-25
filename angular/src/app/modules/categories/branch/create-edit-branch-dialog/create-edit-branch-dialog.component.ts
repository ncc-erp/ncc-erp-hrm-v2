import { Component, Injector, OnInit } from '@angular/core';
import { BranchService } from '@app/service/api/categories/branch.service';
import { BranchDto } from '@app/service/model/categories/branch.dto';
import { startWithTap } from '@shared/helpers/observerHelper';
import { DialogComponentBase } from '../../../../../shared/dialog-component-base';
import { finalize } from 'rxjs/operators'
import { EmployeeService } from '@app/service/api/employee/employee.service';
import { GetEmployeeBasicInfo } from '@app/service/model/employee/employee.dto';
@Component({
  selector: 'app-create-edit-branch-dialog',
  templateUrl: './create-edit-branch-dialog.component.html',
  styleUrls: ['./create-edit-branch-dialog.component.css']
})
export class CreateEditBranchDialogComponent extends DialogComponentBase<BranchDto> implements OnInit {
  public branch = {} as BranchDto;
  public searchEmployee:string = "";
  public listEmployees: GetEmployeeBasicInfo[] = [];
  constructor(injector: Injector, 
    private branchService: BranchService,
    private employeeService: EmployeeService) {
    super(injector)
  }

  ngOnInit(): void {
    if (this.dialogData?.id) {
      this.branch = this.dialogData
      console.log(this.branch)
      this.title = `Edit branch <strong>${this.dialogData.name}</strong>`
    }
    else {
      this.branch.color = this.APP_CONST.defaultBadgeColor;
      this.title = "Create new branch"
    }
    this.getAllEmployees();
  }
  nameChange(event){
    if(!this.dialogData?.id){
        this.branch.shortName = event
        this.branch.code = event;
    }
  }
  saveAndClose() {
    this.trimData(this.branch)
    if (this.dialogData?.id) {
      this.subscription.push(this.branchService.update(this.branch)
      .pipe(startWithTap(() => { this.isLoading = true }))
      .pipe(finalize(() => { this.isLoading = false }))
      .subscribe(rs => {
        abp.notify.success(`Update branch successfull`)
        this.dialogRef.close(true)
      }))
    }
    else {
      this.subscription.push(
        this.branchService.create(this.branch)
        .pipe(startWithTap(() => { this.isLoading = true }))
        .pipe(finalize(() => { this.isLoading = false }))
        .subscribe(rs => {
          abp.notify.success(`Created new branch ${this.branch.name}`)
          this.dialogRef.close(true)
        })
      )
    }
  }

  public getAllEmployees(){
    this.subscription.push(
      this.employeeService.getAllEmployeeBasicInfo().subscribe((rs)=>{
        this.listEmployees = rs.result;
      })
    )
  }
}

