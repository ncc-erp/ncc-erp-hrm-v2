import { SentToken } from './../../../service/model/payroll-token/PayrollTokenDto.dto';
import { APP_ENUMS } from '../../../../shared/AppEnums';
import { ActivatedRoute } from '@angular/router';
import { MezonTokenDto } from '../../../service/model/payroll-token/PayrollTokenDto.dto';
import { Component, Injector, OnInit } from '@angular/core';
import { PagedListingComponentBase } from '@shared/paged-listing-component-base';
import { PagedRequestDto } from '@shared/paged-listing-component-base';
import { MezonTokenServiceService } from '@app/service/api/mezon-token/mezon-token-service.service';  
import { PERMISSIONS_CONSTANT } from '@app/permission/permission';
import { property } from '@node_modules/@types/lodash';
import { AddMezonTokenComponent } from '../add-mezon-token/add-mezon-token.component';
import { MatDialog } from '@angular/material/dialog';
import * as FileSaver from 'file-saver';

@Component({
  selector: 'app-mezon-token',
  templateUrl: './mezon-token.component.html',
  styleUrls: ['./mezon-token.component.css']
})
export class MezonTokenComponent extends PagedListingComponentBase<MezonTokenDto> implements OnInit {
  public month : string;
  public statusSendTokens : any
  public listMezonToken :  MezonTokenDto[];
  public filter : any;
  public totalAmount: number
  public selectedSatatusSendToken : number =0;
    public DEFAULT_FILTER: StatusSent = {
      status: APP_ENUMS.StatusSendToken.Pending,
      type: this.APP_CONST.DEFAULT_ALL_FILTER_VALUE
    }
    statusSendConvert : any

  constructor(injector: Injector,private route:ActivatedRoute,private mezonTokenService :MezonTokenServiceService) {
    super(injector);
   }

ngOnInit(): void {
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
    this.filter = request;
    this.subscription.push(
      this.mezonTokenService.GetListMezonToken(request).subscribe((rs) => {
        this.listMezonToken = rs.result.result.items;
        this.totalAmount = rs.result.totalAmout;

      this.showPaging(rs.result.result, pageNumber)
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
  public onExport() {
  
    this.subscription.push(
      this.mezonTokenService.exportMezonToken(this.filter).subscribe((rs) => {
        const file = new Blob([this.convertFile(atob(rs.result.base64))], {
          type: "application/vnd.ms-excel;charset=utf-8"
        });
        FileSaver.saveAs(file, `MezonToken.xlsx`)
      })

    )
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
          title : "Edit Send Token To User",  
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
          title : 'Send Token To User',
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
  isShowExportBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Mezon_Token_Export);
  }
  isShowDeleteAllBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Mezon_Token_DeleteAll);
  }
  isShowSentTokenBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Mezon_Token_SendToken);
  }
  isShowSentTokenAllBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Mezon_Token_SendTokenAll);
  }   
  isAllowViewTabPersonalInfo(){
    return this.isGranted(PERMISSIONS_CONSTANT.Employee_EmployeeDetail_TabPersonalInfo_View);
  }
  SentToken(mezonToken){
    const input = {
        mezonTokenId: mezonToken.id,
        tokenBot:"",
    }
    this.confirmDelete(`Sent token for ${mezonToken.email}`, () => {
      this.subscription.push(
        this.mezonTokenService.sentToken(input).subscribe(rs => {   

          if (rs.result.code == 0) {
            abp.message.success(rs.result.message);
          }else{
            abp.message.error(rs.result.message);  
          }    
          this.refresh();
        })
      )
    })
  }

  SentAllMezonToken() {
    this.confirmDelete('Send All Mezon Token Pending', () => {
      this.subscription.push(
        this.mezonTokenService.sentAllMezonTokenPending().subscribe({
          next: (rs) => {
            if (rs.result) {           
              abp.message.success(rs.result);
              this.refresh();
            } 
          }
        })
      );
      this.refresh();
    });
  }
  

}

export interface StatusSent{
  status: boolean | number,
  type: number
}
