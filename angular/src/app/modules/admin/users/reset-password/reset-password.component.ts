import { DialogComponentBase } from '@shared/dialog-component-base';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { Component, OnInit, Injector, Inject } from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';
import {
  UserServiceProxy,
  ResetPasswordDto,
  UserDto
} from '@shared/service-proxies/service-proxies';
import { BsModalRef } from 'ngx-bootstrap/modal';

@Component({
  selector: 'app-reset-password',
  templateUrl: './reset-password.component.html'
})
export class ResetPasswordDialogComponent extends DialogComponentBase<ResetPasswordDto>
  implements OnInit {
  public isLoading = false;
  public resetPasswordDto: ResetPasswordDto;
  public user = {} as UserDto;
  public isRandomPassword:boolean = false;
  public title: string;

  constructor(
    injector: Injector,
    private _userService: UserServiceProxy,
    @Inject(MAT_DIALOG_DATA) public data: any,
    public dialogRef: MatDialogRef<ResetPasswordDialogComponent>
  ) {
    super(injector);
  }

  ngOnInit() {
    this.user = this.data;
    this.title = `Reset password for: <strong>${this.user.fullName}</strong>`;
    this.isLoading = true;
    this.resetPasswordDto = new ResetPasswordDto();
    this.resetPasswordDto.userId = this.user.id;
    this.isLoading = false;
  }

  public randomPassword(){
    this.isRandomPassword = true;
    this.resetPasswordDto.newPassword = Math.random()
      .toString(36)
      .substr(2, 10);
  }

  public resetPassword(): void {
    this.isLoading = true;
    this._userService.resetPassword(this.resetPasswordDto).subscribe(() => {
      this.notify.success('Password reset successful');
      this.dialogRef.close(this.resetPassword);
    },()=>{this.isLoading = false});
  }
}
