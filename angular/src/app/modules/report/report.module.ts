import { TooltipModule } from 'ngx-bootstrap/tooltip';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ReportSalaryComponent } from './report-salary/report-salary.component';
import { ReportRoutingModule } from './report-routing.module';
import { SharedModule } from '@shared/shared.module';


@NgModule({
  declarations: [
    ReportSalaryComponent
]
    ,
  imports: [
    FormsModule,
        ReactiveFormsModule,
        CommonModule,
        SharedModule, 
        TooltipModule.forRoot(),
        ReportRoutingModule
  ]
})
export class ReportModule { }
