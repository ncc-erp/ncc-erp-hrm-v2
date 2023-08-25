import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';

@Component({
  selector: 'toggle-column',
  templateUrl: './table-toggle-column.component.html',
  styleUrls: ['./table-toggle-column.component.css']
})
export class TableToggleColumnComponent implements OnInit {
  @Input() columnList: columnDto[] = []
  @Input() tableName: string
  @Output() columnToggle = new EventEmitter()
  constructor() { }

  ngOnInit(): void {
    let currentColumnList = localStorage.getItem(this.tableName)
    if (!currentColumnList) {
      localStorage.setItem(this.tableName, JSON.stringify(this.columnList))
    }
    else {
      if (this.compareWithLocalStorage(currentColumnList)) {
        this.columnList = JSON.parse(currentColumnList)
      }
      else{
        localStorage.setItem(this.tableName, JSON.stringify(this.columnList))
      }
    }
    this.columnToggle.emit(this.columnList)
  }
  compareWithLocalStorage(localColumnList) {
    let listToCompare = JSON.parse(localColumnList)
    listToCompare = listToCompare.map(item => item.name)
    let currentColumnList = this.columnList.map(item => item.name)
    if (JSON.stringify(listToCompare) === JSON.stringify(currentColumnList)) {
      return true
    }
    return false
  }

  onColumnToggle(column: columnDto, e) {
    column.isShow = e.checked
    localStorage.setItem(this.tableName, JSON.stringify(this.columnList))
    this.columnToggle.emit(this.columnList)
  }
  onColumnNameChange(){
    localStorage.setItem(this.tableName, JSON.stringify(this.columnList))
  }
}
export interface columnDto {
  name: string;
  isShow: boolean;
}
