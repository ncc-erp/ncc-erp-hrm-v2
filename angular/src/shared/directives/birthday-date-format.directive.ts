import { Directive } from '@angular/core';
import { MAT_DATE_FORMATS } from '@angular/material/core';

@Directive({
  selector: '[BirthdayDateFormat]',
  providers: [
    {
      provide: MAT_DATE_FORMATS,
      useValue: {
        parse:
        {
            dateInput: 'DD/MM',
        },
        display: {
            dateInput: 'DD/MM',
            monthYearLabel: 'MM/YYYY',
            dateA11yLabel: 'DD/MM',
            monthYearA11yLabel: 'MMMM YYYY',
        }
      },
    },

  ]
})
export class BirthdayDateFormatDirective {

  constructor() { }

}
