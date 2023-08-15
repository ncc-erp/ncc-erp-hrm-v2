import { Component, EventEmitter, Inject, Injector, Input, OnInit, Output, ViewChild } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { EmployeeService } from '@app/service/api/employee/employee.service';
import { GetEmployeeDto } from '@app/service/model/employee/employee.dto';
import { FilterDto, TeamsFilterInputDto } from '@shared/paged-listing-component-base';
import { finalize } from 'rxjs/operators';
import * as _ from 'lodash';
import { DatePipe } from '@angular/common';
import { ListEmployeeComponent } from '../list-employee/list-employee.component';
import { SeniorityFilterDto } from '@app/service/model/employee/GetEmployeeExcept.dto';

@Component({
  selector: 'app-add-employee',
  templateUrl: './add-employee.component.html',
  styleUrls: ['./add-employee.component.css'],

})
export class AddEmployeeComponent implements OnInit {
  @ViewChild('listEmployee') listEmployeeComp: ListEmployeeComponent;
  public title: string = ""
  constructor(private dialogRef: MatDialogRef<AddEmployeeComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any) {
  }

  ngOnInit(): void {
    this.title = this.data.title
  }

  public onFilter(filterItem: FilterDto): void {
    this.listEmployeeComp.onFilter(filterItem)
  }

  onSearchEnter(searchText: string) {
    this.listEmployeeComp.onSearchEnter(searchText)
  }

  public onSave() {
    let listEmployeeId: number[] = this.listEmployeeComp.selectedEmployees.map(x => x.id)
    this.dialogRef.close(listEmployeeId)
  }
  public onCancel() {
    this.dialogRef.close()
  }

  public onFilterBySeniority(listData: SeniorityFilterDto): void {
    this.listEmployeeComp.onFilterBySeniority(listData);
  }

  public onMultiFilterWithCondition(teamsFilterInput: TeamsFilterInputDto): void {
    this.listEmployeeComp?.onMultiFilterWithCondition(teamsFilterInput);
  }

  public onMultiFilter(listData): void {
    this.listEmployeeComp.onMultiFilter(listData);
  }
  public onDateSelectorChange(filterValue){
    this.listEmployeeComp?.onDateSelectorChange(filterValue);
  }


}
