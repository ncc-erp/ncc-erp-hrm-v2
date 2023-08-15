import { Component, Injector, OnInit } from '@angular/core';
import { TeamService } from '@app/service/api/categories/team.service';
import { DialogComponentBase } from '@shared/dialog-component-base';
import { TeamDto } from '@app/service/model/categories/team.dto';
import { startWithTap } from '@shared/helpers/observerHelper';
import { finalize } from 'rxjs/operators'
@Component({
  selector: 'app-create-edit-team-dialog',
  templateUrl: './create-edit-team-dialog.component.html',
  styleUrls: ['./create-edit-team-dialog.component.css']
})
export class CreateEditTeamDialogComponent extends DialogComponentBase<TeamDto> implements OnInit {
  public team = {} as TeamDto
  constructor(injector: Injector, private teamService: TeamService) {
    super(injector)
  }

  ngOnInit(): void {
    if (this.dialogData?.id) {
      this.team = this.dialogData
      this.title = `Edit team <strong>${this.team.name}</strong>`
    } else {
      this.title = `Create new team`
    }
  }

  saveAndClose() {
    this.trimData(this.team)
    if (this.dialogData?.id) {
      this.subscription.push(
        this.teamService.update(this.team)
        .pipe(startWithTap(() => { this.isLoading = true}))
        .pipe(finalize(() => { this.isLoading = false}))
        .subscribe(() => {
          abp.notify.success("Update team successfully")
          this.dialogRef.close(true)
        })
      )
    } else {
      this.subscription.push(
        this.teamService.create(this.team)
        .pipe(startWithTap(() => { this.isLoading = true}))
        .pipe(finalize(() => { this.isLoading = false}))
        .subscribe(() => {
          abp.notify.success("Created new team")
          this.dialogRef.close(true)
        })
      )
    }
  }
}
