import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ListPunishmentRefundsComponent } from './list-punishment-refunds/list-punishment-refunds.component';
import { PunishmentFundsRoutingModule } from './punishment-funds-routing.module';
import { SharedModule } from '@shared/shared.module';
import { CreateEditDisburseComponent } from './list-punishment-refunds/create-edit-disburse/create-edit-disburse.component';


@NgModule({
  declarations: [ListPunishmentRefundsComponent, CreateEditDisburseComponent],
  imports: [
    CommonModule,
    PunishmentFundsRoutingModule,
    SharedModule

  ],
  exports: [ListPunishmentRefundsComponent]
})
export class PunishmentFundsModule { }
