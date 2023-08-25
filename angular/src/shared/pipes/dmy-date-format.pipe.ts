import { Pipe, PipeTransform } from '@angular/core';
import { DatePipe } from '@angular/common';

@Pipe({
  name: 'DMYDateFormat'
})
export class DMYDateFormatPipe implements PipeTransform {

  transform(date:Date | string) {
    return new DatePipe('en-US').transform(date, "dd/MM/yyyy");
  }
}
