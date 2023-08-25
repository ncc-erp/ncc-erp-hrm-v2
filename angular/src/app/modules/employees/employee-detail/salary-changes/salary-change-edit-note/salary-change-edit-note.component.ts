import { Component, Injector, OnInit } from '@angular/core';
import { HistoryService } from '@app/service/api/history/history.service';
import { UpdateNoteInHistoryDto } from '@app/service/model/history/UpdateNoteInHistory.dto';
import { DialogComponentBase } from '@shared/dialog-component-base';

@Component({
  selector: 'app-salary-change-edit-note',
  templateUrl: './salary-change-edit-note.component.html',
  styleUrls: ['./salary-change-edit-note.component.css']
})
export class SalaryChangeEditNoteComponent extends DialogComponentBase<UpdateNoteInHistoryDto> implements OnInit {

  public salaryHistoryNote = {} as UpdateNoteInHistoryDto;
  constructor(injector: Injector,
    private historyService: HistoryService) {
    super(injector);
  }

  ngOnInit(): void {
    this.salaryHistoryNote = this.dialogData
  }

  onSave() {
    this.subscription.push(
      this.historyService.updateSalaryHistoryNote(this.salaryHistoryNote).subscribe(rs => {
        abp.notify.success(`salary history updated successful`);
        this.dialogRef.close()
      })
    )
  }

  onClose() {
    this.dialogRef.close()
  }
}
