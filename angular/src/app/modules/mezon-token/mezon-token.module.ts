import { PayrollTokenRoutingModule } from './mezon-token-routing.component';
import { popperVariation,  tooltipVariation } from '@ngneat/helipopper';
import { TooltipModule } from 'ngx-bootstrap/tooltip';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { SharedModule } from '@shared/shared.module';
import { MezonTokenComponent } from './mezon-tokens/mezon-token.component';
import { ConfirmTokenDialogComponent } from './confirm-token-dialog-component/confirm-token-dialog-component.component';
import { TippyModule } from '@ngneat/helipopper';
import { TippyProps } from '@ngneat/helipopper/lib/tippy.types';

import { AddMezonTokenComponent } from './add-mezon-token/add-mezon-token.component';
export const customeTooltipVariation: Partial<TippyProps> = {
  theme: 'light',
  arrow: true,
  animation: 'scale',
  trigger: 'mouseenter',
  offset: [0, 10],
};

@NgModule({
  declarations: [

    MezonTokenComponent,
    ConfirmTokenDialogComponent,
    AddMezonTokenComponent
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
export class MezonTokenModule { }
