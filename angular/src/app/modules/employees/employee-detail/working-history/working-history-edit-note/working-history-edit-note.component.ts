import { Component, Injector, OnInit } from '@angular/core';
import { HistoryService } from '@app/service/api/history/history.service';
import { UpdateNoteInHistoryDto } from '@app/service/model/history/UpdateNoteInHistory.dto';
import { DialogComponentBase } from '@shared/dialog-component-base';

@Component({
  selector: 'app-working-history-edit-note',
  templateUrl: './working-history-edit-note.component.html',
  styleUrls: ['./working-history-edit-note.component.css']
})
export class WorkingHistoryEditNote extends DialogComponentBase<UpdateNoteInHistoryDto> implements OnInit {
  public statusHistoryNote = {} as UpdateNoteInHistoryDto;
  constructor(injector: Injector,
    private historyService: HistoryService) {
    super(injector);
  }

  ngOnInit(): void {
    this.statusHistoryNote = this.dialogData
  }

  onSave() {
    this.subscription.push(
      this.historyService.updateWorkingHistoryNote(this.statusHistoryNote).subscribe(rs => {
        abp.notify.success(`Working history updated`);
        this.dialogRef.close()
      })
    )
  }

  onClose() {
    this.dialogRef.close()
  }

}
