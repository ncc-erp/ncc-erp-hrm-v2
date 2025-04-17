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
  public sentToEmployeeAt: Date;
  public defaultBenefit: any ;
  public employeeId: number;
  public searchUser: string = ''; 

  constructor(injector : Injector,private mezonTokenService :MezonTokenServiceService,
    private employeeService : EmployeeService,private benefitService: BenefitService,   
  ) { 
    super(injector);
    
   }

  ngOnInit(): void {
    this.title = this.dialogData.title;
    if(this.dialogData.mezonToken){
    this.note = this.dialogData.mezonToken.note;
    this.employeeId = this.dialogData.mezonToken.employeeId;
    this.sentToEmployeeAt = this.dialogData.mezonToken.sentToEmployeeAt;
    this.amount = this.dialogData.mezonToken.amount;
    this.selectedBenefit = this.dialogData.mezonToken.referenceId;
    this.getBenefitDefault(this.selectedBenefit);
    }

   
    this.getListEmployee();
    this.getBenefitActive();
    this.getAllBenefitType();

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

  compareBenefit = (a: any, b: any): boolean => {
    return a && b && a.id === b.id;
  }
  public getBenefitActive() {
    this.subscription.push(
      this.benefitService.GetBenefitActive().subscribe((rs) => {  
        if (rs.success) {
          this.listBenefit = rs.result.map((item: any) => ({
            ...item,
            name: item.name.toLowerCase(),
          }));;
        } 
      })
    );
  }
  onBenefitChange(value: any) {
    this.selectedBenefit = value.id;
  }
  private getAllBenefitType(){
        const listTypeBenefit = this.getListFormEnum(APP_ENUMS.BenefitType).filter(item => item.key != 'All');
        this.listTypeBenefit = listTypeBenefit.reduce((acc, item) => {
          acc[item.value as string] = item.key; 
          return acc;
        }, {});
        
      }
  saveAndClose(){
    let localDate: Date;

    if (this.sentToEmployeeAt instanceof Date) {
      localDate = this.sentToEmployeeAt;
    } else {
      localDate = new Date(this.sentToEmployeeAt);
    }
  
    localDate = new Date(localDate.getTime() + 7 * 60 * 60 * 1000);
    this.sentToEmployeeAt = localDate;

        let input = {
        id: this.dialogData.mezonToken?.id,
        employeeId: this.employeeId,
        amount: this.amount,
        note: this.note,
        sentToEmployeeAt: this.sentToEmployeeAt,
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
