import { Pipe, PipeTransform } from '@angular/core';
import { AppConsts } from '@shared/AppConsts';

@Pipe({
  name: 'userLevel'
})
export class UserLevelPipe implements PipeTransform {

  transform(userLevelEnum: number, isStyle: boolean) {
    return isStyle ? AppConsts.userLevel[userLevelEnum].style : AppConsts.userLevel[userLevelEnum].name
  }
}
