import { PayrollTokenDto } from '../../../service/model/payroll-token/PayrollTokenDto.dto';
import { Component, Injector, OnInit } from '@angular/core';
import { PagedListingComponentBase } from '@shared/paged-listing-component-base';
import { PagedRequestDto } from '@shared/paged-listing-component-base';
import { PayrollTokenServiceService } from '@app/service/api/payroll-token/payroll-token-service.service';  
import { PERMISSIONS_CONSTANT } from '@app/permission/permission';
@Component({
  selector: 'app-payroll-token',
  templateUrl: './payroll-token.component.html',
  styleUrls: ['./payroll-token.component.css']
})
export class PayrollTokenComponent extends PagedListingComponentBase<PayrollTokenDto> implements OnInit  {
  
 listPayrollToken : PayrollTokenDto[] = [];
  constructor(injector: Injector,private payrollTokenService:PayrollTokenServiceService) {
    super(injector);
   }

  ngOnInit(): void {
    this.pageSizeType = 5;
    this.pageSize = 5;
    this.listBreadCrumb = [
      {name: '<i class="fa-solid fa-house fa-sm"></i>',url:''},
      {name: ' <i class="fa-solid fa-chevron-right"></i> '},
      {name:'Payroll Token',url:''}];
      this.refresh();
  }
  protected list(request: PagedRequestDto, pageNumber: number, finishedCallback: Function): void {
    this.subscription.push(
      this.payrollTokenService.getAllPagging(request).subscribe((rs) => {
        this.listPayrollToken = rs.result.items;
        this.showPaging(rs.result, pageNumber)
      })
    )
  }


  isShowDeleteBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Payroll_Delete);
  }
  isShowSendBtn(){
      return this.isGranted(PERMISSIONS_CONSTANT.Payroll_PaySlip_Token_Send);
    }



  public onDelete(month: any) {
    this.confirmDelete(`Delete payroll token <strong>${month}</strong>`, () => {
      this.subscription.push(
        this.payrollTokenService.deletePayrollToken(month).subscribe(rs => {
          abp.notify.success(`Deleted payroll token of ${month} successfull`)
          this.refresh()
        })
      )
    })
  }

  
}
