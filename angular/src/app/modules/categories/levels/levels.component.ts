import { Component, Injector, OnInit } from '@angular/core';
import { PagedListingComponentBase, PagedRequestDto } from '@shared/paged-listing-component-base';
import { LevelDto } from '@app/service/model/categories/level.dto'
import { LevelService } from '@app/service/api/categories/level.service'
import { finalize } from 'rxjs/operators';
import { CreateEditLevelDialogComponent } from './create-edit-level-dialog/create-edit-level-dialog.component';
import { PERMISSIONS_CONSTANT } from '@app/permission/permission';
@Component({
  selector: 'app-levels',
  templateUrl: './levels.component.html',
  styleUrls: ['./levels.component.css']
})
export class LevelsComponent extends PagedListingComponentBase<LevelDto> implements OnInit {
  public levelList: LevelDto[] = [];
  Category_Level_View = PERMISSIONS_CONSTANT.Category_Level_View
  Category_Level_Create = PERMISSIONS_CONSTANT.Category_Level_Create
  Category_Level_Edit = PERMISSIONS_CONSTANT.Category_Level_Edit
  Category_Level_Delete = PERMISSIONS_CONSTANT.Category_Level_Delete

  protected list(request: PagedRequestDto, pageNumber: number, finishedCallback: Function): void {
    this.subscription.push(
      this.levelService.getAllPagging(request)
        .pipe(finalize(() => {
          finishedCallback()
        })).subscribe(rs => {
          this.levelList = rs.result.items;
          this.showPaging(rs.result, pageNumber)
        })
    )
  }

  constructor(injector: Injector, private levelService: LevelService) {
    super(injector)
  }

  ngOnInit(): void {
    this.listBreadCrumb = [
      { name: '<i class="fa-solid fa-house fa-sm"></i>', url: '' },
      { name: ' <i class="fa-solid fa-chevron-right"></i> ' },
      { name: 'Levels' }],
      this.refresh()
  }

  isShowCreateBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Category_Level_Create);
  }
  isShowEditBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Category_Level_Edit);
  }
  isShowDeleteBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Category_Level_Delete);
  }

  onCreate() {
    this.openDialog(CreateEditLevelDialogComponent);
  }

  onUpdate(level: LevelDto) {
    this.openDialog(CreateEditLevelDialogComponent, { ...level })
  }

  onDelete(level: LevelDto) {
    this.confirmDelete(`Delete level <strong>${level.name}</strong>`, () => {
      this.levelService.delete(level.id).subscribe(() => {
        abp.notify.success(`Deleted level ${level.name}`)
        this.refresh();
      })
    })
  }
}
