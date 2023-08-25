import { Component, Injector, OnInit } from '@angular/core';
import { PunishmentTypeService } from '@app/service/api/categories/punishmentType.service';
import { PunishmentTypeDto } from '@app/service/model/categories/punishmentType.dto';
import { DialogComponentBase } from '@shared/dialog-component-base';

@Component({
  selector: 'app-create-edit-punishment-type-dialog',
  templateUrl: './create-edit-punishment-type-dialog.component.html',
  styleUrls: ['./create-edit-punishment-type-dialog.component.css']
})
export class CreateEditPunishmentTypeDialogComponent extends DialogComponentBase<PunishmentTypeDto> implements OnInit {
  public punishmentType = {} as PunishmentTypeDto
  constructor(injector: Injector, private punishmentTypeService: PunishmentTypeService) {
    super(injector);
  }

  ngOnInit(): void {
    if(this.dialogData?.id){
      this.punishmentType = this.dialogData
      this.title = `Edit Punishment Type <strong>${this.punishmentType.name}</strong>`
    } else {
      this.title = `Create new Punishment Type`
      this.punishmentType.isActive = true;
    }
  }
  
  saveAndClose(){
    this.trimData(this.punishmentType)
    if(this.dialogData?.id){
      this.subscription.push(
        this.punishmentTypeService.update(this.punishmentType).subscribe(()=>{
          abp.notify.success("Update punishment type successfull")
          this.dialogRef.close(true)
        })
      )
    } else {
      this.subscription.push(
        this.punishmentTypeService.create(this.punishmentType).subscribe(()=>{
          abp.notify.success("Created new punishment type")
          this.dialogRef.close(true)
        })
      )
    }
  }

}
