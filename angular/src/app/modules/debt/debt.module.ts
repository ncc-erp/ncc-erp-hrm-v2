import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SharedModule } from '@shared/shared.module';
import { DebtListComponent } from './debt-list/debt-list.component';
import {DebtRoutingModule} from './debt-routing.module';
import { CreateEditViewDebtComponent } from './create-edit-view-debt/create-edit-view-debt.component'
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { DebtPlanComponent } from './debt-plan/debt-plan.component';
import { DebtPaidComponent } from './debt-paid/debt-paid.component';

@NgModule({
  declarations: [
    DebtListComponent,
    CreateEditViewDebtComponent,
    DebtPlanComponent,
    DebtPaidComponent,
  ],
  imports: [
    CommonModule,
    SharedModule,
    DebtRoutingModule,
    FormsModule,
    ReactiveFormsModule,
  ],
})
export class DebtModule { }
