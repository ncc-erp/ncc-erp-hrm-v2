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
 public _mezonToken : number = 200000;
 public title : string;
 public selectedBenefit : number
 public defaultBenefit : any = null
 public listBenefit : any[] = [];
 public listTypeBenefit: any

  constructor(injetor : Injector,
    @Inject(MAT_DIALOG_DATA) public data: any,
    public dialogRef: MatDialogRef<ConfirmTokenDialogComponent>,private payslipService : PayslipService,private benefitService: BenefitService
  ) { 
    super(injetor);
  }
 
  ngOnInit(): void {
       this.title = "Split Benefit by Token";
       this.getBenefitActive();
       this.getAllBenefitType();
       
  }
 isCheck(){
  return this._mezonToken != null;
 }

 onBenefitChange(value: any) {
  this.selectedBenefit = value.id;
}
public getBenefitActive() {
  this.subscription.push(
    this.benefitService.GetBenefitActive().subscribe((rs) => {  
      if (rs.success) {
        this.listBenefit = rs.result.map((item: any) => ({
          ...item,
          name: item.name.toLowerCase(),
        }));

        const found = this.listBenefit.find(item => item.name === 'ăn trưa');
        this.defaultBenefit = found ? found : null;
        this.selectedBenefit = this.defaultBenefit ? this.defaultBenefit.id : null;
      } 
    })
  );
}

    private getAllBenefitType(){
      const listTypeBenefit = this.getListFormEnum(APP_ENUMS.BenefitType).filter(item => item.key != 'All');
      this.listTypeBenefit = listTypeBenefit.reduce((acc, item) => {
        acc[item.value as string] = item.key; 
        return acc;
      }, {});
      
    }
  
    public send() {
      this.isLoading = true; 
     this._mezonToken = Number(this._mezonToken) || 0;
      this.subscription.push(
        (this.data?.payslipId
          ? this.payslipService.changePayslipWithTokenForOnePayslip(this.data.payslipId, this.selectedBenefit, this._mezonToken)
          : this.payslipService.changePayslipWithToken(this.data.payrollId, this._mezonToken, this.selectedBenefit))
          .subscribe({
            next: (rs) => {
              this.isLoading = false; 
              if (rs.success) {
                this.notify.success("Detach token success");
                this.dialogRef.close(true);
              }
            },
            error: (err) => {
              this.isLoading = false; 
              this.notify.error("An error occurred while processing.");
              console.error(err);
            },
          })
      );
    }

    get mezonToken(): string {
      return new Intl.NumberFormat('en-US').format(this._mezonToken);
    }
    
    set mezonToken(value: string) {
      this._mezonToken = Number(value.replace(/,/g, '')) || 0;
    }
    compareBenefit(o1: any, o2: any): boolean {
      return o1 && o2 ? o1.id === o2.id : o1 === o2;
    }
    

}
