import { Component, Injector, OnInit } from '@angular/core';
import { PayslipService } from '@app/service/api/payslip/payslip.service';
import { MessageResponse } from '@app/service/model/common.dto';
import { AppComponentBase } from '@shared/app-component-base';
import { DialogComponentBase } from '@shared/dialog-component-base';

@Component({
  selector: 'app-import-employee-remain-leave-days-after-calculating-salary',
  templateUrl: './import-employee-remain-leave-days-after-calculating-salary.component.html',
  styleUrls: ['./import-employee-remain-leave-days-after-calculating-salary.component.css']
})
export class ImportEmployeeRemainLeaveDaysAfterCalculatingSalaryComponent extends DialogComponentBase<any> implements OnInit {

  constructor(injector: Injector,
    private payslipService: PayslipService) {
    super(injector);
  }
  public payrollId:number = 0;
  public selectedFiles: FileList;
  public results = {} as MessageResponse;
  ngOnInit(): void {
    this.payrollId = this.dialogData;
  }

  public onSelectFile(event){
    this.selectedFiles = event.target.files;
  }

  public onImportExcel(){
    console.log(this.selectedFiles)
    if(!this.selectedFiles){
      abp.message.error("Choose a file!");
      return;
    }
    const formData = new FormData();
    formData.append("payrollId", this.payrollId.toString());
    formData.append("file", this.selectedFiles.item(0));
    this.subscription.push(
      this.payslipService.UpdateEmployeeRemainLeaveDaysAfterCalculatingSalary(formData).subscribe((rs)=>{
        if(rs){
          abp.message.success(`Update remain leave days successful`);
          this.results = rs.result;
          abp.message.success(`<p style='color:#28a745'>Success<b> ${this.results.successList.length} </b>${this.results.successList.length > 1? ' employees' : ' employee'}!</p>
          <p style='color:red'>Failed <b>(${this.results.failedList.length})</b>${this.results.failedList.length > 1? ' employees' : ' employee'}</p> 
          <p>${this.getFailMessage(this.results.failedList)}</p>
        `, 
         'Update remain leave days',true)
          this.dialogRef.close(true);
        }
      })
    )
  }

  public getFailMessage(failedList){
    let messages = failedList.length > 0 ? `<div class='row'><div class='col-1'>
      <b class='text-center'>#</b>
    </div>
    <div class='col-5'><b class='text-center'>Email</b></div>
    <div class='col-3'><b class='text-center'>RLD after</b></div>
    <div class='col-3'><b class='text-center'>Error</b></div>
    </div>` : "";
    failedList.forEach(mess=> {
      messages+=`<div class='row'><div class='col-1'>
       ${mess.row}
      </div>
      <div class='col-5 text-left'>${mess.email}</div>
      <div class='col-3 text-center'>${mess.remainLeaveDays}</div>
      <div class='col-3 text-left'>${mess.reasonFail}</div>
      </div>`
    });
    return messages;
    }

}

export class FailedRespon{

}
