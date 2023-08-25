import { Component, Injector, OnInit } from '@angular/core';
import { HistoryService } from '@app/service/api/history/history.service';
import { UpdateNoteInHistoryDto } from '@app/service/model/history/UpdateNoteInHistory.dto';
import { DialogComponentBase } from '@shared/dialog-component-base';

@Component({
  selector: 'app-branch-history-edit-note',
  templateUrl: './branch-history-edit-note.component.html',
  styleUrls: ['./branch-history-edit-note.component.css']
})
export class BranchHistoryEditNoteComponent extends DialogComponentBase<UpdateNoteInHistoryDto> implements OnInit {

  public branchHistoryNote = {} as UpdateNoteInHistoryDto;
  constructor(injector: Injector,
    private historyService: HistoryService) {
    super(injector);
  }

  ngOnInit(): void {
    this.branchHistoryNote = this.dialogData
  }

  onSave() {
    this.subscription.push(
      this.historyService.updateBranchHistoryNote(this.branchHistoryNote).subscribe(rs => {
        abp.notify.success(`Branch history updated`);
        this.dialogRef.close()
      })
    )
  }

  onClose() {
    this.dialogRef.close()
  }

}
