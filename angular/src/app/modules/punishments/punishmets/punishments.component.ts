import { AppConsts } from '@shared/AppConsts';
import { CreateEditPunishmentsComponent } from './../create-edit-punishment/create-edit-punishment.component';
import { finalize } from 'rxjs/operators';
import { PunishmentService } from '../../../service/api/punishment/punishment.service';
import { PunishmentsDto } from '@app/service/model/punishments/punishments.dto';
import { Component, OnInit, Injector, ViewChild } from '@angular/core';
import { PagedListingComponentBase, PagedRequestDto } from '@shared/paged-listing-component-base';
import * as moment from 'moment';
import { GeneratePunishmentsComponent } from '../generate-punishments/generate-punishments.component';
import { APP_ENUMS } from '@shared/AppEnums';
import { MatMenuTrigger } from '@angular/material/menu';
import { PERMISSIONS_CONSTANT } from '@app/permission/permission';

@Component({
  selector: 'app-punishments',
  templateUrl: './punishments.component.html',
  styleUrls: ['./punishments.component.css']
})
export class PunishmentsComponent extends PagedListingComponentBase<PunishmentsDto> implements OnInit {
  @ViewChild(MatMenuTrigger)
  public menu: MatMenuTrigger;
  public contextMenuPosition = { x: '0px', y: '0px' };
  public listDate: any = [];
  public punishmentList: PunishmentsDto[] = [];
  public date = AppConsts.DEFAULT_ALL_FILTER_VALUE;
  public punishmentStatus = APP_ENUMS.PunishmentStatus.Active;
  public punishmentStatusList = [];
  public DEFAULT_FILTER = {
    isActive: APP_ENUMS.PunishmentStatus.Active,
    date: this.date
  }
  public isPunishmentHasEmployee:boolean = false;

  // Punishment_View = PERMISSIONS_CONSTANT.Punishment_View
  // Punishment_Create = PERMISSIONS_CONSTANT.Punishment_Create
  // Punishment_Generate = PERMISSIONS_CONSTANT.Punishment_Generate
  // Punishment_Edit = PERMISSIONS_CONSTANT.Punishment_Edit
  // Punishment_Active = PERMISSIONS_CONSTANT.Punishment_Active
  // Punishment_Deactive = PERMISSIONS_CONSTANT.Punishment_Deactive
  // Punishment_Delete = PERMISSIONS_CONSTANT.Punishment_Delete
  // Punishment_PunishmentDetail = PERMISSIONS_CONSTANT.Punishment_PunishmentDetail

  protected list(request: PagedRequestDto, pageNumber: number, finishedCallback: Function): void {
    this.subscription.push(
      this.punishmentService.getAllPagging(request)
        .pipe(finalize(() => {
          finishedCallback();
        }))
        .subscribe(rs => {
          this.punishmentList = rs.result.items;
          this.punishmentList.forEach((pun)=>{
            pun.date = moment(pun.date).format("MM/YYYY");
          })
          this.showPaging(rs.result, pageNumber)
        })
    )
  }



  constructor(injector: Injector, private punishmentService: PunishmentService) {
    super(injector);
  }


  ngOnInit(): void {
    if(!this.filterItems.length){
      this.setDefaultFilter({ propertyName: 'isActive', value: this.DEFAULT_FILTER.isActive, comparision: 0 })
    }else{
      this.bindFilterValue()
    }
    this.listBreadCrumb = [
      {name: '<i class="fa-solid fa-house fa-sm"></i>',url:''},
      {name: ' <i class="fa-solid fa-chevron-right"></i> '},
      {name:' Punishment '}];
    this.setDefaultFilter({ propertyName: 'isActive', value: this.punishmentStatus, comparision: 0 })
    this.refresh();
    this.getDateFromPunishments();
    this.AddFilterAll(this.punishmentStatusList);
    this.punishmentStatusList = this.getListFormEnum(APP_ENUMS.PunishmentStatus);

  }

  isShowCreateBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Punishment_Create);
  }
  isShowGenerateBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Punishment_Generate);
  }
  isShowEditBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Punishment_Edit);
  }
  isShowDeleteBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Punishment_Delete);
  }
  isShowActiveBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Punishment_Active);
  }
  isShowDeactiveBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Punishment_Deactive);
  }
  isAllowRoutingDetail(){
    return this.isGranted(PERMISSIONS_CONSTANT.Punishment_PunishmentDetail);
  }



  bindFilterValue() {
    this.filterItems.forEach(filterItem => {
      this.DEFAULT_FILTER[filterItem.propertyName] = filterItem.value;
    })
  }
  public getDateFromPunishments() {
    this.subscription.push(
    this.punishmentService.getDateFromPunishments().subscribe((res) => {
      this.listDate = res.result.map(x => {
        return {
          key: moment(x).format("MM/YYYY"),
          value: moment(x).format("MM/YYYY"),
        }
      });
      this.AddFilterAll(this.listDate);
    }))
  }
  public onCreate() {
    this.openDialog(CreateEditPunishmentsComponent);
  }

  public onUpdate(punishment : PunishmentsDto){
    this.openDialog(CreateEditPunishmentsComponent, {...punishment});
  }

  public onGenerate() {
    const dialog = this.dialog.open(GeneratePunishmentsComponent, {
      width: "70vw"
    });
    dialog.afterClosed().subscribe((rs)=>{
      this.refresh();
    })
  }

  public onDelete(punishment: PunishmentsDto) {
    this.isPunishmentHasEmployeeFunc(punishment.id);
    this.confirmDelete(`Delete punishment <strong>${punishment.name}</strong>`,
      () => {
        if (this.isPunishmentHasEmployee) {
          this.confirmDelete(`<strong>${punishment.name}</strong> has employee , are you sure to delete?`,
            () =>
              this.punishmentService.delete(punishment.id).toPromise().then(rs => {
                abp.notify.success(`Deleted punishment ${punishment.name}`);
                this.refresh();
              }))
        }else{
          this.punishmentService.delete(punishment.id).toPromise().then(rs => {
            abp.notify.success(`Deleted punishment ${punishment.name}`);
            this.refresh();
          })
        }
      })
  }

  public isPunishmentHasEmployeeFunc(id: number){
    this.subscription.push(
      this.punishmentService.IsPunishmentHasEmployee(id).subscribe((rs)=>{
        this.isPunishmentHasEmployee = rs.result;
      })
    )
  }

  public onChangeStatus(punishment) {
    this.subscription.push(
    this.punishmentService.changeStatus(punishment.id).subscribe((res) => {
      if (res) {
        abp.notify.success(`Change Status Successfully`);
      }
      this.refresh();
    }))
  }

  private AddFilterAll(listData: any[]) {
    listData.unshift({ key: "All", value: AppConsts.DEFAULT_ALL_FILTER_VALUE })
  }
}
