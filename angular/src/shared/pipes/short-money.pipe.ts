import { Pipe, PipeTransform } from '@angular/core'
@Pipe({
    name: 'shortMoney'
})
export class ShortMoneyPipe implements PipeTransform {
    transform(value: string | number, digitsAfterComma: number = 2) {
        if(Number(value)) {
            const num = Number(value)
            const lookup = [
                { value: 1, symbol: "" },
                { value: 1e3, symbol: "k" },
                { value: 1e6, symbol: "M" },
                { value: 1e9, symbol: "B" },
              ];
            for(const item of lookup.reverse()){
                if( num >= item.value ){
                    return (num/ item.value).toFixed(digitsAfterComma) + item.symbol
                }
            }
        }
        return value
    }
}

