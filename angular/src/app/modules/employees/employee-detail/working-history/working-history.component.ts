import { Component, Injector, OnInit } from '@angular/core';
import { GetEmployeeWorkingHistoryDto } from '@app/service/model/working-history/GetEmployeeWorkingHistoryDto'
import { AppComponentBase } from '@shared/app-component-base';
import { ActivatedRoute } from '@angular/router';
import { EmployeeWorkingHistoryDto } from '@app/service/model/history/EmployeeWorkingHistory.dto';
import { HistoryService } from '@app/service/api/history/history.service';
import { UpdateNoteInHistoryDto } from '@app/service/model/history/UpdateNoteInHistory.dto';
import { MatDialog } from '@angular/material/dialog';
import { WorkingHistoryEditNote } from './working-history-edit-note/working-history-edit-note.component';
import { PERMISSIONS_CONSTANT } from '@app/permission/permission';
import { finalize } from 'rxjs/operators';
import { WorkingHistoryEditDateComponent } from './working-history-edit-date/working-history-edit-date.component';
@Component({
  selector: 'app-working-history',
  templateUrl: './working-history.component.html',
  styleUrls: ['./working-history.component.css']
})
export class WorkingHistoryComponent extends AppComponentBase implements OnInit {
  public listWorkingHistory: EmployeeWorkingHistoryDto[] = []
  public employeeId: number
  constructor(
    injector: Injector,
    private historyService: HistoryService,
    private activatedRoute: ActivatedRoute,
    public dialog : MatDialog
  ) {
    super(injector)
  }

  ngOnInit(): void {
    this.employeeId = this.activatedRoute.snapshot.queryParams['id']
    this.getWorkingHistoriesOfEmployee(this.employeeId)
  }

  onEdit(item) {
    this.dialog.open(
      WorkingHistoryEditNote,
      {
        width: '600px',data: {note: item.note, 
          id: item.id}
      }).afterClosed().subscribe((data)=>{
        this.getWorkingHistoriesOfEmployee(this.employeeId)
      })
  }

  onDelete(workingHistoryId: number) {
    abp.message.confirm(`Delete working history`, "", (result) => {
      if (result) {
        this.subscription.push(
          this.historyService.deleteWorkingHistory(workingHistoryId, this.employeeId).subscribe(rs => {
            abp.notify.success(`History deleted`)
            this.getWorkingHistoriesOfEmployee(this.employeeId)
          })
        )
      }
    })
  }

  getWorkingHistoriesOfEmployee(employeeId: number) {
    this.isLoading = true;
    this.subscription.push(
      this.historyService.getAllEmployeeWorkingHistory(employeeId)
      .pipe(finalize(() => {
        this.isLoading = false;
      }))
      .subscribe(rs => {
        this.listWorkingHistory = rs.result
      }))
  }
  onEditDate(item) {
    this.dialog.open(
      WorkingHistoryEditDateComponent,
      {
        width: '600px',
        data: {
          dateAt: item.dateAt, 
          id: item.id
        }
      }).afterClosed().subscribe(()=>{
        this.getWorkingHistoriesOfEmployee(this.employeeId)
      })
  }

  isShowEditNoteBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Employee_EmployeeDetail_TabWorkingHistory_EditNote);
  }
  isShowEditDateBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Employee_EmployeeDetail_TabWorkingHistory_EditDate);
  }

  isShowDeleteBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Employee_EmployeeDetail_TabWorkingHistory_Delete);
  }
}
