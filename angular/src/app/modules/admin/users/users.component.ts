import { appModuleAnimation } from '@shared/animations/routerTransition';
import { PagedListingComponentBase, PagedRequestDto } from './../../../../shared/paged-listing-component-base';
import { Component, Injector } from '@angular/core';
import { finalize } from 'rxjs/operators';
import { BsModalService, BsModalRef } from 'ngx-bootstrap/modal';


import { CreateUserDialogComponent } from './create-user/create-user-dialog.component';
import { EditUserDialogComponent } from './edit-user/edit-user-dialog.component';
import { ResetPasswordDialogComponent } from './reset-password/reset-password.component';
import { UserDto, UserDtoPagedResultDto, UserServiceProxy } from '../../../../shared/service-proxies/service-proxies';
import {PERMISSIONS_CONSTANT} from '../../../permission/permission'
import { MatMenuTrigger } from '@angular/material/menu';
import { EditUserRoleComponent } from './edit-user-role/edit-user-role.component';
class PagedUsersRequestDto extends PagedRequestDto {
  keyword: string;
  isActive: boolean | null;
}

@Component({
  templateUrl: './users.component.html',
  animations: [appModuleAnimation()]
})
export class UsersComponent extends PagedListingComponentBase<UserDto> {
  menu: MatMenuTrigger;
  contextMenuPosition = { x: '0px', y: '0px' };
  users: UserDto[] = [];
  keyword = '';
  isActive: boolean | null;
  advancedFiltersVisible = false;

  Admin_User_Create = PERMISSIONS_CONSTANT.Admin_User_Create
  Admin_User_Edit = PERMISSIONS_CONSTANT.Admin_User_Edit
  Admin_User_Delete = PERMISSIONS_CONSTANT.Admin_User_Delete
  Admin_User_ResetPassword = PERMISSIONS_CONSTANT.Admin_User_ResetPassword
  constructor(
    injector: Injector,
    private _userService: UserServiceProxy,
    private _modalService: BsModalService
  ) {
    super(injector);
  }
  ngOnInit(): void {
    this.listBreadCrumb = [
      {name: '<i class="fa-solid fa-house fa-sm"></i>',url:''}, 
      {name: ' <i class="fa-solid fa-chevron-right"></i> '}, 
      {name:'Users'}];
      this.refresh()
  }

  isShowCreateBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Admin_User_Create);
  }
  isShowEditBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Admin_User_Edit);
  }
  isShowDeleteBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Admin_User_Delete);
  }
  isShowResetPasswordBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Admin_User_ResetPassword);
  }
  isShowEditUserRoleBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Admin_User_EditUserRole)
  }


  createUser(): void {
    this.showCreateOrEditUserDialog();
  }

  editUser(user: UserDto): void {
    this.showCreateOrEditUserDialog(user.id);
  }

  public resetPassword(user: UserDto): void {
    this.showResetPasswordUserDialog(user);
  }

  public editUserRole(user: UserDto): void {
    this.showEditUserRoleDialog(user);
  }

  clearFilters(): void {
    this.keyword = '';
    this.isActive = undefined;
    this.getDataPage(1);
  }

  protected list(
    request: PagedUsersRequestDto,
    pageNumber: number,
    finishedCallback: Function
  ): void {
    request.keyword = this.keyword;
    request.isActive = this.isActive;

    this._userService
      .getAll(
        request.keyword,
        request.isActive,
        request.skipCount,
        request.maxResultCount
      )
      .pipe(
        finalize(() => {
          finishedCallback();
        })
      )
      .subscribe((result: UserDtoPagedResultDto) => {
        this.users = result.items;
        this.showPaging(result, pageNumber);
      });
  }

  onDelete(user: UserDto): void {
    abp.message.confirm(
      this.l('UserDeleteWarningMessage', user.fullName),
      undefined,
      (result: boolean) => {
        if (result) {
          this._userService.delete(user.id).subscribe(() => {
            abp.notify.success(this.l('SuccessfullyDeleted'));
            this.refresh();
          });
        }
      }
    );
  }

  private showResetPasswordUserDialog(user: UserDto): void {
    this.dialog.open(ResetPasswordDialogComponent,{
      data:{
        id: user.id,
        fullName : user.fullName
      },
      width:"700px"
    })
  }

  private showCreateOrEditUserDialog(id?: number): void {
    let createOrEditUserDialog: BsModalRef;
    if (!id) {
      createOrEditUserDialog = this._modalService.show(
        CreateUserDialogComponent,
        {
          class: 'modal-lg',
        }
      );
    } else {
      createOrEditUserDialog = this._modalService.show(
        EditUserDialogComponent,
        {
          class: 'modal-lg',
          initialState: {
            id: id,
          },
        }
      );
    }

    createOrEditUserDialog.content.onSave.subscribe(() => {
      this.refresh();
    });
  }

  private showEditUserRoleDialog(user: UserDto){
    let modal = this.dialog.open(EditUserRoleComponent,
      {
        data: user,
        width: '700px'
      });
      modal.afterClosed().subscribe(rs => {
        if (rs) {
          this.refresh()
        }
      })
  }
}
