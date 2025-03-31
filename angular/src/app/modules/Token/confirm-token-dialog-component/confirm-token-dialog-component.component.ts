import { PayslipService } from '@app/service/api/payslip/payslip.service';
import { Component, Inject, Injector, OnInit } from '@angular/core';
import { AppComponent } from '@app/app.component';
import { extend } from '@node_modules/@types/lodash';
import { AppComponentBase } from '@shared/app-component-base';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { BenefitService } from '@app/service/api/benefits/benefit.service';
import { APP_ENUMS } from '@shared/AppEnums';
@Component({
  selector: 'app-confirm-token-dialog-component',
  templateUrl: './confirm-token-dialog-component.component.html',
  styleUrls: ['./confirm-token-dialog-component.component.css']
})
export class ConfirmTokenDialogComponent extends AppComponentBase {
 public mezonToken : number;
 public title : string;
 public selectedBenefit : number;
 public listBenefit : any[] = [];
 public listTypeBenefit: any
  constructor(injetor : Injector,
    @Inject(MAT_DIALOG_DATA) public data: any,
    public dialogRef: MatDialogRef<ConfirmTokenDialogComponent>,private payslipService : PayslipService,private benefitService: BenefitService
  ) { 
    super(injetor);
  }
 
  ngOnInit(): void {
       this.title = "Detach Token";
       this.getBenefitActive();
       this.getAllBenefitType();
  }
 isCheck(){
  return this.mezonToken != null;
 }

 onBenefitChange(value: any) {
  console.log('Benefit selected:', value);
  this.selectedBenefit = value;
}
 public getBenefitActive(){
  this.subscription.push(
    this.benefitService.GetBenefitActive().subscribe((rs) => {  
      if(rs.success){
        this.listBenefit = rs.result;
      } 
    } 
  )
   
  )}  
    private getAllBenefitType(){
      const listTypeBenefit = this.getListFormEnum(APP_ENUMS.BenefitType).filter(item => item.key != 'All');
      this.listTypeBenefit = listTypeBenefit.reduce((acc, item) => {
        acc[item.value as string] = item.key; 
        return acc;
      }, {});
    }
  
  public send(){
        this.subscription.push(
          this.payslipService.changePayslipWithToken(this.data.payrollId,this.mezonToken,this.selectedBenefit).subscribe((rs) => {
            if(rs.success){
            this.notify.success("Detach token success");
              this.dialogRef.close(true);
              this.isLoading = false;
            }
          })
        )
  }

}
