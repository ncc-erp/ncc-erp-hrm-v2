import { Component, OnInit, Injector } from '@angular/core';
import {  MatDialogRef } from '@angular/material/dialog';
import { PayslipService } from '@app/service/api/payslip/payslip.service';
import { UpdatePayslipInfo } from '@app/service/model/payslip/payslip.dto';
import { DialogComponentBase } from '@shared/dialog-component-base';

@Component({
  selector: 'app-edit-payslip-detail-dialog',
  templateUrl: './edit-payslip-detail-dialog.component.html',
  styleUrls: ['./edit-payslip-detail-dialog.component.css']
})
export class EditPayslipDetailDialogComponent extends DialogComponentBase<UpdatePayslipInfo> implements OnInit {
public input:UpdatePayslipInfo
id:number;
  constructor(injector: Injector,
    private payslipService:PayslipService, public dialog: MatDialogRef<EditPayslipDetailDialogComponent>) 
  { 
    super(injector);
  }

  ngOnInit(): void {
    Object.assign(this,this.dialogData)
    this.title = "Edit Payslip Info";
    this.GetPaySlipToUpdateInfo();
  }

  saveAndClose() {
    this.trimData(this.input)
    this.isLoading = true;
    if (this.dialogData?.id) {
      this.subscription.push(this.payslipService.UpdatePayslipDetail(this.input)
      .subscribe(rs => {
        abp.notify.success(`Update payslip detail successfully`)
        this.dialogRef.close(true)
        this.isLoading = false;
      }))
    }
  }

  public GetPaySlipToUpdateInfo(){
    this.subscription.push(
      this.payslipService.GetPayslipBeforeUpdateInfo(this.id).subscribe((rs)=>{
        this.input = rs.result;
      })
    )
  }
}
