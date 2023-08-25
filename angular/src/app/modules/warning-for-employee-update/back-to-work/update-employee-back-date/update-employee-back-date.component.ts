import { Component, Injector, OnInit} from '@angular/core';
import { WarningEmployeeService } from '@app/service/api/warning-employee/warning-employee.service';
import { UpdateEmployeeBackDateDto } from '@app/service/model/warning-employee/WarningEmployeeDto';
import { DialogComponentBase } from '@shared/dialog-component-base';

@Component({
  selector: 'app-update-employee-back-date',
  templateUrl: './update-employee-back-date.component.html',
  styleUrls: ['./update-employee-back-date.component.css']
})
export class UpdateEmployeeBackDateComponent extends DialogComponentBase<any> implements OnInit {
  public inputToUpdateBackDate = {} as  UpdateEmployeeBackDateDto;
  constructor( injector: Injector,
    private warningEmployeeService: WarningEmployeeService) {
    super(injector);
  }

  ngOnInit(): void {
    this.inputToUpdateBackDate = this.dialogData.inputToUpdate;
    this.title = `Update back date of <b>${this.dialogData.employeeName}</b>`;
  }
  public saveAndClose(){
    this.isLoading = true;
    this.inputToUpdateBackDate.backDate = this.formatDateYMD(this.inputToUpdateBackDate.backDate)
    this.subscription.push(
      this.warningEmployeeService.updateEmployeeBackDate(this.inputToUpdateBackDate).subscribe((rs)=>{
        abp.notify.success("Update back date succesful");
        this.isLoading = false;
        this.dialogRef.close(true);
      },()=> this.isLoading = false)
    )
  }

}
