import { Pipe, PipeTransform } from '@angular/core';
import { AppConsts } from '@shared/AppConsts';

@Pipe({
  name: 'userStatus'
})
export class UserStatusPipe implements PipeTransform {

  transform(userStatusEnum: number, isStyle: boolean) {
    return isStyle ? AppConsts.userStatus[userStatusEnum]?.class : AppConsts.userStatus[userStatusEnum]?.name
  }

}
