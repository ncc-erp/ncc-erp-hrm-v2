import { Component, Injector, OnInit } from '@angular/core';
import { EmployeeService } from '@app/service/api/employee/employee.service';
import { MessageResponse } from '@app/service/model/common.dto';
import { DialogComponentBase } from '@shared/dialog-component-base';

@Component({
  selector: 'app-update-employee-from-file',
  templateUrl: './update-employee-from-file.component.html',
  styleUrls: ['./update-employee-from-file.component.css']
})
export class UpdateEmployeeFromFileComponent  extends DialogComponentBase<any>implements OnInit {

  public selectedFile: File;
  public bonusId:string = "";
  public results = {} as MessageResponse;
  constructor(injector: Injector, private employeeService : EmployeeService) {super(injector)}

  ngOnInit(): void {
  }

  public onSelectFile(e){
    this.selectedFile = e.target.files.item(0);
  }

  public onUpdateEmployeeFromFile(){
    this.isLoading = true;
    let input = new FormData();
    input.append("file",this.selectedFile);
    this.subscription.push(
      this.employeeService.updateEmployeeFromFile(input).subscribe((res)=>{
        if(res){
          this.results = res.result;
          abp.message.success(`<p class="successList" style='color:#28a745'>Success: <b>${this.results.successList.length}</b>  ${this.results.successList.length > 1? ' employees' : ' employee'}</p>
          <p style='color:red'>Failed: <b>${this.results.failedList.length}</b>${this.results.failedList.length > 1? ' employees' : ' employee'}</p> 
          <div style = 'max-height: 500px !important ; overflow-y: auto; overflow-x: hidden'>${ this.getFailMessage(this.results.failedList)}</div>
        `, 
       'Update employee',true);
        this.isLoading = false;
        this.dialogRef.close(this.results);
        }
      },()=> this.isLoading = false)
    )
  }

  public getFailMessage(failedList){
  let messages = failedList.length > 0 ? `<div class='row'><div class='col-1'>
    <b class='text-center'>Row</b>
  </div>
  <div class='col-5'><b class='text-center'>Email</b></div>
  <div class='col-6'><b class='text-center'>Error</b></div>
  </div>` : "";
  failedList.forEach(mess=> {
    messages+=`<div class='row'><div class='col-1'>
     ${mess.row}
    </div>
    <div class='col-5 text-left'>${mess.email}</div>
    <div class='col-6 text-left'>${mess.reasonFail}</div>
    </div>`
  });
  return messages;
  } 
}