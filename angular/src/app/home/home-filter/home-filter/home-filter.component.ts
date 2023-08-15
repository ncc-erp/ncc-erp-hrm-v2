import { Component, OnInit, Output, EventEmitter, Input } from '@angular/core';
import { APP_ENUMS } from '@shared/AppEnums';

@Component({
  selector: 'app-home-filter',
  templateUrl: './home-filter.component.html',
  styleUrls: ['./home-filter.component.css']
})
export class HomeFilterComponent implements OnInit {
  APP_ENUM = APP_ENUMS
  @Output() onDateSelectorChange = new EventEmitter()
  @Input() fromDate
  @Input() toDate

  constructor() { }

  ngOnInit(): void {
    this.getDateOptions();
    this.birthdayFromDate = this.fromDate;
    this.birthdayToDate = this.toDate;
  }
  public listDateOptions = [];
  public defaultValue = APP_ENUMS.DATE_TIME_OPTIONS.Month;
  public birthdayFromDate: string
  public birthdayToDate: string
  getDateOptions() {
    this.listDateOptions = [APP_ENUMS.DATE_TIME_OPTIONS.Day, APP_ENUMS.DATE_TIME_OPTIONS.Week,
    APP_ENUMS.DATE_TIME_OPTIONS.Month, APP_ENUMS.DATE_TIME_OPTIONS.Quarter,
    APP_ENUMS.DATE_TIME_OPTIONS.Year, APP_ENUMS.DATE_TIME_OPTIONS.CustomTime];
  }
  public handleDateSelectorChange(value) {
     this.onDateSelectorChange.emit(value);
  }
}
