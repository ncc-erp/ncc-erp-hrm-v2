import { Component, Injector, OnInit, ViewChild } from '@angular/core';
import { GetRequestEmployeeDto } from '@app/service/model/salary-change-request/GetRequestEmployee';
import { ESalaryChangeRequestStatus, SalaryChangeRequestStatusList } from '@app/service/model/salary-change-request/GetSalaryChangeRequestDto';
import { PagedListingComponentBase, PagedRequestDto } from '@shared/paged-listing-component-base';
import { SelectOptionDto } from '@shared/dto/selectOptionDto';
import { HistoryService } from '@app/service/api/history/history.service';
import { EmployeeSalaryHistoryDto } from '@app/service/model/history/EmployeeSalaryHistory.dto';
import { UpdateNoteInHistoryDto } from '@app/service/model/history/UpdateNoteInHistory.dto';
import { AppComponentBase } from '@shared/app-component-base';
import { ActivatedRoute } from '@angular/router';
import { MatDialog } from '@angular/material/dialog';
import { SalaryChangeEditNoteComponent } from './salary-change-edit-note/salary-change-edit-note.component';
import { PERMISSIONS_CONSTANT } from '@app/permission/permission';
import { MatMenuTrigger } from '@angular/material/menu';
import { SalaryChangesEditDialogComponent } from './salary-changes-edit-dialog/salary-changes-edit-dialog.component';
import { finalize } from 'rxjs/operators';
@Component({
  selector: 'app-salary-changes',
  templateUrl: './salary-changes.component.html',
  styleUrls: ['./salary-changes.component.css']
})
export class SalaryChangesComponent extends AppComponentBase implements OnInit {
  @ViewChild(MatMenuTrigger)
  public menu: MatMenuTrigger;
  public contextMenuPosition = { x: '0px', y: '0px' };
  public employeeId: number
  public listSalaryHistory: EmployeeSalaryHistoryDto[] = []
  public FILTER_REQUEST_STATUS: SelectOptionDto[] = Object.entries(SalaryChangeRequestStatusList).map(x => ({ key: x[1].key, value: x[1].value }))
  public LIST_REQUEST_STATUS = SalaryChangeRequestStatusList
  public ESalaryChangeRequestStatus = ESalaryChangeRequestStatus
  public DefaultFilterValue = ESalaryChangeRequestStatus.Executed


  constructor(injector: Injector,
    private activatedRoute: ActivatedRoute,
    private historyService: HistoryService,
    public dialog : MatDialog
     ) {
    super(injector);
  }

  ngOnInit(): void {
    this.employeeId = this.activatedRoute.snapshot.queryParams["id"];
    this.getAllSalaryHistory()
  }

  delete(id: number) {
    abp.message.confirm(`Delete salary history`, "", (result) => {
      if (result) {
        this.subscription.push(
          this.historyService.deleteSalaryHistory(id).subscribe(rs => {
            this.notify.success("Employee's salary request deleted")
            this.getAllSalaryHistory()
          })
        )
      }
    })  
  }

  onEditNote(history) {
    this.dialog.open(
      SalaryChangeEditNoteComponent,
      {
        width: '600px',
        data: 
        {
          note: history.note, 
          id: history.id
        }
      }).afterClosed().subscribe((data)=>{
        this.getAllSalaryHistory();
      })
  }


  public getAllSalaryHistory(){
    this.isLoading = true;
    this.subscription.push(
      this.historyService.getAllEmployeeSalaryHistory(this.employeeId)
      .pipe(finalize(() => {
        this.isLoading = false;
      }))
      .subscribe(rs => {
        this.listSalaryHistory = rs.result
      },)
    )
  }

  isShowEditBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Employee_EmployeeDetail_TabSalaryHistory_EditNote);
  }

  isShowDeleteBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Employee_EmployeeDetail_TabSalaryHistory_Delete);
  }

  isShowEditDialogBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Employee_EmployeeDetail_TabSalaryHistory_Edit);
  }

  isShowForceDeleteBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Employee_EmployeeDetail_TabSalaryHistory_ForceDelete);
  }

  onEditHistory(history: EmployeeSalaryHistoryDto){
    const dialogRef = this.dialog.open(SalaryChangesEditDialogComponent, {
      width: '600px',
      data: history,
      panelClass: 'salary-history-dialog',
    })
    dialogRef.afterClosed().subscribe(rs => {
      this.getAllSalaryHistory()
    })
  }

  onForceDelete(history: EmployeeSalaryHistoryDto) {
    abp.message.confirm("Force delete salary history", "", (rs) => {
      if(rs) {
        this.historyService.deleteSalaryHistory(history.id).subscribe(rs => {
          abp.notify.success("Salary history deleted")
        })
      }
    })
  }
}
