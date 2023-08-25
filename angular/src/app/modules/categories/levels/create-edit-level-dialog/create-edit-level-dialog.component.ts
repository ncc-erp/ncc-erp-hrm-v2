import { Component, Injector, OnInit } from '@angular/core';
import { DialogComponentBase } from '@shared/dialog-component-base';
import { LevelDto } from '@app/service/model/categories/level.dto';
import { LevelService } from '@app/service/api/categories/level.service';
@Component({
  selector: 'app-create-edit-level-dialog',
  templateUrl: './create-edit-level-dialog.component.html',
  styleUrls: ['./create-edit-level-dialog.component.css']
})
export class CreateEditLevelDialogComponent extends DialogComponentBase<LevelDto> implements OnInit {
  public level = {} as LevelDto

  constructor(injector: Injector, private levelService: LevelService) {
    super(injector)
  }

  ngOnInit(): void {
    if (this.dialogData?.id) {
      this.level = this.dialogData
      this.title = `Edit level <strong>${this.level.name}</strong>`
    } else {
      this.level.color = this.APP_CONST.defaultBadgeColor
      this.title = `Create new level`
    }
  }
  nameChange(event){
    if(!this.dialogData?.id){
      this.level.shortName = event
      this.level.code = event
    }
  }
  saveAndClose() {
    this.trimData(this.level)
    if (this.dialogData?.id) {
      this.subscription.push(
        this.levelService.update(this.level).subscribe(() => {
          abp.notify.success(`Updated level ${this.level.name}`)
          this.dialogRef.close(true)
        })
      )
    } else {
      this.subscription.push(
        this.levelService.create(this.level).subscribe(() => {
          abp.notify.success("Created new level")
          this.dialogRef.close(true)
        })
      )
    }
  }
}
