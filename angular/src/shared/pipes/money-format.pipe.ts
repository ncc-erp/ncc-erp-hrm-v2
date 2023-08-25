import { DecimalPipe } from '@angular/common';
import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'moneyFormat'
})
export class MoneyFormatPipe implements PipeTransform {

  transform(money: number | string) {
    return new DecimalPipe('en-US').transform(money, "1.0");
  }

}
