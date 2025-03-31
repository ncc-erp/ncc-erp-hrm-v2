import { PayrollTokenRoutingModule } from './payroll-token-routing.component';
import { popperVariation,  tooltipVariation } from '@ngneat/helipopper';
import { TooltipModule } from 'ngx-bootstrap/tooltip';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { SharedModule } from '@shared/shared.module';
import { PayslipTokenComponent } from './payslip-token/payslip-token.component';
import { PayrollTokenComponent } from './payroll-token/payroll-token.component';
import { ConfirmTokenDialogComponent } from './confirm-token-dialog-component/confirm-token-dialog-component.component';
import { TippyModule } from '@ngneat/helipopper';
import { TippyProps } from '@ngneat/helipopper/lib/tippy.types';

export const customeTooltipVariation: Partial<TippyProps> = {
  theme: 'light',
  arrow: true,
  animation: 'scale',
  trigger: 'mouseenter',
  offset: [0, 10],
};

@NgModule({
  declarations: [
    PayrollTokenComponent,
    PayslipTokenComponent,
    ConfirmTokenDialogComponent
]   ,
  imports: [
    FormsModule,
        ReactiveFormsModule,
        CommonModule,
        SharedModule, 
        PayrollTokenRoutingModule,
           FormsModule,
            ReactiveFormsModule,
            TippyModule.forRoot({
              defaultVariation: 'tooltip',
              variations: {
                tooltip: customeTooltipVariation,
                popper: popperVariation,
              }
            })
  ]
})
export class PayrollTokenModule { }
