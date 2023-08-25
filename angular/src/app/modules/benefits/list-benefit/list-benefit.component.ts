import { UpdateBenefitStatusDto } from './../../../service/model/benefits/ChangeBenefitStatus.dto';
import { BenefitService } from './../../../service/api/benefits/benefit.service';
import { benefitDto } from './../../../service/model/benefits/beneft.dto';
import { Component, Injector, OnInit } from '@angular/core';
import { PagedListingComponentBase, PagedRequestDto } from '@shared/paged-listing-component-base';
import { APP_ENUMS } from '@shared/AppEnums';
import { MatMenuTrigger } from '@angular/material/menu';
import { finalize } from 'rxjs/operators';
import { CreateEditBenefitDialogComponent } from '../create-edit-benefit-dialog/create-edit-benefit-dialog.component';
import { BenefitType } from '@shared/AppConsts';
import { PERMISSIONS_CONSTANT } from '@app/permission/permission';

@Component({
  selector: 'app-list-benefit',
  templateUrl: './list-benefit.component.html',
  styleUrls: ['./list-benefit.component.css']
})
export class ListBenefitComponent extends PagedListingComponentBase<benefitDto> implements OnInit {

  public benefitList: benefitDto[] = [];
  public benefitStatus = APP_ENUMS.BenefitType;
  public listBenefitType = []
  public listBenefitStatus = [];
  menu: MatMenuTrigger;
  contextMenuPosition = { x: '0px', y: '0px' };
  public DEFAULT_FILTER: ListBenefitDefaultFilter = {
    isActive: this.APP_ENUM.IsActive.Active,
    type: this.APP_CONST.DEFAULT_ALL_FILTER_VALUE
  }
  protected list(request: PagedRequestDto, pageNumber: number, finishedCallback: Function): void {
    this.subscription.push(
      this.benefitService.getAllPagging(request)
        .pipe(finalize(() => {
          finishedCallback()
        })).subscribe(rs => {
          this.benefitList = rs.result.items;
          this.showPaging(rs.result, pageNumber)
        })
    )
  }

  constructor(injector: Injector,
    private benefitService: BenefitService) {
    super(injector)
  }

  ngOnInit(): void {
    this.listBreadCrumb = [
      {name: '<i class="fa-solid fa-house fa-sm"></i>',url:''}, 
      {name: ' <i class="fa-solid fa-chevron-right"></i> '},
      {name:' Benefit '}];
    this.listBenefitType = this.getListFormEnum(BenefitType)
    this.listBenefitStatus = this.getListFormEnum(APP_ENUMS.IsActive)
    this.bindFilterValue()
    this.refresh();
  }

  public onCreate() {
    this.openDialog(CreateEditBenefitDialogComponent)
  }

  public onUpdate(benefit: benefitDto) {
    this.openDialog(CreateEditBenefitDialogComponent, { benefit: {...benefit} })
  }

  public onDelete(benefit: benefitDto) {
    this.confirmDelete(`Delete benefit <strong>${benefit.name}`,
      () => this.benefitService.delete(benefit.id).subscribe(() => {
        abp.notify.success(`Deleted benefit ${benefit.name}`)
        this.refresh()
      }))
  }

  public onStatusUpdate(benefit: benefitDto, status: boolean) {
    let input = {
      id: benefit.id,
      isActive: status
    } as UpdateBenefitStatusDto
    this.subscription.push(this.benefitService.UpdateStatus(input).subscribe(rs => {
      abp.notify.success(`Update status to ${status ? 'Active' : 'InActive'}`)
      this.refresh()
    }))
  }

  bindFilterValue() {
    if(!this.router.url.includes("filterItems")){
      this.filterItems = [
        {
          propertyName: "isActive",
          value: this.DEFAULT_FILTER.isActive,
          comparision: this.APP_ENUM.filterComparison.EQUAL
        }
      ]
      return;
    }
    if(this.filterItems.length){
      this.filterItems.forEach(filterItem => {
        this.DEFAULT_FILTER[filterItem.propertyName] = filterItem.value;
      })
      if(!this.filterItems.find(filter => filter.propertyName.toLocaleLowerCase() == 'isactive')){
        this.DEFAULT_FILTER.isActive = this.APP_CONST.DEFAULT_ALL_FILTER_VALUE
      }
      return;
    }
    this.DEFAULT_FILTER = {
      isActive: this.APP_CONST.DEFAULT_ALL_FILTER_VALUE,
      type: this.APP_CONST.DEFAULT_ALL_FILTER_VALUE
    }  
  }

  isShowCreateBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Benefit_Create);
  }
  isShowEditBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Benefit_Edit);
  }
  isShowDeleteBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Benefit_Delete);
  }
  isShowActiveBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Benefit_Active);
  }
  isShowDeactiveBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Benefit_Deactive);
  }
  isAllowRoutingDetail(){
    return this.isGranted(PERMISSIONS_CONSTANT.Benefit_BenefitDetail);
  }
}

export interface ListBenefitDefaultFilter{
  isActive: boolean | number,
  type: number
}