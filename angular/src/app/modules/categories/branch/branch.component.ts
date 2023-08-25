import { BranchService } from '@app/service/api/categories/branch.service';
import { BranchDto } from '@app/service/model/categories/branch.dto';
import { Component, Injector } from '@angular/core';
import { PagedListingComponentBase, PagedRequestDto } from '@shared/paged-listing-component-base';
import { finalize } from 'rxjs/operators';
import { CreateEditBranchDialogComponent } from './create-edit-branch-dialog/create-edit-branch-dialog.component';
import { PERMISSIONS_CONSTANT } from '@app/permission/permission';

@Component({
  selector: 'app-branch',
  templateUrl: './branch.component.html',
  styleUrls: ['./branch.component.css']
})
export class BranchComponent extends PagedListingComponentBase<BranchDto> {
  public branchList: BranchDto[] = [];
  protected list(request: PagedRequestDto, pageNumber: number, finishedCallback: Function): void {
    this.subscription.push(
      this.branchService.getAllPagging(request)
        .pipe(finalize(() => {
          finishedCallback();
        }))
        .subscribe(rs => {
          this.branchList = rs.result.items
          this.showPaging(rs.result, pageNumber)
        })
    )
  }

  constructor(injector: Injector, private branchService: BranchService) {
    super(injector)
  }

  ngOnInit(): void {
    this.listBreadCrumb = [
      {name: '<i class="fa-solid fa-house fa-sm"></i>',url:''}, 
      {name: ' <i class="fa-solid fa-chevron-right"></i> '}, 
      {name:'Branches'}],
    this.refresh()
  }

  isShowCreateBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Category_Branch_Create);
  }
  isShowEditBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Category_Branch_Edit);
  }
  isShowDeleteBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Category_Branch_Delete);
  }

  onCreate() {
    this.openDialog(CreateEditBranchDialogComponent)
  }

  onUpdate(branch: BranchDto) {
    this.openDialog(CreateEditBranchDialogComponent, { ...branch })
  }

  onDelete(branch: BranchDto) {
    this.confirmDelete(`Delete branch <strong>${branch.name}</strong>`,
      () => this.branchService.delete(branch.id).toPromise().then(rs => {
        abp.notify.success(`Deleted branch ${branch.name}`)
      }))
  }

}
