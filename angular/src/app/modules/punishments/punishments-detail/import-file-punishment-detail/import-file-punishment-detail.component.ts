import { catchError } from 'rxjs/operators';
import { PunishmentService } from './../../../../service/api/punishment/punishment.service';
import { DialogComponentBase } from '@shared/dialog-component-base';
import { Component, OnInit, Injector } from '@angular/core';
import { MessageResponse } from '@app/service/model/common.dto';

@Component({
  selector: 'app-import-file-punishment-detail',
  templateUrl: './import-file-punishment-detail.component.html',
  styleUrls: ['./import-file-punishment-detail.component.css']
})
export class ImportFilePunishmentDetailComponent extends DialogComponentBase<any> implements OnInit {
  public selectedFiles:  FileList;
  public punishmentId : number;
  public results = {} as MessageResponse;
  constructor(injector : Injector , private punishmentService : PunishmentService) {super(injector)}

  ngOnInit(): void {
    this.punishmentId = this.dialogData.data.punishmentId;
  }
  public onSelectFile(event){
    this.selectedFiles = event.target.files;
  }
  public onImportExcel(){
    if (!this.selectedFiles) {
      abp.message.error("Choose a file!")
      return
    }
    const formData = new FormData();
    formData.append('PunishmentId', this.punishmentId.toString());
    formData.append('File', this.selectedFiles.item(0));
    this.subscription.push(this.punishmentService.importFilePunishmentDetailComponent(formData )
    .subscribe((res) => {
      if(res){
        this.results = res.result;
        abp.message.success(`<p style='color:#28a745'>Success<b> ${this.results.successList.length} </b>${this.results.successList.length > 1? ' employees' : ' employee'}!</p>
        <p style='color:red'>Failed <b>(${this.results.failedList.length})</b>${this.results.failedList.length > 1? ' employees' : ' employee'}</p> 
        <p>${this.getFailMessage(this.results.failedList)}</p>
      `, 
       'Add employees to punishment',true)
        this.dialogRef.close(this.results);
      }
    }))
  }
  public getFailMessage(failedList){
    let messages = failedList.length > 0 ? `<div class='row'><div class='col-1'>
      <b class='text-center'>#</b>
    </div>
    <div class='col-5'><b class='text-center'>Email</b></div>
    <div class='col-3'><b class='text-center'>Money</b></div>
    <div class='col-3'><b class='text-center'>Error</b></div>
    </div>` : "";
    failedList.forEach(mess=> {
      messages+=`<div class='row'><div class='col-1'>
       ${mess.row}
      </div>
      <div class='col-5 text-left'>${mess.email}</div>
      <div class='col-3 text-right'>${mess.money}</div>
      <div class='col-3 text-left'>${mess.reasonFail}</div>
      </div>`
    });
    return messages;
    }
}
