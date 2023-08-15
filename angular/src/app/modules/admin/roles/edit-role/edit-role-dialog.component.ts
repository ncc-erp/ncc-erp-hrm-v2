import { AddUserInRoleComponent } from './add-user-in-role/add-user-in-role.component';
import { MatDialog } from '@angular/material/dialog';
import {
  Component,
  Injector,
  OnInit,
  EventEmitter,
  Output,
  ChangeDetectorRef,
} from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';
import {
  RoleServiceProxy,
  RoleDto,
  PermissionDto,
  Permission,
  RolePermissionDto
} from '@shared/service-proxies/service-proxies';
import { SelectionModel } from '@angular/cdk/collections';
import { NestedTreeControl } from '@angular/cdk/tree';
import { MatTreeNestedDataSource } from '@angular/material/tree';
import { ActivatedRoute } from '@angular/router';
import * as _ from 'lodash';
import { AppConsts } from '@shared/AppConsts';

@Component({
  templateUrl: 'edit-role-dialog.component.html',
  styleUrls: ['edit-role-dialog.component.css']
})
export class EditRoleDialogComponent extends AppComponentBase implements OnInit {
  selection = new SelectionModel<string>(true, []);
  treeControl: NestedTreeControl<any>;
  dataSource: MatTreeNestedDataSource<any>;
  isStatic: boolean;
  path = '';
  saving = false;
  roleId: number;
  role = new RoleDto();
  id: any;
  title: string;
  roles = [];
  listRoles = [];
  public listUsers: any[] = []
  selectedRole: any;
  keyword: string = '';
  public isCreated: boolean = false
  public searchUser: string = ''
  public userId: number = 0
  public listUsersNotInRole: any[] = []
  listUsersForSearch: any[] = []
  public searchMemberText = '';
  checkedPermissionsMap: { [key: string]: boolean } = {};
  defaultPermissionCheckedStatus = true;
  permissions: RolePermissionDto = new RolePermissionDto();
  grantedPermissionNames: string[];

  @Output() onSave = new EventEmitter<any>();
  constructor(
    injector: Injector,
    private _roleService: RoleServiceProxy,
    private _route: ActivatedRoute,
    private ref: ChangeDetectorRef,
    private dialog: MatDialog
  ) {
    super(injector);
    this.dataSource = new MatTreeNestedDataSource<Permission>();
    this.treeControl = new NestedTreeControl<Permission>(node => node.childrens);
  }
  hasChild = (_: number, node: Permission) => !!node.childrens && node.childrens.length > 0;

  ngOnInit() {
    this.id = this._route.snapshot.queryParamMap.get('id');
    this.title = this._route.snapshot.queryParamMap.get('name')
    this._roleService
      .getRoleForEdit(this.id)
      .subscribe((result: any) => {
        this.role = result.result.role;
        this.isStatic = result.result.role.isStatic;
        this.grantedPermissionNames = result.result.grantedPermissionNames;
        this.dataSource.data = result.result.permissions;
        this.treeControl.dataNodes = result.result.permissions;
        this.treeControl.dataNodes.forEach(node =>
          this.initSelectionList(node)
        );
      });
    this.getAllUserInRole()
    this.getAllUserNotInRole()
  }

  ngAfterViewChecked(): void {
    this.ref.detectChanges()
  }

  isPermissionChecked(permissionName: string): boolean {
    return this.defaultPermissionCheckedStatus;
  }

  onPermissionChange(permission: PermissionDto, $event) {
    this.checkedPermissionsMap[permission.name] = $event.target.checked;
  }

  getCheckedPermissions(): string[] {
    const permissions: string[] = [];
    _.forEach(this.checkedPermissionsMap, function (value, key) {
      if (value) {
        permissions.push(key);
      }
    });
    return permissions;
  }

  initSelectionList(node: Permission) {
    const selectedList = this.grantedPermissionNames as any;
    if (selectedList.includes(node.name)) {
      this.selected(node);
    }

    if (!node.childrens || node.childrens.length === 0) {
      return;
    } else {
      node.childrens.forEach(child => this.initSelectionList(child));
    }
  }
  isSelected(node: Permission) {
    return this.selection.isSelected(node.name);
  }

  deselected(node: Permission) {
    this.selection.deselect(node.name);
  }
  selected(node: Permission) {
    this.selection.select(node.name);
  }

  descendantsAllSelected(node: Permission): boolean {
    const descendants = this.treeControl.getDescendants(node);
    const descAllSelected = descendants.every(child => this.isSelected(child));
    descAllSelected ? this.selected(node) : this.deselected(node);
    return descAllSelected;
  }

  descendantsPartiallySelected(node: Permission): boolean {
    const descendants = this.treeControl.getDescendants(node);
    const result = descendants.some(child => this.isSelected(child));
    return result && !this.descendantsAllSelected(node);
  }

  todoLeafItemSelectionToggle(node: Permission) {
    this.isSelected(node) ? this.deselected(node) : this.selected(node);
    this.descendantsPartiallySelected(node);
    this.onSaveData(node);
  }

  todoItemSelectionToggle(node: Permission) {
    this.isSelected(node) ? this.deselected(node) : this.selected(node);
    const descendants = this.treeControl.getDescendants(node);
    descendants.forEach(child => {
      this.isSelected(node) ? this.selected(child) : this.deselected(child);
    });
    this.onSaveData(node);
  }

  // Trước khi gọi API
  // Cho những node có trạng thái indeterminate vào list.
  selectOrDeselectAllIndeterminateParents(doSelect: boolean) {
    this.treeControl.dataNodes.forEach(
      parent => { this.selectOrDeselectTheIndeterminateParent(parent, doSelect); }
    );
  }

  selectOrDeselectTheIndeterminateParent(node: Permission, doSelect: boolean) {
    if (!node.childrens || node.childrens.length < 1) { return; }
    const descendants = this.treeControl.getDescendants(node);
    const descSomeSelected = descendants.some(child => this.isSelected(child));

    if (descSomeSelected) {
      if (doSelect) {
        this.selected(node);
      } else if (!this.descendantsAllSelected(node)) {
        this.deselected(node);
      }
    }

    descendants.forEach(
      child => this.selectOrDeselectTheIndeterminateParent(child, doSelect)
    );
  }

  onBack() {
    history.back();
  }

  onSaveClick() {
    this._roleService
      .update(this.role)
      .subscribe(() => {
        this.notify.success(this.l('Saved successfully'));
        this.onSave.emit()

      });
  }

  onCancelClick() {
    history.back();
  }

  onSaveData(node: Permission) {
    node.isLoading = true;
    let descendants = this.treeControl.getDescendants(node);
    if (descendants) {
      descendants.forEach(child => child.isLoading = true);
    }
    this.selectOrDeselectAllIndeterminateParents(true);
    this.permissions.permissions = this.selection.selected;
    this.permissions.id = this.role.id;
    this._roleService.changeRolePermission(this.permissions).subscribe(() => {
      node.isLoading = false
      if (descendants) {
        descendants.forEach(child => child.isLoading = false);
      }
    });
  }
  public getAllUserInRole() {
    this._roleService.getAllUserInRole(this.id).subscribe(res => {
      this.listUsersForSearch = this.listUsers = res.result
    });
  }

  public removeMemberFromProject(id, fullName) {
    abp.message.confirm(
      "Remove " + fullName + " From Out Role?",
      "",
      (result: boolean) => {
        if (result) {
          this._roleService.removeUserFromOutRole(id).subscribe((res) => {
            if (res.success) {
              abp.notify.success(res.result)
              let index = this.listUsersForSearch.findIndex(x => x.id == id);
              if (index >= 0) {
                this.listUsersForSearch.splice(index, 1);
              }
              this.searchMember()
              this.getAllUserNotInRole()
            }
            else {
              abp.notify.error(res.result)
            }
          })
        }
      }
    );

  }

  public searchMember() {
    this.listUsers = this.listUsersForSearch.filter(
      member =>
        (this.searchMemberText ?
          (member.fullName.toLowerCase().includes(this.searchMemberText.toLowerCase()) ||
            member.email.toLowerCase().includes(this.searchMemberText.toLowerCase())
          ) : true)
    );
  }
  keyPressed(event) {
    if (event.keyCode == 13) {
      event.preventDefault();
      return false;
    }
  }

  focusOut() {
    this.searchUser = "";
  }


  getAllUserNotInRole() {
    this._roleService.getAllUserNotInRole(this.id).subscribe(res => {
      this.listUsersNotInRole = res.result
    })
  }

  addUserInRole() {
    const show = this.dialog.open(AddUserInRoleComponent, {
      data: {
        roleId: this.id
      },
      width: '500px'
    })
    show.afterClosed().subscribe(result => {
      if (result) {
        this.getAllUserInRole();
      }
    });
  }
}
