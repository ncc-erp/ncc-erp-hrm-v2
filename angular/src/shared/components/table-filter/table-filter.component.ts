import { APP_ENUMS } from './../../AppEnums';
import { Component, EventEmitter, Input, OnInit, Output, SimpleChanges, ViewChild } from '@angular/core';
import { extend, isNull } from 'lodash-es';
import * as moment from 'moment';
import { DateAdapter, MAT_DATE_FORMATS, MAT_DATE_LOCALE } from '@angular/material/core';
import { MomentDateAdapter } from '@angular/material-moment-adapter';
import { APP_DATE_FORMATS } from '@shared/custom-date-adapter';
import { TeamDto } from '@app/service/model/categories/team.dto';
import { FormControl } from '@angular/forms';
import { takeUntil } from 'rxjs/operators';
import { ReplaySubject, Subject } from 'rxjs';
import { MatSelect } from '@angular/material/select';
import { TeamsFilterInputDto } from '@shared/paged-listing-component-base';
import { AppComponentBase } from '@shared/app-component-base';
import { FilterByComparisonDto } from '@app/service/model/punishment-fund/punishment-fund.dto';
@Component({
  selector: 'table-filter',
  templateUrl: './table-filter.component.html',
  styleUrls: ['./table-filter.component.css'],
  providers: [
    { provide: DateAdapter, useClass: MomentDateAdapter, deps: [MAT_DATE_LOCALE] },
    {
      provide: MAT_DATE_FORMATS,
      useValue: APP_DATE_FORMATS
    }
  ]
})
export class TableFilterComponent extends AppComponentBase implements OnInit {
  @ViewChild("multiSelect") multiSelect: MatSelect;
  @Input() filterType: number
  @Input() placeholder?: string = ""
  @Input() dropdownData: any[] = []
  @Input() defaultValue: any;
  @Output() onTableFilter = new EventEmitter()
  @Output() onTableMultiSelectWithConditionFilter = new EventEmitter()
  @Output() onTableMultiSelectFilter = new EventEmitter();
  @Output() onTableFilterBySeniority  = new EventEmitter();
  @Output() onChangeDaysLeftOfContractEnd = new EventEmitter();
  @Output() onTableFilterByOperatorComparison = new EventEmitter();
  @Input() searchable: boolean
  @Input() isExistFilterContractEndDate: boolean;
  public dropdownSearchText: string = ""
  public searchText: string = ""
  public filterTypeEnum = APP_ENUMS.FilterTypeEnum
  public filterComparisonEnum = APP_ENUMS.filterComparison
  public radioFilterValue: boolean
  public dropdownFilterValue: number
  public dropdownMultiValueFilter: number[] = []
  public datePickerValue: string
  public filterComparison: number = APP_ENUMS.filterComparison.LESS_THAN_OR_EQUAL
  private tempDropdownData: any[] = []
  public daysLeftContractEnd : number = 20;
  public OPERATOR_LIST = {
    [APP_ENUMS.filterComparison.LESS_THAN_OR_EQUAL]: {
      keySign: '<=',
      key: 'Less than or equal',
      value: APP_ENUMS.filterComparison.LESS_THAN_OR_EQUAL
    },
    [APP_ENUMS.filterComparison.GREATER_THAN_OR_EQUAL]: {
      keySign: '>=',
      key: 'Greater than or equal',
      value: APP_ENUMS.filterComparison.GREATER_THAN_OR_EQUAL
    },
    [APP_ENUMS.filterComparison.EQUAL]: {
      keySign: '=',
      key: 'Equal',
      value: APP_ENUMS.filterComparison.EQUAL
    }
  }

  public SENIORITY_LIST = [
    {
      key: 'Day',
      value: 1
    },
    {
      key: 'Month',
      value: 2
    },
    {
      key: 'Year',
      value: 3
    },

  ]
  public seniorityValue: string = "";
  public filterSenioryComparison: number = null;
  public filterOperatorsComparison:number = null;
  public seniorityType:number = APP_ENUMS.SeniorityType.Day;
  public seniorityTypeList = [];
  public teamMultiCtrl = [];
  public teamMultiFilterCtrl: FormControl = new FormControl();
  protected _onDestroy = new Subject<void>();
  public filterOperators = Object.values(this.OPERATOR_LIST);
  public filteredTeamsMulti: ReplaySubject<any[]> = new ReplaySubject<any[]>();
  public filterOption: boolean = false;
  public valueFilterByOperatorComparison: string = "";
  ngOnInit(): void {
    if (!isNull(this.defaultValue)) {
      this.dropdownFilterValue = this.defaultValue;
      this.dropdownMultiValueFilter = [this.defaultValue];

      if (this.filterType == APP_ENUMS.FilterTypeEnum.DatePicker) {
        this.datePickerValue = this.defaultValue;
      }
    }
    this.teamMultiFilterCtrl.valueChanges
      .pipe(takeUntil(this._onDestroy))
      .subscribe(() => {
        this.filterTeamsMulti();
      });
  }

  ngOnChanges(change: SimpleChanges) {
    if (change['dropdownData']) {
      this.tempDropdownData = this.dropdownData
      this.filteredTeamsMulti.next(this.dropdownData.slice());
      if(!isNull(this.defaultValue))
      {
        this.teamMultiCtrl.push(...this.dropdownData.filter(x => x.value == this.defaultValue))
        this.onSelectMulti()
      }
    }
  }

  onSearchEnter() {
    this.onTableFilter.emit(this.searchText)
  }

  onRadioFilter() {
    this.onTableFilter.emit(this.radioFilterValue)
  }

  onDropdownFilter() {
    this.onTableFilter.emit(this.dropdownFilterValue)
  }

  public onDropdownMultiFilter() {
    this.onTableMultiSelectFilter.emit(this.dropdownMultiValueFilter)
  }

  public onSelectMulti() {
    let teamIds = this.teamMultiCtrl.map((item) => item.value)
    let teamsFilterInput = {
      teamIds: teamIds,
      isAndCondition: this.filterOption
    } as TeamsFilterInputDto;
    this.onTableMultiSelectWithConditionFilter.emit(teamsFilterInput)
  }

  public selectTypeMultiFilter(event) {
    this.filterOption = event.target.value();
    this.onSelectMulti()
  }

  public onChangeFilterDaysLeftOfContractEnd(){
    this.onChangeDaysLeftOfContractEnd.emit({
      daysLeftContractEnd: this.daysLeftContractEnd
    });
  }

  onDatePickerFilter() {
    if (!this.datePickerValue) {
      this.onTableFilter.emit(
        {
          value: null,
          comparision: this.filterComparison
        }
      )
    }
    else {
      this.onTableFilter.emit(
        {
          value: moment(this.datePickerValue).format("YYYY-MM-DD"),
          comparision: this.filterComparison
        }
      )
    }
  }

  onComparisonChange() {
    if (!this.datePickerValue) {
      this.onTableFilter.emit(
        {
          value: null,
          comparision: this.filterComparison
        }
      )
      return;
    }
    switch (this.filterType) {
      case this.filterTypeEnum.DatePicker: {
        this.onTableFilter.emit(
          {
            value: moment(this.datePickerValue).format('YYYY-MM-DD'),
            comparision: this.filterComparison
          }
        )
        break;
      }
    }
  }

  onComparisonSeniorityChange() {
    this.onTableFilterBySeniority.emit(
      {
        comparison: this.filterSenioryComparison,
        seniorityType: this.seniorityType,
        seniorityValue: this.seniorityValue
      }
    );
  }
  onComparisonByOperatorChange(){
    this.onTableFilterByOperatorComparison.emit(
      {
        operatorComparison :this.filterOperatorsComparison,
        value : this.valueFilterByOperatorComparison
      } as FilterByComparisonDto
    )
  }

  onSearch() {
    this.dropdownData = this.tempDropdownData.filter(x => this.l(x.key.toLowerCase()).includes(this.l(this.dropdownSearchText.toLowerCase())))
  }

  public ngOnDestroy() {
    this._onDestroy.next();
    this._onDestroy.complete();
  }

  public filterTeamsMulti() {
    if (!this.dropdownData) {
      return;
    }
    let search = this.teamMultiFilterCtrl.value;
    if (!search) {
      this.filteredTeamsMulti.next(this.dropdownData.slice());
      return;
    } else {
      search = search.toLowerCase();
    }
    this.filteredTeamsMulti.next(
      this.dropdownData.filter(team => team.key.toLowerCase().indexOf(search) > -1)
    );
  }

  public clearSelected(item) {
    this.teamMultiCtrl.splice(this.teamMultiCtrl.indexOf(item), 1)
    let index = this.dropdownData.indexOf(item)
    let itemToinsert = { ...item }
    this.dropdownData.splice(this.dropdownData.indexOf(item), 1)
    this.dropdownData.splice(index, 0, itemToinsert)
    let search = this.teamMultiFilterCtrl.value;
    if (!search) {
      this.filteredTeamsMulti.next(this.dropdownData.slice());
      return;
    } else {
      search = search.toLowerCase();
    }
    this.filteredTeamsMulti.next(
      this.dropdownData.filter(team => team.key.toLowerCase().indexOf(search) > -1)
    );
  }
  public clearAll() {
    let a = [...this.dropdownData]
    this.teamMultiCtrl = []
    this.filteredTeamsMulti.next(a);
  }

  clearSearchText() {
    if (this.dropdownData.length == 0) {
      this.dropdownSearchText = "";
      this.dropdownData = [...this.tempDropdownData]
    }
  }
}


