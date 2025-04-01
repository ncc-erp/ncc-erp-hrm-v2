import { APP_ENUMS } from './../../../../shared/AppEnums';
import { ActivatedRoute } from '@angular/router';
import { PayrollTokenDto,PayslipTokenDto } from './../../../service/model/payroll-token/PayrollTokenDto.dto';
import { Component, Injector, OnInit } from '@angular/core';
import { PagedListingComponentBase } from '@shared/paged-listing-component-base';
import { PagedRequestDto } from '@shared/paged-listing-component-base';
import { PayrollTokenServiceService } from '@app/service/api/payroll-token/payroll-token-service.service';  
import { PERMISSIONS_CONSTANT } from '@app/permission/permission';

@Component({
  selector: 'app-payslip-token',
  templateUrl: './payslip-token.component.html',
  styleUrls: ['./payslip-token.component.css']
})
export class PayslipTokenComponent extends PagedListingComponentBase<PayslipTokenDto> implements OnInit {
  public month : string;
  public statusSendTokens : any
  public listPayslipToken :  PayslipTokenDto[];
  constructor(injector: Injector,private route:ActivatedRoute,private payrollTokenService:PayrollTokenServiceService) {
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
      {name:'Payslip Token Detail',url:''}];
      this.refresh();
      this.getAllStatusSendToken()
  }
  protected list(request: PagedRequestDto, pageNumber: number, finishedCallback: Function): void {
    this.subscription.push(
      this.payrollTokenService.getAllPaggingPayslip(this.month,request).subscribe((rs) => {
        this.listPayslipToken = rs.result.items;
        this.showPaging(rs.result, pageNumber)
      }, () => this.isLoading = false))   
  }
  private getAllStatusSendToken(){
    const statusSendTokens = this.getListFormEnum(APP_ENUMS.StatusSendToken).filter(item => item.key != 'All');
    this.statusSendTokens = statusSendTokens.reduce((acc, item) => {
      acc[item.value as string] = item.key; 
      return acc;
    }, {});
  }


  public onDelete(id: any) {

    this.confirmDelete(`Delete payslip token has id <strong>${id}</strong>`, () => {
      this.subscription.push(
        this.payrollTokenService.deletePayslipToken(id).subscribe(rs => {
          abp.notify.success(`Deleted payslip token with id ${id} successfull`)
          this.refresh()
        })
      )
    })
  }

  isShowSendBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Payroll_PaySlip_Token_Send);
  }
  isShowDeleteBtn(){
      return this.isGranted(PERMISSIONS_CONSTANT.Payroll_PaySlip_Token_Delete);
    }


  SendToken(id: any){
     this.subscription.push(
      this.payrollTokenService.sendToken(id).subscribe((rs) => {
        if(rs.success){
          this.notify.success("Send token success");
          this.refresh()
        }
      }
      )
     )
  }
}
