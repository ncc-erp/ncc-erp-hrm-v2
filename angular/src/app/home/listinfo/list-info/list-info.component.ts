import { Component, OnInit, Injector } from "@angular/core";
import { LastEmployeeWorkingHistoryDto } from "@app/service/model/homepage/HomepageEmployeeStatistic.dto";
import { DialogComponentBase } from "@shared/dialog-component-base";
import { MatDialog } from "@angular/material/dialog";
import { PERMISSIONS_CONSTANT } from "@app/permission/permission";
import { AppConsts } from "@shared/AppConsts";
import { APP_ENUMS } from "@shared/AppEnums";
@Component({
  selector: "app-list-info",
  templateUrl: "./list-info.component.html",
  styleUrls: ["./list-info.component.css"],
})
export class ListInfoComponent
  extends DialogComponentBase<any>
  implements OnInit {
  constructor(injector: Injector, public dialog: MatDialog) {
    super(injector);
  }
  public isShowOnboardAndQuit: boolean = false;
  public list: LastEmployeeWorkingHistoryDto[];
  public tempListEmployee: LastEmployeeWorkingHistoryDto[] = [];
  public sortDirectionEnum =  APP_ENUMS.SortDirectionEnum;

  public employeeStatus = {
    1: "working",
    2: "pausing",
    3: "quit",
    4: "maternityLeave"
  }
  public sortDirection:number = -1;

  ngOnInit(): void {
    this.list = this.dialogData.listInfo;
    this.tempListEmployee = [...this.dialogData.listInfo];
    if(this.dialogData.isOnboardAndQuit){
      this.isShowOnboardAndQuit = true
    }

    this.title = `${this.dialogData.title} ${this.dialogData.action} ${this.dialogData.branchName} </b>`;
  }

  isAllowRoutingDetail() {
    return this.isGranted(PERMISSIONS_CONSTANT.Employee_EmployeeDetail);
  }

  getStatusStyle(userStatusEnum: number, isStyle: boolean) {
    return isStyle ? AppConsts.userStatus[userStatusEnum]?.class : AppConsts.userStatus[userStatusEnum]?.name
  }
  changeSort(){
    if(this.sortDirection == -1){
      this.sortDirection = 0;
      this.list = this.list.sort((x,y)=> new Date(x.dateAt).getTime() - new Date(y.dateAt).getTime());
      return;
    }
    if(this.sortDirection == 0){
      this.sortDirection = 1;
      this.list = this.list.sort((x,y)=> new Date(y.dateAt).getTime() - new Date(x.dateAt).getTime());
      return;
    }
    if(this.sortDirection == 1){
      this.sortDirection = -1;
      this.list = this.tempListEmployee;
      return;
    }
    
  }
  onClose(){
    this.dialogRef.close();
  }
}
