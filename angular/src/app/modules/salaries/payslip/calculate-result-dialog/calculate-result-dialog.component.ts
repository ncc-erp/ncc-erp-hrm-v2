import { MatDialog } from '@angular/material/dialog';
import { Injector, QueryList, ElementRef, ViewChild, Inject, ChangeDetectorRef, NgZone } from '@angular/core';
import { Component, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { AppComponentBase } from '@shared/app-component-base';
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

  constructor(injector: Injector, 
    @Inject(MAT_DIALOG_DATA) public data, 
    private dialog:MatDialog,
    public ngZone: NgZone,
    public dialogRef: MatDialogRef<CalculateResultDialogComponent>) {
    super(injector);
  }

  ngOnInit(): void {
    this.title = `Calculate salary for payroll: ${this.formatDateMY(this.data.payrollApplyMonth)}`
    this.status = ''
  }

  ngAfterViewInit() {
    this.scrollToBottom();
      this.subscription.push(
      this.APP_CONST.calSalaryProcess.asObservable().subscribe(rs => {
        this.ngZone.run(() => {
          this.resulMessage.push(rs)
          this.process = rs?.process ?? ""
          this.status = rs?.status
          this.scrollToBottom()
          if(rs?.status == 'Error' && rs?.message?.errorList.length>0){
  
            this.dialog.open(CalculateResultComponent, {
              width: "700px",
              data: rs?.message?.errorList
            })
          }
        })        
      }))
  }

  isDone(){
    return this.status == 'Done'
  }

  scrollToBottom = () => {
    try {
      this.content.nativeElement.scrollTop = this.content.nativeElement.scrollHeight;
    } catch (err) { console.log(err)}
  }

  closeDialog() {
    this.dialogRef.close(true);
  }
}
