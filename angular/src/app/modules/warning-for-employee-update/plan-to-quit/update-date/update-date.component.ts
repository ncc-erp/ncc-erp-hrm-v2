import { Component, Injector, OnInit } from '@angular/core';
import { WarningEmployeeService } from '@app/service/api/warning-employee/warning-employee.service';
import { DialogComponentBase } from '@shared/dialog-component-base';

@Component({
  selector: 'app-update-date',
  templateUrl: './update-date.component.html',
  styleUrls: ['./update-date.component.css']
})
export class UpdateDateComponent extends DialogComponentBase<any> implements OnInit {

  constructor(injector: Injector, private warningEmployeeService: WarningEmployeeService) {
    super(injector);
  }

  public inputToUpdate = {} as InputToUpdate;

  ngOnInit(): void {
    this.inputToUpdate = this.dialogData.item;
  }
  onSave(){
    this.inputToUpdate.dateAt = this.formatDateYMD(this.inputToUpdate.dateAt);
    this.isLoading = true;
    this.subscription.push(
      this.warningEmployeeService.UpdatePlanQuitBgJob(this.inputToUpdate).subscribe((rs)=>{
        if(rs){
          abp.notify.success("Update date successful");
        }
        this.isLoading = false;
        this.dialogRef.close(true);
      }, ()=> this.isLoading = false)
    )
  }
  onClose(){
    this.dialogRef.close();
  }

}
export class InputToUpdate{
  jobId: number;
  workingHistoryId: number;
  dateAt: string;
}
