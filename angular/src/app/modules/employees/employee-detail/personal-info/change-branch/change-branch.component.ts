import { Component, Injector, OnInit } from '@angular/core';
import { BranchService } from '@app/service/api/categories/branch.service';
import { EmployeeService } from '@app/service/api/employee/employee.service';
import { BranchDto } from '@app/service/model/categories/branch.dto';
import { DialogComponentBase } from '@shared/dialog-component-base';
import * as _ from 'lodash';
import { PersonalInfoComponent } from '../personal-info.component';

@Component({
  selector: 'app-change-branch',
  templateUrl: './change-branch.component.html',
  styleUrls: ['./change-branch.component.css']
})
export class ChangeBranchComponent extends DialogComponentBase<any> implements OnInit {
  
  constructor(
    injector: Injector,
    private branchService: BranchService,
    private employeeService: EmployeeService) {
    super(injector);
  }

  public personalInfo = {} as PersonalInfoComponent;
  public branches:BranchDto[] = [];
  public newBranch:number = 0;
  public inputToUpdate = {} as ChangeEmployeeBranchDto;


  ngOnInit(): void {
    this.personalInfo = this.dialogData.personalInfo;
    this.title = `Change branch of <b>${this.personalInfo.fullName}</b>`;
    this.getAllBranch();
  }

  public getAllBranch(){
    this.subscription.push(
      this.branchService.getAll().subscribe((rs)=>{
        this.branches = rs.result;
        this.branches = this.branches.filter(x=> x.id != this.personalInfo.branchId);
      })
    )
  }

  public saveAndClose(){
    this.inputToUpdate.employeeId = this.personalInfo.id;
    this.inputToUpdate.date = this.formatDateYMD(this.inputToUpdate.date)
    this.subscription.push(
      this.employeeService.changeEmployeeBranch(this.inputToUpdate).subscribe((rs)=>{
        abp.notify.success(`Change branch of ${this.personalInfo.fullName} successful`);
        this.dialogRef.close(this.inputToUpdate)
      })
    )
  }
}
export interface ChangeEmployeeBranchDto{
  employeeId: number;
  branchId: number;
  date: string;
}
