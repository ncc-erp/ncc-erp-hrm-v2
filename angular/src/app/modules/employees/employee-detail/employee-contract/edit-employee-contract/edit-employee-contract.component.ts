import { Component, Injector, OnInit } from '@angular/core';
import { JobPositionService } from '@app/service/api/categories/jobPosition.service';
import { LevelService } from '@app/service/api/categories/level.service';
import { UserTypeService } from '@app/service/api/categories/userType.service';
import { EmployeeContractService } from '@app/service/api/employee/employeeContract.service';
import { JobPositionDto } from '@app/service/model/categories/jobPosition.dto';
import { LevelDto } from '@app/service/model/categories/level.dto';
import { UserTypeDto } from '@app/service/model/categories/userType.dto';
import { DialogComponentBase } from '@shared/dialog-component-base';
import { EmployeeContractDto } from '../employee-contract.component';

@Component({
  selector: 'app-edit-employee-contract',
  templateUrl: './edit-employee-contract.component.html',
  styleUrls: ['./edit-employee-contract.component.css']
})
export class EditEmployeeContractComponent extends DialogComponentBase<any> implements OnInit {
  public employeeContract = {} as EmployeeContractDto;
  public positionList: JobPositionDto[] = []
  public userTypeList: UserTypeDto[] = []
  public levelList: LevelDto[] = [];
  constructor(
    injector: Injector,
    private employeeContractService: EmployeeContractService,
    private positionService: JobPositionService,
    private levelService : LevelService,
    private userTypeService : UserTypeService
  ) {
    super(injector)
  }

  ngOnInit(): void {
    this.employeeContract = this.dialogData;
    console.log(this.employeeContract)
    this.getAllJobPositon();
    this.getAllUserType();
    this.getAllLevel();
  }
  onSave() {
    this.employeeContract.startDate = this.formatDateYMD(this.employeeContract.startDate);
    if(this.employeeContract.endDate){
      this.employeeContract.endDate = this.formatDateYMD(this.employeeContract.endDate);
    }
    this.isLoading = true;
    this.subscription.push(
      this.employeeContractService.updateEmployeeContract(this.employeeContract).subscribe((rs)=>{
        abp.notify.success("Update employee contract successful!");
        this.isLoading = false;
        this.dialogRef.close(true);
      },()=> this.isLoading = false)
    )
  }

  public onChange(event){
    this.employeeContract.realSalary = Math.round( Number(this.employeeContract.basicSalary)*(this.employeeContract.probationPercentage/100));
  }

  private getAllJobPositon() {
    this.subscription.push(this.positionService.getAll().subscribe(rs => {
      this.positionList = this.mapToFilter(rs.result, true)
    }))
  }
  private getAllUserType() {
    this.subscription.push(this.userTypeService.getAll().subscribe(rs => {
      this.userTypeList = this.mapToFilter(rs.result, true)
    }))
  }

  private getAllLevel() {
    this.subscription.push(this.levelService.getAll().subscribe(rs => {
      this.levelList = this.mapToFilter(rs.result, true)
    }))
  }

}
