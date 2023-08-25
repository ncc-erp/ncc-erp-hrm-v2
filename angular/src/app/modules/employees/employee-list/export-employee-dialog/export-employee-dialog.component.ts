import { MatDialogRef } from '@angular/material/dialog';
import { Component, OnInit } from '@angular/core';
import * as moment from 'moment';

@Component({
  selector: 'app-export-employee-dialog',
  templateUrl: './export-employee-dialog.component.html',
  styleUrls: ['./export-employee-dialog.component.css']
})
export class ExportEmployeeDialogComponent implements OnInit {
  startDate
  endDate
  constructor(private dialogRef:MatDialogRef<ExportEmployeeDialogComponent>) { }

  ngOnInit(): void {
    this.endDate = new Date()
    this.startDate = this.endDate.setMonth(this.startDate.getMonth() - 1);
  }

  onSave(){
    this.startDate = moment(this.startDate).format("YYYY-MM-DD")
    this.endDate = moment(this.endDate).format("YYYY-MM-DD")
    this.dialogRef.close({startDate: this.startDate, endDate:this.endDate})
  }

}
