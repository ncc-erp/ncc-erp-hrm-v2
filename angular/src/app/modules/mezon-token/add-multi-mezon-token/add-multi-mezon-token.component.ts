import { PayRollService } from '@app/service/api/pay-roll/pay-roll.service';
import { Component, Injector, OnInit } from '@angular/core';
import { DialogComponentBase } from '@shared/dialog-component-base';
import { MezonTokenServiceService } from '@app/service/api/mezon-token/mezon-token-service.service';
import { BenefitService } from '@app/service/api/benefits/benefit.service';
import { of } from 'rxjs';
import { switchMap, catchError } from 'rxjs/operators';
import { MatDialog } from '@angular/material/dialog';
import { AddEmployeeComponent } from '@shared/components/employee/add-employee/add-employee.component';

@Component({
  selector: 'app-add-multi-mezon-token',
  templateUrl: './add-multi-mezon-token.component.html',
  styleUrls: ['./add-multi-mezon-token.component.css']
})
export class AddMultiMezonTokenComponent extends DialogComponentBase<any> implements OnInit {
  public selectedBenefit: number = -1;
  public amount: number;
  public note: string = '';
  public defaultBenefit: any;
  public listPayroll: any[] = [];
  public payrollId: any;

  constructor(injector: Injector, private mezonTokenService: MezonTokenServiceService,
    private benefitService: BenefitService,
    private payRollService: PayRollService, private dialog: MatDialog
  ) { 
    super(injector)
  }

  ngOnInit(): void {
    this.title = this.dialogData.title;
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

 clearPayroll() {
  this.payrollId = null;
 }

 addEmployee() {
  const ref = this.dialog.open(AddEmployeeComponent, {
    width: "92vw",
    height: "95vh",
    maxWidth: "100vw",
    data: {
      title: `Select employees`
    }
  });

  ref.afterClosed().subscribe((res) => {
    if (res && res.length) {
      const input = {
        employeeIds: res,
        amount: this.amount,
        note: this.note,
        payrollId: this.payrollId,
        referenceId: this.selectedBenefit,
        sentToEmployeeAt: ""
      };
      this.mezonTokenService.createMany(input).subscribe(() => {
        abp.notify.success("Add token to multiple employees success");
        this.dialogRef.close(true);
      })
    }
  })
 }
}
