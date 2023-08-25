import { WarningEmployeeService } from './../../../service/api/warning-employee/warning-employee.service';
import { Component, Injector, OnInit } from '@angular/core';
import { PlanQuitEmployeeDto } from '../../../service/model/warning-employee/WarningEmployeeDto';
import { PERMISSIONS_CONSTANT } from '@app/permission/permission';
import { AppComponentBase } from '@shared/app-component-base';
import { MatDialog } from '@angular/material/dialog';
import { InputToUpdate, UpdateDateComponent } from './update-date/update-date.component';
import { APP_ENUMS } from '@shared/AppEnums';

@Component({
  selector: 'app-plan-to-quit',
  templateUrl: './plan-to-quit.component.html',
  styleUrls: ['./plan-to-quit.component.css']
})
export class PlanToQuitComponent extends AppComponentBase implements OnInit {
  WarningEmployee_PlanQuitEmployee_Edit = PERMISSIONS_CONSTANT.WarningEmployee_PlanQuitEmployee_Edit
  WarningEmployee_PlanQuitEmployee_Detele = PERMISSIONS_CONSTANT.WarningEmployee_PlanQuitEmployee_Detele

  public listBreadCrumb: any = []
  public planQuitEmployees: PlanQuitEmployeeDto[] = []
  public searchText:string = ""
  private tempPlanEmployee: PlanQuitEmployeeDto[] = []
  public sortProperty:string = ""
  public sortDirection:number = null;
  public sortDirectionEnum = APP_ENUMS.SortDirectionEnum
  public planQuitEmployeeStatusClass = this.APP_CONST.PlanQuitEmployeeStatus;
  constructor(injector: Injector, 
    private warningEmployeeService: WarningEmployeeService,
    public dialog: MatDialog ) {
    super(injector)
    this.listBreadCrumb = [
      { name: '<i class="fa-solid fa-house fa-sm"></i>', url: '' },
      { name: '<i class="fa-solid fa-chevron-right"></i> ' },
      { name: 'Plan Quit Employee' }];
  }

  ngOnInit(): void {
    this.GetPlanQuitEmployee();
  }

  GetPlanQuitEmployee() {
    this.warningEmployeeService.GetPlanQuitEmployee().subscribe(rs => {
      this.planQuitEmployees = rs.result
      this.tempPlanEmployee = [...this.planQuitEmployees];
      this.onSortDefault();
    })
  }

  onSearchEnter(){
      this.planQuitEmployees = this.tempPlanEmployee.filter(x => x.fullName.toLowerCase().trim().includes(this.searchText.toLowerCase().trim())
      || x.email.toLowerCase().trim().includes(this.searchText.toLowerCase().trim()))
  }

  deletePlanQuitEmployee(employee:PlanQuitEmployeeDto){
    abp.message.confirm("Are you sure to delete this job?", "", (rs)=>{
      if(rs){
          this.warningEmployeeService.DeletePlanQuitBgJob(employee.jobId).subscribe(rs =>{
            abp.notify.success(`Deleted quit Bg job for employee ${employee.fullName}`)
            this.GetPlanQuitEmployee()
          })
      }
    })

  }
  onUpdate(item){
    var dig = this.dialog.open(UpdateDateComponent,{
      data: {
        item: {
          jobId: item.jobId,
          workingHistoryId: item.workingHistoryId,
          dateAt: item.dateAt
        } as InputToUpdate
      },
      width: "500px"
    })
    dig.afterClosed().subscribe((rs)=>{
      if(rs){
        this.GetPlanQuitEmployee();
      }
    })
  }

  onSort(property: string){
    this.onSearchEnter();
    if(this.sortProperty != property){
      this.sortDirection = null;
    }
    if(property){
      switch(this.sortDirection){
        case null:{
          this.sortDirection = this.sortDirectionEnum.Ascending;
          this.sortProperty = property;
          this.onSortByAscending(property);
          break;
        }
        case this.sortDirectionEnum.Ascending:{
          this.sortDirection = this.sortDirectionEnum.Descending;
          this.sortProperty = property;
          this.onSortByDescending(property);
          break;
        }
        case this.sortDirectionEnum.Descending:{
          this.sortDirection = null;
          this.sortProperty = "";
          this.planQuitEmployees = this.planQuitEmployees;
          break;
        }
      }
    }
  }

  onSortByAscending(property){
    switch(property){
      case 'creationTime':{
        this.planQuitEmployees = this.planQuitEmployees.sort((fItem, sItem)=> new Date(fItem.creationTime).getTime() - new Date(sItem.creationTime).getTime());
        break;
      }
      case 'dateAt':{
        this.planQuitEmployees = this.planQuitEmployees.sort((fItem, sItem)=> new Date(fItem.dateAt).getTime() - new Date(sItem.dateAt).getTime());
        break;
      }
      case 'email':{
        this.planQuitEmployees = this.planQuitEmployees.sort((fItem, sItem)=> fItem.email.localeCompare(sItem.email));
        break;
      }
    }
  }

  onSortByDescending(property){
    switch(property){
      case 'creationTime':{
        this.planQuitEmployees = this.planQuitEmployees.sort((fItem, sItem)=> new Date(sItem.creationTime).getTime() - new Date(fItem.creationTime).getTime());
        break;
      }
      case 'dateAt':{
        this.planQuitEmployees = this.planQuitEmployees.sort((fItem, sItem)=> new Date(sItem.dateAt).getTime() - new Date(fItem.dateAt).getTime());
        break;
      }
      case 'email':{
        this.planQuitEmployees = this.planQuitEmployees.sort((fItem, sItem)=> sItem.email.localeCompare(fItem.email));
        break;
      }
    }
  }

  onSortDefault(){
    if(this.planQuitEmployees.length > 0){
      this.onSortByAscending('dateAt');
      this.sortDirection = this.sortDirectionEnum.Ascending;
      this.sortProperty = "dateAt";
    }
    
  }

  isAllowRoutingDetail() {
    return this.isGranted(PERMISSIONS_CONSTANT.Employee_EmployeeDetail);
  }
  isShowDeleteBtn(){
    return this.permission.isGranted(this.WarningEmployee_PlanQuitEmployee_Detele)
  }
  isShowEditBtn(){
    return this.permission.isGranted(this.WarningEmployee_PlanQuitEmployee_Edit)
  }

}
