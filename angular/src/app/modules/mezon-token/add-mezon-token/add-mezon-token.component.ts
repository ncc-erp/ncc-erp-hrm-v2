import { PayRollService } from '@app/service/api/pay-roll/pay-roll.service';
import { Component, Injector, OnInit } from '@angular/core';
import { EmployeeService } from '@app/service/api/employee/employee.service';
import { MezonTokenDto } from '@app/service/model/payroll-token/PayrollTokenDto.dto';
import { DialogComponentBase } from '@shared/dialog-component-base';
import { MezonTokenServiceService } from '@app/service/api/mezon-token/mezon-token-service.service';
import { BenefitService } from '@app/service/api/benefits/benefit.service';
import { APP_ENUMS } from '@shared/AppEnums';
import { benefitDto } from '@app/service/model/benefits/beneft.dto';
import { FormControl } from '@angular/forms';
import { of, EMPTY } from 'rxjs';
import { switchMap, catchError } from 'rxjs/operators';
@Component({
  selector: 'app-add-mezon-token',
  templateUrl: './add-mezon-token.component.html',
  styleUrls: ['./add-mezon-token.component.css']
})
export class AddMezonTokenComponent extends DialogComponentBase<any> implements OnInit {
  public mezonToken : MezonTokenDto = null;
  public listEmployeeExceptStatusQuit: any;
  public listBenefit: any[] = [];
  public listTypeBenefit: any;
  public selectedBenefit: number ;
  public selectedEmployee: number;
  public amount: number;
  public note: string = '';
  public defaultBenefit: any ;
  public employeeId: number;
  public searchUser: string = ''; 
  public nameBenefit: string ;
  public listPayroll: any[] = [];
  public payrollId: any;

  constructor(injector : Injector,private mezonTokenService :MezonTokenServiceService,
    private employeeService : EmployeeService,private benefitService: BenefitService,   private payRollService: PayRollService,
  ) { 
    super(injector);
    
   }

  ngOnInit(): void {
    this.title = this.dialogData.title;
    if(this.dialogData.mezonToken){
    this.note = this.dialogData.mezonToken.note;
    this.employeeId = this.dialogData.mezonToken.employeeId;
    this.amount = this.dialogData.mezonToken.amount;
    this.payrollId = this.dialogData.mezonToken.payrollId;
    this.selectedBenefit = (this.dialogData.mezonToken.referenceId != null && this.dialogData.mezonToken.referenceId != 0) ? this.dialogData.mezonToken.referenceId : -1;
    this.getBenefitDefault(this.selectedBenefit);
   
    }
    this.getListEmployee();
    this.getListPayroll();
  }
   
  isChecksBenfit(){
    if(this.selectedBenefit > 0){
      return true;
    }
    return false;
  }

  getListPayroll(){
    this.payRollService.GetPayrolls().subscribe((res) => {
      this.listPayroll = res.result;

    } )
  }
  
  getListEmployee(){
    this.subscription.push(
      this.employeeService.getEmployeeExceptStatusQuit().subscribe((res) => {
        this.listEmployeeExceptStatusQuit = res.result;
      }
   
      , () => this.isLoading = false)
    ) 
    
  }

  getBenefitDefault(id: number) {
    if(this.isChecksBenfit()){
    this.subscription.push(
      this.benefitService.GetBenefitById(id).pipe(
        switchMap((res1) => {
          if (!res1?.success || !res1.result) {
            return this.benefitService.GetBenefitByPayslipDetailId(id);
          }
          return of(res1);
        }),
        catchError((error) => {
          return of({ success: false, result: null });
        })
      ).subscribe((rs) => {
        if (rs && rs.success && rs.result) {
          this.defaultBenefit = {
            ...rs.result,
            name: rs.result.name?.toLowerCase(),
            
          };
          this.selectedBenefit = rs.result.id;
        } else {
          this.defaultBenefit = null;
          this.selectedBenefit = null;
        }
      })
    );
  }
  }
  compareBenefit = (a: any, b: any): boolean => {
    return a && b && a.id === b.id;
  }

 clearPayroll() {
  this.payrollId = null;
 }
  saveAndClose(){

        let input = {
        id: this.dialogData.mezonToken?.id,
        employeeId: this.employeeId,
        amount: this.amount,
        note: this.note,
        sentToEmployeeAt : "",
        payrollId: this.payrollId,
        referenceId: this.selectedBenefit
      }
    if(this.dialogData.type === 'edit'){
      this.subscription.push(
        this.mezonTokenService.editMezonToken(input).subscribe((rs) => {
          if (rs) {
            this.notify.success("Edit success");
            this.dialogRef.close(true);
          }
        }
        , () => this.isLoading = false)
    )
    }else{

     this.subscription.push(    
      this.mezonTokenService.create (input).subscribe((rs) => {
        if (rs) {
          this.notify.success("Add success");
          this.dialogRef.close(true);
        }
      }
      , () => this.isLoading = false)
  )}}
}
