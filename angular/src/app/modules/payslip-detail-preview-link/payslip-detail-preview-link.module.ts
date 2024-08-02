import { PayslipDetailPreviewLinkRoutingModule } from './payslip-detail-preview-link-routing.module';
import { SharedModule } from './../../../shared/shared.module';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PayslipDetailPreviewLinkComponent } from './payslip-detail-preview-link.component';

@NgModule({
  declarations: [
    PayslipDetailPreviewLinkComponent
  
  ],
  imports: [
    CommonModule,
    SharedModule,
    PayslipDetailPreviewLinkRoutingModule,
  ],
})
export class PayslipDetailPreviewLinkModule {
  constructor() {
  }
}
