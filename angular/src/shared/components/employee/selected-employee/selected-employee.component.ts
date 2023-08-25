import { Component, Input, OnInit, Output, EventEmitter } from '@angular/core';
import { GetEmployeeDto } from '@app/service/model/employee/employee.dto';
@Component({
  selector: 'app-selected-employee',
  templateUrl: './selected-employee.component.html',
  styleUrls: ['./selected-employee.component.css']
})
export class SelectedEmployeeComponent implements OnInit {
  @Input() employeeList: GetEmployeeDto[] = []
  @Input() selectedEmployees: GetEmployeeDto[] = []
  @Output() deselectEmployee: EventEmitter<GetEmployeeDto> = new EventEmitter();
  public listSelected: GetEmployeeDto[] = []

  constructor() { }

  ngOnChanges(): void {
    this.listSelected = this.selectedEmployees
  }
  ngOnInit(): void {
  }

  public removeSelected(employee: GetEmployeeDto) {
    employee.selected = false
    this.deselectEmployee.emit(employee)
  }
}
