import { BonusService } from '../../../service/api/bonuses/bonus.service';
import { BonusDto } from '../../../service/model/bonuses/bonus.dto';
import { Component, Injector, OnInit } from '@angular/core';
import { PagedListingComponentBase, PagedRequestDto } from '@shared/paged-listing-component-base';
import { finalize } from 'rxjs/operators';
import { CreateEditBonusDialogComponent } from '../create-edit-bonus-dialog/create-edit-bonus-dialog.component';
import * as moment from 'moment';
import { APP_ENUMS } from '@shared/AppEnums';
import { MatMenuTrigger } from '@angular/material/menu';
import { AppConsts } from '@shared/AppConsts';
import { PERMISSIONS_CONSTANT } from '@app/permission/permission';
@Component({
  selector: 'app-bonus-list',
  templateUrl: './bonus-list.component.html',
  styleUrls: ['./bonus-list.component.css']
})

export class BonusListComponent extends PagedListingComponentBase<BonusDto> implements OnInit {
  public bonusList: BonusDto[] = [];

  public listDate: any = [];
  public bonusStatusList = [];
  public bonusStatus = APP_ENUMS.BonusStatusHaveAll.Active;
  public applyMonthDefault = "-1";
  public isBonusHasEmployee:boolean = false;
  public listStatus = Object.keys(APP_ENUMS.BonusStatusHaveAll).forEach((key) => {
    this.bonusStatusList.push({
      key: key,
      value: APP_ENUMS.BonusStatusHaveAll[key]
    })
  })
  menu: MatMenuTrigger;
  contextMenuPosition = { x: '0px', y: '0px' };
  public DEFAULT_FILTER = {
    isActive: APP_ENUMS.BonusStatusHaveAll.Active,
    applyMonth: this.applyMonthDefault
  }
  protected list(request: PagedRequestDto, pageNumber: number, finishedCallback: Function): void {
    this.subscription.push(
      this.bonusService.getAllPagging(request)
        .pipe(finalize(() => {
          finishedCallback()
        })).subscribe(rs => {
          this.bonusList = rs.result.items;
          this.showPaging(rs.result, pageNumber)
        })
    )
  }

  constructor(injecor: Injector,
    private bonusService: BonusService) {
    super(injecor)
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
      {name:' Bonus '}];
    this.setDefaultFilter({ propertyName: 'isActive', value: this.bonusStatus, comparision: 0 })
    this.refresh();
    this.getListMonthAgoFilter();
  }

  bindFilterValue() {
    this.filterItems.forEach(filterItem => {
      this.DEFAULT_FILTER[filterItem.propertyName] = filterItem.value;
    })
  }

  getListMonthAgoFilter() {
    this.bonusService.getListMonthFilter().subscribe((res) => {
      this.listDate = res.result.map(x => {
        return {
          key: moment(x).format("MM/YYYY"),
          value: moment(x).format("YYYY/MM/DD"),
        }
      });
      this.listDate.unshift({ key: "All", value: "-1" })
    })
  }

  onCreate() {
    let bonus = {} as BonusDto;
    const d = new Date();
    let year = d.getFullYear();
    let month = d.getMonth() + 1;
    if (month == 1) {
      month = 12;
      year = year - 1;
    }
    month = month - 1
    if (month < 10) {
      bonus.applyMonth = "0" + month + "/" + year;
    }
    else {
      bonus.applyMonth = month + "/" + year;
    }
    this.openDialog(CreateEditBonusDialogComponent, { ...bonus })
  }

  onUpdate(bonus: BonusDto) {
    this.openDialog(CreateEditBonusDialogComponent, { ...bonus })
  }

  onDelete(bonus: BonusDto) {
    this.isBonusHasEmployeeFunc(bonus.id);

    this.confirmDelete(`Delete bonus <strong>${bonus.name}`,
      () => {
        if (this.isBonusHasEmployee) {
          this.confirmDelete(`<strong>${bonus.name}</strong> has employee, are you sure to delete?`, () =>
            this.bonusService.delete(bonus.id).subscribe(() => {
              abp.notify.success(`Deleted bonus ${bonus.name}`)
              this.refresh()
            })
            )
        }else{
          this.bonusService.delete(bonus.id).subscribe(() => {
            abp.notify.success(`Deleted bonus ${bonus.name}`)
            this.refresh()
          })
        }
      })
  }

  public isBonusHasEmployeeFunc(id: number) {
    this.subscription.push(
      this.bonusService.IsBonusHasEmployee(id).subscribe((rs)=>{
        this.isBonusHasEmployee =  rs.result;
      })
    )
  }

  onChangeStatus(bonus) {
    this.bonusService.changeStatus(bonus.id).subscribe((res) => {
      if (res) {
        abp.notify.success(`Change Status bonus Successfully`);
      }
      this.refresh();
    })
  }

  isShowCreateBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Bonus_Create);
  }
  isShowEditBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Bonus_Edit);
  }
  isShowDeleteBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Bonus_Delete);
  }
  isShowActiveBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Bonus_Active);
  }
  isShowDeactiveBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Bonus_Deactive);
  }
  isAllowRoutingDetail(){
    return this.isGranted(PERMISSIONS_CONSTANT.Bonus_BonusDetail);
  }
  
}