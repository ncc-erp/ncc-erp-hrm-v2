import { RefundsRoutingModule } from './refunds-routing.module';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ListRefundComponent } from './list-refund/list-refund.component';
import { RefundDetailComponent } from './refund-detail/refund-detail.component';
import { CreateEditRefundDialogComponent } from './create-edit-refund-dialog/create-edit-refund-dialog.component';
import { SharedModule } from '@shared/shared.module';



@NgModule({
  declarations: [
    ListRefundComponent,
    RefundDetailComponent,
    CreateEditRefundDialogComponent
  ],
  imports: [
    CommonModule,
    SharedModule,
    RefundsRoutingModule
  ]
})
export class RefundsModule { }
