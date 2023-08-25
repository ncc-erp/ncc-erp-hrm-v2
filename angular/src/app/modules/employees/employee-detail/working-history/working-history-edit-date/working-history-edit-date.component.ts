import { Component, Injector, OnInit } from '@angular/core';
import { HistoryService } from '@app/service/api/history/history.service';
import { DialogComponentBase } from '@shared/dialog-component-base';
import * as moment from 'moment';

@Component({
  selector: 'app-working-history-edit-date',
  templateUrl: './working-history-edit-date.component.html',
  styleUrls: ['./working-history-edit-date.component.css']
})
export class WorkingHistoryEditDateComponent extends DialogComponentBase<UpdateDateInHistoryDto> implements OnInit {
  public statusHistoryDate = {} as UpdateDateInHistoryDto;
  constructor(injector: Injector,
    private historyService: HistoryService) {
    super(injector);
  }

  ngOnInit(): void {
    this.statusHistoryDate = this.dialogData;
    console.log(this.statusHistoryDate);
  }

  onSave() {
    this.statusHistoryDate.dateAt = this.formatDateYMD(this.statusHistoryDate.dateAt);
    this.isLoading = true;
    this.subscription.push(
      this.historyService.updateWorkingHistoryDate(this.statusHistoryDate).subscribe(rs => {
        abp.notify.success(`Working history update date successful`);
        this.isLoading = false;
        this.dialogRef.close();
      },()=> this.isLoading = false)
    )
  }

  onClose() {
    this.dialogRef.close()
  }

}

export interface UpdateDateInHistoryDto{
  id: number,
  dateAt: string,
}   
