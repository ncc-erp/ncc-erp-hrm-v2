import { Component, Injector, OnInit } from '@angular/core';
import { EmployeeBranchHistory } from '@app/service/model/history/EmployeeBranchHistory.dto'
import { AppComponentBase } from '@shared/app-component-base';
import { HistoryService } from '@app/service/api/history/history.service'
import { ActivatedRoute } from '@angular/router';
import { UpdateNoteInHistoryDto } from '@app/service/model/history/UpdateNoteInHistory.dto';
import { MatDialog } from '@angular/material/dialog';
import { BranchHistoryEditNoteComponent } from './branch-history-edit-note/branch-history-edit-note.component';
import { PERMISSIONS_CONSTANT } from '@app/permission/permission';
import { finalize } from 'rxjs/operators';
@Component({
  selector: 'app-branch-history',
  templateUrl: './branch-history.component.html',
  styleUrls: ['./branch-history.component.css']
})
export class BranchHistoryComponent extends AppComponentBase implements OnInit {
  public listBranchHistory: EmployeeBranchHistory[]
  public employeeId: number
  constructor(injector: Injector,
    private historyService: HistoryService,
    private activatedRoute: ActivatedRoute,
    public dialog : MatDialog) {
    super(injector)
  }

  ngOnInit(): void {
    this.employeeId = this.activatedRoute.snapshot.queryParams['id']
    this.getAllEmployeeBranchHistory(this.employeeId)
  }

  onEdit(item) {
    this.dialog.open(
      BranchHistoryEditNoteComponent,
      {
        width: '600px',data: {note: item.note, 
          id: item.id}
      }).afterClosed().subscribe((data)=>{
        this.getAllEmployeeBranchHistory(this.employeeId)
      })
  }

  onCancel(item) {
    this.getAllEmployeeBranchHistory(this.employeeId)
  }

  onSave(history: EmployeeBranchHistory) {
    const updateDto: UpdateNoteInHistoryDto = {
      id: history.id,
      note: history.note
    }
    this.subscription.push(
      this.historyService.updateBranchHistoryNote(updateDto).subscribe(rs => {
        abp.notify.success("Note updated")
        this.getAllEmployeeBranchHistory(this.employeeId)
      })
    )
  }

  delete(id: number) {
    abp.message.confirm("Delete branch history", "", (result) => {
      if (result) {
        this.subscription.push(
          this.historyService.deleteBranchHistory(id).subscribe(rs => {
            abp.notify.success("Branch history detelted")
            this.getAllEmployeeBranchHistory(this.employeeId)
          })
        )
      }
    })
  }

  getAllEmployeeBranchHistory(employeeId: number) {
    if(this.isAllowViewTabBranchHistory()){
      this.isLoading = true;
      this.subscription.push(
        this.historyService.getAllEmployeeBranchHistory(employeeId)
        .pipe(finalize(() => {
          this.isLoading = false;
        })).subscribe(rs => {
          this.listBranchHistory = rs.result
        })
      )
    }
  }

  isShowEditBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Employee_EmployeeDetail_TabBranchHistory_EditNote);
  }

  isShowDeleteBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Employee_EmployeeDetail_TabBranchHistory_Delete);
  }

  isAllowViewTabBranchHistory(){
    return this.isGranted(PERMISSIONS_CONSTANT.Employee_EmployeeDetail_TabBranchHistory_View);
  }

}
