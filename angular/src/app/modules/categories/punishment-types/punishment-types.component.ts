import { Component, Injector, OnInit } from '@angular/core';
import { PagedListingComponentBase, PagedRequestDto } from '@shared/paged-listing-component-base';
import { PunishmentTypeDto } from '@app/service/model/categories/punishmentType.dto'
import { PunishmentTypeService } from '@app/service/api/categories/punishmentType.service'
import { CreateEditPunishmentTypeDialogComponent } from './create-edit-punishment-type-dialog/create-edit-punishment-type-dialog.component';
import { finalize } from 'rxjs/operators';
import { PERMISSIONS_CONSTANT } from '@app/permission/permission';
@Component({
  selector: 'app-punishment-types',
  templateUrl: './punishment-types.component.html',
  styleUrls: ['./punishment-types.component.css']
})
export class PunishmentTypesComponent extends PagedListingComponentBase<PunishmentTypeDto> implements OnInit {
  public punishmentTypeList: PunishmentTypeDto[] = []
  public DEFAULT_FILTER_VALUE: number
  public FIRST_PAGE = 1;
  public listBreadCrumb = [];
  public readonly filterList = [
    {
      key: 'All',
      value: this.APP_CONST.DEFAULT_ALL_FILTER_VALUE,
    },
    {
      key: 'Active',
      value: FILTER_VALUE.ACTIVE,
    },
    {
      key: 'Inactive',
      value: FILTER_VALUE.INACTIVE
    }
  ]

  protected list(request: PagedRequestDto, pageNumber: number, finishedCallback: Function): void {
    this.subscription.push(
      this.punishmentTypeService.getAllPagging(request)
        .pipe(finalize(() => {
          finishedCallback()
        })).subscribe(rs => {
          this.punishmentTypeList = rs.result.items;
          this.showPaging(rs.result, pageNumber)
        }))
  }

  constructor(injector: Injector, private punishmentTypeService: PunishmentTypeService) {
    super(injector)
  }

  ngOnInit(): void {
    this.listBreadCrumb = [
      {name: '<i class="fa-solid fa-house fa-sm"></i>',url:''}, 
      {name: ' <i class="fa-solid fa-chevron-right"></i> '}, 
      {name:'Punishment Types'}],
    this.DEFAULT_FILTER_VALUE = FILTER_VALUE.ACTIVE
    this.pageNumber = this.FIRST_PAGE;
    this.setDefaultFilter({propertyName: 'isActive',value: this.DEFAULT_FILTER_VALUE, comparision: 0})
    this.refresh()
  }


  isShowCreateBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Category_PunishmentType_Create);
  }
  isShowEditBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Category_PunishmentType_Edit);
  }
  isShowDeleteBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Category_PunishmentType_Delete);
  }

  onCreate() {
    this.openDialog(CreateEditPunishmentTypeDialogComponent);
  }

  onUpdate(punishmentType: PunishmentTypeDto) {
    this.openDialog(CreateEditPunishmentTypeDialogComponent, { ...punishmentType })
  }

  onDelete(punishmentType: PunishmentTypeDto) {
    this.confirmDelete(`Delete punishmentType <strong>${punishmentType.name}</strong>`, () => {
      this.punishmentTypeService.delete(punishmentType.id).subscribe(() => {
        abp.notify.success(`Deleted punishmentType ${punishmentType.name}`)
        this.refresh();
      })
    })
  }
}

export enum FILTER_VALUE{
  ACTIVE = 1,
  INACTIVE = 0
}