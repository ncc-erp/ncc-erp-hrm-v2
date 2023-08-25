import { Component, Injector, OnInit } from '@angular/core';
import { PagedListingComponentBase, PagedRequestDto } from '@shared/paged-listing-component-base';
import { JobPositionDto } from '@app/service/model/categories/jobPosition.dto'
import { CreateEditJobPositionDialogComponent } from './create-edit-job-position-dialog/create-edit-job-position-dialog.component';
import { JobPositionService } from '@app/service/api/categories/jobPosition.service'
import { finalize } from 'rxjs/operators';
import { PERMISSIONS_CONSTANT } from '@app/permission/permission';
@Component({
  selector: 'app-job-positions',
  templateUrl: './job-positions.component.html',
  styleUrls: ['./job-positions.component.css']
})
export class JobPositionsComponent extends PagedListingComponentBase<JobPositionDto> implements OnInit {
  public jobPositionList: JobPositionDto[] = [];
  protected list(request: PagedRequestDto, pageNumber: number, finishedCallback: Function): void {
    this.subscription.push(
      this.jobPositionService.getAllPagging(request)
        .pipe(finalize(() => {
          finishedCallback()
        })).subscribe(rs => {
          this.jobPositionList = rs.result.items;
          this.showPaging(rs.result, pageNumber)
        })
    )
  }

  constructor(injector: Injector, private jobPositionService: JobPositionService) {
    super(injector)
  }

  ngOnInit(): void {
    this.listBreadCrumb = [
      {name: '<i class="fa-solid fa-house fa-sm"></i>',url:''}, 
      {name: ' <i class="fa-solid fa-chevron-right"></i> '}, 
      {name:'Job Position'}],
    this.refresh()
  }

  isShowCreateBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Category_JobPosition_Create);
  }
  isShowEditBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Category_JobPosition_Edit);
  }
  isShowDeleteBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Category_JobPosition_Delete);
  }

  onCreate() {
    this.openDialog(CreateEditJobPositionDialogComponent);
  }

  onUpdate(jobPosition: JobPositionDto) {
    this.openDialog(CreateEditJobPositionDialogComponent, { ...jobPosition })
  }

  onDelete(jobPosition: JobPositionDto) {
    this.confirmDelete(`Delete job position <strong>${jobPosition.name}</strong>`, () => {
      this.jobPositionService.delete(jobPosition.id).subscribe(() => {
        abp.notify.success(`Deleted job position ${jobPosition.name}`)
        this.refresh();
      })
    })
  }
}
