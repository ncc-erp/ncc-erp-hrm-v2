import { Component, EventEmitter, Injector, Input, OnInit, Output } from '@angular/core';
import { FormControl } from '@angular/forms';
import { MAT_MOMENT_DATE_ADAPTER_OPTIONS, MomentDateAdapter } from '@angular/material-moment-adapter';
import { DateAdapter, MAT_DATE_FORMATS, MAT_DATE_LOCALE } from '@angular/material/core';
import { MatDialog } from '@angular/material/dialog';
import { AppComponentBase } from '@shared/app-component-base';
import { DATE_TIME_OPTIONS } from '@shared/AppConsts';
import { APP_ENUMS } from '@shared/AppEnums';
import * as moment from 'moment';
import { CustomTimeComponent } from '../custom-time/custom-time.component';
const DD_MM = {
  parse: {
    dateInput: 'DD/MM',
  },
  display: {
    dateInput: 'DD/MM',
    monthYearLabel: 'DD/MM',
    dateA11yLabel:'DD/MM',
    monthYearA11yLabel: 'DD/MM'
  },
}
@Component({
  selector: 'app-date-selector',
  templateUrl: './date-selector.component.html',
  styleUrls: ['./date-selector.component.css'],
  providers: [{
    provide: DateAdapter,
    useClass: MomentDateAdapter,
    deps: [MAT_DATE_LOCALE, MAT_MOMENT_DATE_ADAPTER_OPTIONS],
  },
  ],
})

export class DateSelectorComponent extends AppComponentBase implements OnInit {

  constructor(injector: Injector,
    public dialog: MatDialog) {
    super(injector);
  }

  @Output() onDateSelectorChange: EventEmitter<{ fromDate, toDate }> = new EventEmitter<{ fromDate, toDate }>();
  @Input() label: string;
  @Input() dropdownData: any[] = [];
  @Input() defaultValue: any;
  @Input() mode: number;
  @Input() fDate: any;
  @Input() tDate: any;

  public viewChange: FormControl = new FormControl();
  public activeView;
  public textDate: string;
  public listDateOptions = [];
  public DateTimeOptions = APP_ENUMS.DATE_TIME_OPTIONS;
  public currentValue;
  public isCancelPopup: boolean = false;
  public date = new Date();
  public isDateChange: boolean = false;
  private displayDateFormat:string = "DD/MM/YYYY"

  ngOnInit(): void {
    this.listDateOptions = this.getListFormEnum(DATE_TIME_OPTIONS, true).filter(x => this.dropdownData.includes(x.value));
    this.viewChange.setValue(this.defaultValue);
    this.changeView(true, this.fDate, this.tDate);
    if(this.mode == APP_ENUMS.DateFilterMode.Birthday){
      this.displayDateFormat = "DD/MM"
    }
  }
  changeView(reset?: boolean, fDate?: any, tDate?: any) {
    if (reset) {
      this.activeView = 0;
    }

    let fromDate, toDate;

    if (this.viewChange.value === APP_ENUMS.DATE_TIME_OPTIONS.All) {
      fromDate = '';
      toDate = '';
      this.isCancelPopup = false;
    }
    if (this.viewChange.value === APP_ENUMS.DATE_TIME_OPTIONS.Day) {
      if (fDate) {
        fromDate = fDate;
        toDate = tDate;
      } else {
        fromDate = moment().startOf('D').add(this.activeView, 'D');
        toDate = moment(fromDate).endOf('D');
        this.date = fromDate;
      }
    }
    if (this.viewChange.value === APP_ENUMS.DATE_TIME_OPTIONS.Week) {
      fromDate = moment().startOf('isoWeek').add(this.activeView, 'w');
      toDate = moment(fromDate).endOf('isoWeek');

    }
    if (this.viewChange.value === APP_ENUMS.DATE_TIME_OPTIONS.Month) {
      fromDate = moment(fDate).startOf('M').add(this.activeView, 'M');
      toDate = moment(fromDate).endOf('M');

    }
    if (this.viewChange.value === APP_ENUMS.DATE_TIME_OPTIONS.Quarter) {
      fromDate = moment().startOf('Q').add(this.activeView, 'Q');
      toDate = moment(fromDate).endOf('Q');
    }
    if (this.viewChange.value === APP_ENUMS.DATE_TIME_OPTIONS.Year) {
      fromDate = moment().startOf('y').add(this.activeView, 'y');
      toDate = moment(fromDate).endOf('y');
    }

    if (this.viewChange.value === APP_ENUMS.DATE_TIME_OPTIONS.CustomTime) {
      fromDate = '';
      toDate = '';
      if (!reset && fDate && tDate) {
        fromDate = fDate;
        toDate = tDate;
        this.isCancelPopup = false;

      } else if (!this.isCancelPopup) {
        this.textDate = '';
        return;
      }
    }

    if (fromDate && toDate) {
      this.getDateText(this.viewChange.value, fromDate, toDate);
    }

    fromDate = fromDate === '' ? '' : fromDate?.format('YYYY-MM-DD');
    toDate = toDate === '' ? '' : toDate?.format('YYYY-MM-DD');

    this.onDateSelectorChange.emit({ fromDate, toDate });
    this.currentValue = { fromDate, toDate }

  }

  public getDateText(viewChange, fromDate, toDate) {
    switch (viewChange) {
      case APP_ENUMS.DATE_TIME_OPTIONS.Week: {
        this.textDate = fromDate?.format('DD') + ' - ' + toDate?.format(this.displayDateFormat);
        break;
      };
      case APP_ENUMS.DATE_TIME_OPTIONS.Month: {
        if (this.mode == APP_ENUMS.DateFilterMode.HomeMonthYear) {
          this.textDate = fromDate?.format('MM/YYYY');
          break;
        }
        this.textDate = fromDate.format('MM');
        break;
      };
      case APP_ENUMS.DATE_TIME_OPTIONS.Quarter: {
        this.textDate = fromDate?.format('DD/MM') + ' - ' + toDate?.format(this.displayDateFormat);
        break;
      }
      case APP_ENUMS.DATE_TIME_OPTIONS.CustomTime: {
        this.textDate = fromDate?.format(this.displayDateFormat) + ' - ' + toDate?.format(this.displayDateFormat);
        break;
      }
      case APP_ENUMS.DATE_TIME_OPTIONS.Year: {
        this.textDate = fromDate?.format('YYYY');
        break;
      }
    }
  }

  public dateChange(data) {
    this.viewChange.setValue(APP_ENUMS.DATE_TIME_OPTIONS.Day);
    this.changeView(true, data.value, data.value);

  }

  public onShowCustomTimePopup() {
    var dialogRef = this.dialog.open(CustomTimeComponent, {
      width: "500px",
      data: {
        mode: this.mode
      }
    })
    dialogRef.afterClosed().subscribe((rs) => {
      if (rs && rs.data) {
        this.changeView(false, rs.data?.from, rs.data?.to);
      } else {
        this.isCancelPopup = true;
        this.viewChange.setValue(APP_ENUMS.DATE_TIME_OPTIONS.All);
        this.changeView(false)
      }
    })
  }

  public nextOrPre(title: any) {
    if (title === 'pre') {
      this.activeView--;
    }
    if (title === 'next') {
      this.activeView++;
    }
    this.changeView();
  }
}
