import { Pipe, PipeTransform } from '@angular/core';
import { AppConsts } from '@shared/AppConsts';

@Pipe({
  name: 'isActive'
})
export class IsActivePipe implements PipeTransform {

  transform(isActive: boolean, isStyle: boolean) {
    return isStyle ? AppConsts.punishmentStatus[Number(isActive)]?.class : AppConsts.punishmentStatus[Number(isActive)]?.name
  }

}
