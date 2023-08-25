import { NgxMatDateFormats } from '@angular-material-components/datetime-picker';
import { Injectable } from '@angular/core'
import { NativeDateAdapter } from '@angular/material/core'
import * as moment from 'moment'
@Injectable({
    providedIn: 'root'
})
export class CustomDateAdapter extends NativeDateAdapter {
    format(date: Date, displayFormat: Object): string {
        return moment(date).format('DD/MM/YYYY')
    }
}

export const APP_DATE_FORMATS = {
    parse:
    {
        dateInput: 'DD/MM/YYYY',
    },
    display: {
        dateInput: 'DD/MM/YYYY',
        monthYearLabel: 'MM/YYYY',
        dateA11yLabel: 'DD/MM/YYYY',
        monthYearA11yLabel: 'MMMM YYYY',
    }
};
export const CUSTOM_MAT_DATE_FORMATS: NgxMatDateFormats = {
    parse: {
        dateInput: 'DD/MM/YYYY HH:mm'
    },
    display: {
      dateInput: 'DD/MM/YYYY HH:mm',
      monthYearLabel: 'MM YYYY',
      dateA11yLabel: 'DD/MM/YYYY HH:mm',
      monthYearA11yLabel: 'MMMM YYYY',
    }
  };