import { APP_ENUMS } from '../../../../shared/AppEnums';
import { ActivatedRoute } from '@angular/router';
import { MezonTokenDto } from '../../../service/model/payroll-token/PayrollTokenDto.dto';
import { Component, Injector, OnInit } from '@angular/core';
import { PagedListingComponentBase } from '@shared/paged-listing-component-base';
import { PagedRequestDto } from '@shared/paged-listing-component-base';
import { MezonTokenServiceService } from '@app/service/api/payroll-token/payroll-token-service.service';  
import { PERMISSIONS_CONSTANT } from '@app/permission/permission';
import { property } from '@node_modules/@types/lodash';
import { AddMezonTokenComponent } from '../add-mezon-token/add-mezon-token.component';

@Component({
  selector: 'app-mezon-token',
  templateUrl: './mezon-token.component.html',
  styleUrls: ['./mezon-token.component.css']
})
export class MezonTokenComponent extends PagedListingComponentBase<MezonTokenDto> implements OnInit {
  public month : string;
  public statusSendTokens : any
  public listMezonToken :  MezonTokenDto[];
  public selectedSatatusSendToken : number =0;
    public DEFAULT_FILTER: StatusSent = {
      status: APP_ENUMS.StatusSendToken.Pending,
      type: this.APP_CONST.DEFAULT_ALL_FILTER_VALUE
    }
    statusSendConvert : any
  constructor(injector: Injector,private route:ActivatedRoute,private mezonTokenService:MezonTokenServiceService) {
    super(injector);

   }

ngOnInit(): void {
   this.pageSizeType = 5;
    this.pageSize = 5;
    this.route.queryParams.subscribe(params => {
      this.month = params['id']
    });
    this.statusSendTokens = this.getListFormEnum(APP_ENUMS.StatusSendToken);
    this.listBreadCrumb = [
      {name: '<i class="fa-solid fa-house fa-sm"></i>',url:''},
      {name: ' <i class="fa-solid fa-chevron-right"></i> '},
      {name:'Mezon Token ',url:''}];
      this.refresh();
     this.getAllStatusSendToken()
  }
  protected list(request: PagedRequestDto, pageNumber: number, finishedCallback: Function): void { 
    
    this.subscription.push(
      this.mezonTokenService.getAllPagging(request).subscribe((rs) => {
        this.listMezonToken = rs.result.items;
        this.showPaging(rs.result, pageNumber)
      }, () => this.isLoading = false))   
  }
  private getAllStatusSendToken(){
    const statusSendConvert = this.getListFormEnum(APP_ENUMS.StatusSendToken).filter(item => item.key != 'All');
    this.statusSendConvert = statusSendConvert.reduce((acc, item) => {
      acc[item.value as string] = item.key; 
      return acc;
    }, {});
  }

  isCheckAction(id: any) {   
    return id !== APP_ENUMS.StatusSendToken.SentToEmployee;
  }
  

  public onDelete(id: any) {

    this.confirmDelete(`Delete mezon token has id <strong>${id}</strong>`, () => {
      this.subscription.push(
        this.mezonTokenService.deleteMezonToken(id).subscribe(rs => {
          abp.notify.success(`Deleted mezon token with id ${id} successfull`)
          this.refresh()
        })
      )
    })
  }

  public deleteAllPending(){
    this.confirmDelete(`Delete mezon token has status Pending`, () => {
      this.subscription.push(
        this.mezonTokenService.deleteAllPending().subscribe(rs => {
          abp.notify.success(`Deleted mezon token with status Pending successfull`)
          this.refresh()
        })
      )
    })
  }

  openEditMezonToken(mezonToken){
      const dia = this.dialog.open(AddMezonTokenComponent, {
        data: {
          title : "Edit Mezon Token",
          mezonToken : {...mezonToken},
          type : "edit"
        },
        width: "700px"
  
      })
      dia.afterClosed().subscribe((rs)=>{
        if(rs){
          this.refresh();
        }
      })
    }
  
    openAddMezonToken(){
      const dia = this.dialog.open(AddMezonTokenComponent, {
        data: {
          title : 'Add Mezon Token',
          type : "create",
        },
        width: "700px"
  
      })
      dia.afterClosed().subscribe((rs)=>{
        if(rs){
          this.refresh();
        }
      })
    }
  

  isShowEditBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Mezon_Token_Edit);
  }
  isShowDeleteBtn(){
      return this.isGranted(PERMISSIONS_CONSTANT.Mezon_Token_Delete);
    }
  isShowAddBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Mezon_Token_Create);
  }

}

export interface StatusSent{
  status: boolean | number,
  type: number
}