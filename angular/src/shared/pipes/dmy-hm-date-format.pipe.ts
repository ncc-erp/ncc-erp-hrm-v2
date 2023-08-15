import { DatePipe } from '@angular/common';
import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'dmyHmDateFormat'
})
export class DmyHmDateFormatPipe implements PipeTransform {

  transform(date:Date | string) {
    return new DatePipe('en-US').transform(date, "dd/MM/yyyy H:mm");
  }
}
