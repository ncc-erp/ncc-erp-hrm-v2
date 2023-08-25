import { Component, OnInit, Inject, Injector } from '@angular/core';
import { RoleDto, UserDto, UserServiceProxy } from '@shared/service-proxies/service-proxies';
import { forEach as _forEach, includes as _includes, map as _map } from 'lodash-es';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import {UserService} from 'app/service/api/user/user.service';
import { DialogComponentBase } from '@shared/dialog-component-base';
@Component({
  selector: 'app-edit-user-role',
  templateUrl: './edit-user-role.component.html',
  styleUrls: ['./edit-user-role.component.css']
})
export class EditUserRoleComponent extends DialogComponentBase<EditUserRoleComponent> implements OnInit {
    roles: RoleDto[] = [];
    checkedRolesMap: { [key: string]: boolean } = {};
    user = {} as UserDto
  
    constructor(
      @Inject(MAT_DIALOG_DATA) public data: any, 
      public dialogRef: MatDialogRef<EditUserRoleComponent>,
      private userServiceProxy: UserServiceProxy,
      private userService : UserService,
      injector: Injector) {
      super(injector);
    }
  
    ngOnInit(): void {
      this.title = `Update role for user: ${this.data.fullName}`
      this.user.roleNames = this.data.roleNames
        this.userServiceProxy.getRoles().subscribe((result) => {
          this.roles = result.items;
          this.setInitialRolesStatus();
        })
      
    }
    setInitialRolesStatus(): void {
      _map(this.roles, (item) => {
        this.checkedRolesMap[item.normalizedName] = this.isRoleChecked(
          item.normalizedName
        );
      });
    }
    isRoleChecked(normalizedName: string): boolean {
      return _includes(this.user.roleNames, normalizedName);
    }
  
    onRoleChange(role: RoleDto, $event) {
      this.checkedRolesMap[role.normalizedName] = $event.target.checked;
    }
  
    getCheckedRoles(): string[] {
      const roles: string[] = [];
      _forEach(this.checkedRolesMap, function (value, key) {
        if (value) {
          roles.push(key);
        }
      });
      return roles;
    }

    saveAndClose() {
      this.user.userId = this.data.id
      this.user.roleNames = this.getCheckedRoles();
      this.isLoading = true;
        this.userService.UpdateUserRole(this.user).subscribe(rs => {
          abp.notify.success("Updated user role")
          this.isLoading = false;
          this.dialogRef.close(true);
        } , ()=> this.isLoading = false);
    }
  }
  