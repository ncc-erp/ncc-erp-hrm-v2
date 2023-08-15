import { MatDialog } from '@angular/material/dialog';
import { DatePipe } from '@angular/common';
import { Injector, QueryList, ElementRef, ViewChild, Inject, ChangeDetectionStrategy } from '@angular/core';
import { Component, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { AppComponentBase } from '@shared/app-component-base';
import { DialogComponentBase } from '@shared/dialog-component-base';
import { BehaviorSubject } from 'rxjs';
import { CalculateResultComponent } from '../../payslip-detail/calculate-result/calculate-result.component';

@Component({
  selector: 'app-calculate-result-dialog',
  templateUrl: './calculate-result-dialog.component.html',
  styleUrls: ['./calculate-result-dialog.component.css'],
})
export class CalculateResultDialogComponent extends AppComponentBase implements OnInit {
  @ViewChild('messages') messages: QueryList<any>;
  @ViewChild('content') content: ElementRef;

  public process: string = ""
  public status: string = ""
  public resulMessage: any[] = []
  public title: string = ""

  constructor(injector: Injector, @Inject(MAT_DIALOG_DATA) public data, private dialog:MatDialog,
    public dialogRef: MatDialogRef<CalculateResultDialogComponent>) {
    super(injector);
  }

  ngOnInit(): void {
    this.title = `Calculate salary for payroll: ${this.formatDateMY(this.data.payrollApplyMonth)}`
  }

  ngAfterViewInit() {
    this.scrollToBottom();
    this.APP_CONST.calSalaryProcess.asObservable().subscribe(rs => {
        this.resulMessage.push(rs)
        this.process = rs?.process ?? ""
        this.status = rs?.status ?? "Starting..."
        this.scrollToBottom()
        if(rs?.status == 'Error' && rs?.message?.errorList.length>0){

          this.dialog.open(CalculateResultComponent, {
            width: "700px",
            data: rs?.message?.errorList
          })
        }
    })
  }

  scrollToBottom = () => {
    try {
      this.content.nativeElement.scrollTop = this.content.nativeElement.scrollHeight;
    } catch (err) { }
  }

  closeDialog() {
    this.dialogRef.close(true)
  }
}
