import { Component, Injector, OnInit } from '@angular/core';
import { DialogComponentBase } from '@shared/dialog-component-base';
import { JobPositionDto } from '@app/service/model/categories/jobPosition.dto';
import { JobPositionService } from '@app/service/api/categories/jobPosition.service';
import { startWithTap } from '@shared/helpers/observerHelper';
import { finalize } from 'rxjs/operators'
@Component({
  selector: 'app-create-edit-job-position-dialog',
  templateUrl: './create-edit-job-position-dialog.component.html',
  styleUrls: ['./create-edit-job-position-dialog.component.css']
})
export class CreateEditJobPositionDialogComponent extends DialogComponentBase<JobPositionDto> implements OnInit {
  public jobPosition = {} as JobPositionDto;
  constructor(injector: Injector, private jobPositionService: JobPositionService) {
    super(injector)
  }

  ngOnInit(): void {
    if (this.dialogData?.id) {
      this.jobPosition = this.dialogData
      this.title = `Edit job position <strong>${this.jobPosition.name}</strong>`
    } else {
      this.jobPosition.color = this.APP_CONST.defaultBadgeColor;
      this.title = `Create new job position`
    }
  }
  nameChange(event){
    if(!this.dialogData?.id){
      this.jobPosition.shortName = event
      this.jobPosition.code = event
    }
  }
  saveAndClose() {
    this.trimData(this.jobPosition)
    if (this.dialogData?.id) {
      this.subscription.push(
        this.jobPositionService.update(this.jobPosition)
        .pipe(startWithTap(() => { this.isLoading = true }))
        .pipe(finalize(() => { this.isLoading = false }))
        .subscribe(() => {
          abp.notify.success("Update job position successfull")
          this.dialogRef.close(true)
        })
      )
    } else {
      this.subscription.push(
        this.jobPositionService.create(this.jobPosition)
        .pipe(startWithTap(() => { this.isLoading = true }))
        .pipe(finalize(() => { this.isLoading = false }))
        .subscribe(() => {
          abp.notify.success("Created new job position")
          this.dialogRef.close(true)
        })
      )
    }
  }
}
