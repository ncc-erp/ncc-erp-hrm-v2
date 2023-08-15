import { Component, Injector, OnInit } from '@angular/core';
import { PagedListingComponentBase, PagedRequestDto } from '@shared/paged-listing-component-base';
import { UserTypeDto } from '@app/service/model/categories/userType.dto'
import { UserTypeService } from '@app/service/api/categories/userType.service'
import { CreateEditUserTypeDialogComponent } from './create-edit-user-type-dialog/create-edit-user-type-dialog.component';
import { AppComponentBase } from '@shared/app-component-base';
@Component({
  selector: 'app-user-types',
  templateUrl: './user-types.component.html',
  styleUrls: ['./user-types.component.css']
})
export class UserTypesComponent extends PagedListingComponentBase<any> implements OnInit {
  protected list(request: PagedRequestDto, pageNumber: number, finishedCallback: Function): void {
    throw new Error('Method not implemented.');
  }
  public userTypeList: UserTypeDto[] = []
    ;

  constructor(injector: Injector, private userTypeService: UserTypeService) {
    super(injector)
  }

  ngOnInit(): void {
    this.listBreadCrumb = [
      { name: '<i class="fa-solid fa-house fa-sm"></i>', url: '' },
      { name: ' <i class="fa-solid fa-chevron-right"></i> ' },
      { name: 'User Types' }]
      this.getAllUserType()
  }

  onCreate() {
    this.dialog.open(CreateEditUserTypeDialogComponent)
  }

  onUpdate(userType: UserTypeDto) {
    this.openDialog(CreateEditUserTypeDialogComponent, { ...userType })
  }

  onDelete(userType: UserTypeDto) {
    this.confirmDelete(`Delete user type <strong>${userType.name}`,
      () => this.userTypeService.delete(userType.id).subscribe(() => {
        abp.notify.success(`Deleted user type ${userType.name}`)
        this.refresh()
      }))
  }

  getAllUserType() {
    this.subscription.push(this.userTypeService.getAll().subscribe(rs => {
      this.userTypeList = rs.result
    }))
  }
}
