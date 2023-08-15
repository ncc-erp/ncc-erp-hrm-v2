import { Component, Injector, OnInit } from '@angular/core';
import { PagedListingComponentBase, PagedRequestDto } from '@shared/paged-listing-component-base';
import { IssuedByDto } from '@app/service/model/categories/issuedBy.dto';
import { finalize } from 'rxjs/operators';
import { PERMISSIONS_CONSTANT } from '@app/permission/permission';
import { CreateEditIssuedByDialogComponent } from './create-edit-issued-by-dialog/create-edit-issued-by-dialog.component';
import { IssuedByService } from '@app/service/api/categories/issuedBy.service'

@Component({
  selector: 'app-issued-by',
  templateUrl: './issued-by.component.html',
  styleUrls: ['./issued-by.component.css']
})

export class IssuedByComponent extends PagedListingComponentBase<IssuedByDto> implements OnInit {

  public issuedByList: IssuedByDto[] = [];

  constructor(injector: Injector, private issuedByService: IssuedByService) {
    super(injector);
  }

  protected list(request: PagedRequestDto, pageNumber: number, finishedCallback: Function): void {
    this.subscription.push(
      this.issuedByService.getAllPagging(request).pipe(finalize(() => {
        finishedCallback()
      })).subscribe(rs => {
        this.issuedByList = rs.result.items
        this.showPaging(rs.result, pageNumber)
      })
    )
  }

  ngOnInit(): void {
    this.listBreadCrumb = [
      { name: '<i class="fa-solid fa-house fa-sm"></i>', url: '' },
      { name: ' <i class="fa-solid fa-chevron-right"></i> ' },
      { name: 'Issued By' }];
    this.refresh();
  }

  isShowCreateBtn() {
    return this.isGranted(PERMISSIONS_CONSTANT.Category_IssuedBy_Create);
  }

  isShowEditBtn() {
    return this.isGranted(PERMISSIONS_CONSTANT.Category_IssuedBy_Edit);
  }

  isShowDeleteBtn() {
    return this.isGranted(PERMISSIONS_CONSTANT.Category_IssuedBy_Delete);
  }

  onCreate() {
    this.openDialog(CreateEditIssuedByDialogComponent)
  }

  onUpdate(issuedBy: IssuedByDto) {
    this.openDialog(CreateEditIssuedByDialogComponent, { ...issuedBy })
  }

  onDelete(issuedBy: IssuedByDto) {
    this.confirmDelete(`Delete Issued By <strong>${issuedBy.name}</strong>`,
      () => this.issuedByService.delete(issuedBy.id).toPromise().then(rs => {
        abp.notify.success(`Deleted Issued By ${issuedBy.name}`)
      }))
  }
}