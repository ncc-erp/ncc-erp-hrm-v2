import { Component, Injector, OnInit } from '@angular/core';
import { SkillService } from '@app/service/api/categories/skill.service';
import { SkillDto } from '@app/service/model/categories/skill.dto';
import { DialogComponentBase } from '@shared/dialog-component-base';

@Component({
  selector: 'app-create-edit-skill-dialog',
  templateUrl: './create-edit-skill-dialog.component.html',
  styleUrls: ['./create-edit-skill-dialog.component.css']
})
export class CreateEditSkillDialogComponent extends DialogComponentBase<SkillDto> implements OnInit {
  public skill = {} as SkillDto
  constructor(injector: Injector, private skillService: SkillService) {
    super(injector)
  }

  ngOnInit(): void {
    if (this.dialogData?.id) {
      this.skill = this.dialogData
      this.title = `Edit skill <strong>${this.skill.name}</strong>`
    } else {
      this.title = `Create new skill`
    }
  }

  nameChange(event){
    if(!this.dialogData?.id){
      this.skill.code = event
    }
  }

  saveAndClose() {
    this.trimData(this.skill)
    if (this.dialogData?.id) {
      this.subscription.push(
        this.skillService.update(this.skill).subscribe(() => {
          abp.notify.success("Update skill successfully")
          this.dialogRef.close(true)
        })
      )
    } else {
      this.subscription.push(
        this.skillService.create(this.skill).subscribe(() => {
          abp.notify.success("Created new skill")
          this.dialogRef.close(true)
        })
      )
    }
  }


}
