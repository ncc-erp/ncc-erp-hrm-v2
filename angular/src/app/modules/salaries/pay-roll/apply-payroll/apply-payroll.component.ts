import { result } from '@node_modules/@types/lodash';

import { inject } from '@angular/core/testing';
import { Component, Injector, OnInit } from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
 import { PayslipService } from '@app/service/api/payslip/payslip.service';
 import { PayRollService } from '@app/service/api/pay-roll/pay-roll.service';
 import { Inject } from '@angular/core';

@Component({
  selector: 'app-apply-payroll',
  templateUrl: './apply-payroll.component.html',
  styleUrls: ['./apply-payroll.component.css']
})
export class ApplyPayrollComponent extends AppComponentBase {

public listPayroll: any[] = [];
public title : string;
public defaultPayrollId: number[] = [];
public payslip: any;
public selectedBranchId : number;
public selectedLevelId : number;
public selectedJobPositionid: number;
public selectedUsertype : number;
public payrollIds: number[] = [];

public selecteUserTypeName: string;
public selectedJobPositionName : string;
public selectedLevelName : string;
public selectedBranchName : string;

  constructor(injetor : Injector,
    @Inject(MAT_DIALOG_DATA) public data: any,
    public dialogRef: MatDialogRef<ApplyPayrollComponent>,private payslipService : PayslipService, private payrollService:PayRollService,
  ) { 
    super(injetor);
  }
  ngOnInit(): void {
    this.title = this.data.payslip.email;
    this.payslip = this.data.payslip;
    this.defaultPayrollId[0] = this.data.payrollId
    this.selectedBranchId = this.data.selectedBrandId;
    this.selectedLevelId = this.data.selectedLevelId;
    this.selectedJobPositionid = this.data.selectedJobPositionId;
    this.selectedUsertype = this.data.selectedUsertype;
    this.selecteUserTypeName = this.data.selecteUserTypeName;
    this.selectedJobPositionName = this.data.selectedJobPositionName;
    this.selectedLevelName = this.data.selectedLevelName;
    this.selectedBranchName = this.data.selectedBranchName;
    this.getListPayroll();
     
  }

  isCheckUpdateBranch(){
    if(this.selectedBranchId != this.payslip.branchId) {
     return  true;
    }
  }

  isCheckUpdateLevel(){
    if(this.selectedLevelId != this.payslip.levelId) {
      return true;
    }
  }
  isCheckUpdateUserType(){
    if(this.selectedUsertype != this.payslip.payslipUserType) {
     return true;
    }
  }
  isCheckUpdateJobPosition(){
    if(this.selectedJobPositionid != this.payslip.jobPositionId) {
     return true;
    }
  }

  public getListPayroll(){
      this.payslipService.getPayrollForEmployee(this.payslip.employeeId).subscribe((res: any) => {
        this.listPayroll = res.result;
        if(this.isCheckUpdateBranch()) {
          this.listPayroll = this.listPayroll.map((item: any) => ({
            value: item.value,
            name : item.name,
            dataType : item.branch
        }))
      } 
    
      if(this.isCheckUpdateLevel()) {
        this.listPayroll = this.listPayroll.map((item: any) => ({
          value: item.value,
          name : item.name,
          dataType : item.level
      }))
    }  

    if(this.isCheckUpdateJobPosition()) {
      this.listPayroll = this.listPayroll.map((item: any) => ({
        value: item.value,
        name : item.name,
        dataType : item.jobPosition
    }))
  }  

  if(this.isCheckUpdateUserType()) {
    this.listPayroll = this.listPayroll.map((item: any) => ({
      value: item.value,
      name : item.name,
      dataType : item.userType
  }))
}  

    }   
    );
  }

  public onSelectPayroll(ids: number[]){
    this.payrollIds = ids;
  }
  public send(){
    if(this.isCheckUpdateBranch()){
      this.updateBranch(this.payrollIds);
    }
    if(this.isCheckUpdateLevel()){
      this.updateLevel(this.payrollIds);
    }
    if(this.isCheckUpdateJobPosition()){
      this.updateJobPosition(this.payrollIds);
    }
    if(this.isCheckUpdateUserType()){
      this.updateUserType(this.payrollIds);
    }
  }
  private updateBranch(ids:number[]){
    const input = {
      payrollIds :     ids,
      branchId : this.selectedBranchId,
      employeeId : this.payslip.employeeId,
    }
    this.payslipService.updateBranchEmployeeForListPayroll(input).subscribe((res: any) => {
      this.notify.success(this.l('Update Branch List Payroll Successfull !'));
      this.dialogRef.close(true);
    }
    );
  }

  private updateJobPosition(ids:number[]){
    const input = {
      payrollIds :     ids,
      jobPositionId : this.selectedJobPositionid,
      employeeId : this.payslip.employeeId,
    }
    this.payslipService.updateJobPositionEmployeeForListPayroll(input).subscribe((res: any) => {
      this.notify.success(this.l('Update JobPosition List Payroll Successfull !'));
      this.dialogRef.close(true);
    }
    );
  }
  private updateLevel(ids:number[]){
    const input = {
      payrollIds :     ids,
      levelId : this.selectedLevelId,
      employeeId : this.payslip.employeeId,
    }
    this.payslipService.updateLevelEmployeeForListPayroll(input).subscribe((res: any) => {
      this.notify.success(this.l('Update Level List Payroll Successfull !'));
      this.dialogRef.close(true);
    }
    );
  }
  private updateUserType(ids:number[]){
    const input = {
      payrollIds :     ids,
      userType : this.selectedUsertype  ,
      employeeId : this.payslip.employeeId,
    }
    this.payslipService.updateUserTypeEmployeeForListPayroll(input).subscribe((res: any) => {
      this.notify.success(this.l('Update UserType List Payroll Successfull !'));
      this.dialogRef.close(true);
    }
    );
  }
  public isCheck(){
    return this.listPayroll.length > 0;
  }
}

