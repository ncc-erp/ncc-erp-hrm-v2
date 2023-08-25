import { Message } from '@angular/compiler/src/i18n/i18n_ast';
import { Component, Injector, OnInit } from '@angular/core';
import { BonusService } from '@app/service/api/bonuses/bonus.service';
import { MessageResponse } from '@app/service/model/common.dto';
import { DialogComponentBase } from '@shared/dialog-component-base';

@Component({
  selector: 'app-import-employee-to-bonus',
  templateUrl: './import-employee-to-bonus.component.html',
  styleUrls: ['./import-employee-to-bonus.component.css']
})
export class ImportEmployeeToBonusComponent extends DialogComponentBase<any>implements OnInit {

  public selectedFile: File;
  public bonusId:string = "";
  public results = {} as MessageResponse;
  constructor(injector: Injector, private bonusService: BonusService) {super(injector)}

  ngOnInit(): void {
    this.bonusId = this.dialogData.id;
  }

  public onSelectFile(e){
    this.selectedFile = e.target.files.item(0);
  }

  public onImportExcel(){
    let input = new FormData();
    input.append("bonusId",this.bonusId);
    input.append("file",this.selectedFile);
    this.subscription.push(
      this.bonusService.importEmployeeToBonus(input).subscribe((res)=>{
        if(res){
          this.results = res.result;
          abp.message.success(`<p class="successList" style='color:#28a745'>Success: <b>${this.results.successList.length}</b>  ${this.results.successList.length > 1? ' employees' : ' employee'}</p>
          <p style='color:red'>Failed: <b>${this.results.failedList.length}</b>${this.results.failedList.length > 1? ' employees' : ' employee'}</p> 
          <div>${ this.getFailMessage(this.results.failedList)}</div>
        `, 
       'Import employees to bonus',true);
        this.dialogRef.close(this.results);
        }
      })
    )
  }

  public getFailMessage(failedList){
  let messages = failedList.length > 0 ? `<div class='row'><div class='col-1'>
    <b class='text-center'>Row</b>
  </div>
  <div class='col-5'><b class='text-center'>Email</b></div>
  <div class='col-2'><b class='text-center'>Money</b></div>
  <div class='col-4'><b class='text-center'>Error</b></div>
  </div>` : "";
  failedList.forEach(mess=> {
    messages+=`<div class='row'><div class='col-1'>
     ${mess.row}
    </div>
    <div class='col-5 text-left'>${mess.email}</div>
    <div class='col-2 text-right'>${mess.money}</div>
    <div class='col-4 text-left'>${mess.reasonFail}</div>
    </div>`
  });
  return messages;
  } 
}