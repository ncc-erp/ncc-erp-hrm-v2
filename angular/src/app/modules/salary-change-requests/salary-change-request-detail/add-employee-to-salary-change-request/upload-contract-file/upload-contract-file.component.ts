import { Component, Injector, OnInit } from '@angular/core';
import { EmployeeContractService } from '@app/service/api/employee/employeeContract.service';
import { ContractFileDto } from '@app/service/model/employee/employee.dto';
import { DialogComponentBase } from '@shared/dialog-component-base';

@Component({
  selector: 'app-upload-contract-file',
  templateUrl: './upload-contract-file.component.html',
  styleUrls: ['./upload-contract-file.component.css']
})
export class UploadContractFileComponent extends DialogComponentBase<any> implements OnInit {

  public selectedFile: File;
  public contractId:string = '';
  constructor(private injector: Injector, private employeeContractService: EmployeeContractService) { super(injector)}

  ngOnInit(): void {
    this.contractId = this.dialogData.id;
  }

  public selectFile(e){
    this.selectedFile = e.target.files.item(0);
    
  }
 
  public onImport(){
    if(!this.selectedFile){
      abp.message.error("Choose a file");
      return;
    }
    this.subscription.push(
      this.employeeContractService.uploadContractFile(this.selectedFile, this.contractId).subscribe((res)=>{
        abp.notify.success("Upload file successful");
        this.dialogRef.close(this.contractId);
      })
    )
    
  }

  
}
