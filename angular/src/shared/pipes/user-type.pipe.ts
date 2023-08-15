import { Pipe, PipeTransform } from '@angular/core';
import { AppConsts } from '@shared/AppConsts';

@Pipe({
  name: 'userType'
})
export class UserTypePipe implements PipeTransform {

  transform(userTypeEnum: number, isStyle: boolean) {
    return isStyle ? AppConsts.userType[userTypeEnum].class : AppConsts.userType[userTypeEnum].name
  }
}
