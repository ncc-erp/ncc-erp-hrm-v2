import { Component, Injector, OnInit } from '@angular/core';
import { DialogComponentBase } from '@shared/dialog-component-base';
import * as moment from 'moment';

@Component({
  selector: 'app-confirm-change-status-dialog',
  templateUrl: './confirm-change-status-dialog.component.html',
  styleUrls: ['./confirm-change-status-dialog.component.css']
})
export class ConfirmChangeStatusDialogComponent extends DialogComponentBase<any> implements OnInit {

  public isConfirmed: boolean = false;
  public noteToCheckbox:string = "";
  
  constructor(injector: Injector) {
    super(injector);
  }

  ngOnInit(): void {
    this.title = "I am sure it happens";
    let applyDate = moment(this.dialogData.applyDate).format("DD/MM/YYYY");
    this.noteToCheckbox = `
    Checked: thay đổi trạng thái của nhân viên ngay lập tức
    Uncheck: trạng thái của nhân viên sẽ được thay đổi vào ngày ${applyDate}
    `


  }
  public onConfirm(e){
    this.isConfirmed = e;
  }

  onContinue(){
    let output = {
      isConfirmed : this.isConfirmed
    }
    this.dialogRef.close(output);
  }
  onCancel(){
    this.dialogRef.close(false);
  }

}
