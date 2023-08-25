import { Component, Injector, OnInit } from '@angular/core';
import { UserTypeService } from '@app/service/api/categories/userType.service';
import { UserTypeDto } from '@app/service/model/categories/userType.dto';
import { DialogComponentBase } from '@shared/dialog-component-base';

@Component({
  selector: 'app-create-edit-user-type-dialog',
  templateUrl: './create-edit-user-type-dialog.component.html',
  styleUrls: ['./create-edit-user-type-dialog.component.css']
})
export class CreateEditUserTypeDialogComponent extends DialogComponentBase<UserTypeDto> implements OnInit {
  public userType = {} as UserTypeDto
  constructor(injector: Injector, private userTypeService: UserTypeService) {
    super(injector)
  }

  ngOnInit(): void {
    if (this.dialogData?.id) {
      this.userType = this.dialogData;
      this.title = `Edit user type <strong>${this.dialogData.name}</strong>`
    } 
    else {
      this.userType.color = this.APP_CONST.defaultBadgeColor;
      this.title = "Create new user type";
    }
  }

  nameChange(event){
    if(!this.dialogData?.id){
      this.userType.shortName = event
      this.userType.code = event;
    }
  }

  saveAndClose(){
    if(this.dialogData?.id){
      this.trimData(this.userType);
      this.subscription.push(
        this.userTypeService.update(this.userType).subscribe(rs => {
          abp.notify.success(`Update user type successful`)
          this.dialogRef.close(true)
        })
      )
    } 
    else {
      this.trimData(this.userType);
      this.subscription.push(
        this.userTypeService.create(this.userType).subscribe(rs => {
          abp.notify.success(`Created new user type ${this.userType.name}`)
          this.dialogRef.close(true);
        })
      )
    }
  }

  
}
