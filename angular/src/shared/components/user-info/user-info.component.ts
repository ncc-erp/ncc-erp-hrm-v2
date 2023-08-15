import { BadgeInfoDto, BaseEmployeeDto } from './../../dto/user-infoDto';
import { AppComponentBase } from '@shared/app-component-base';
import { Component, Input, Injector } from '@angular/core';
import { RouterLink } from '@angular/router';
import { GetEmployeeDto } from '@app/service/model/employee/employee.dto';

@Component({
  selector: 'app-user-info',
  templateUrl: './user-info.component.html',
  styleUrls: ['./user-info.component.css']
})
export class UserInfoComponent extends AppComponentBase {
  @Input() userData: UserData | unknown
  @Input() link: RouterLink
  @Input() queryParams: Object
  @Input() isAllowRouting : boolean
  @Input() isAllowAvatarRouting: boolean
  public user: UserData
  constructor(injector: Injector) {
    super(injector)
  }

  ngOnChanges(): void {
    this.user = this.userData as UserData;
  }

  getAvatar(member) {
    if(member.avatarFullPath) {
      return  member.avatarFullPath;
    }
    if(member.sex == 2 ){
      return 'assets/img/women.png';
    }
    return 'assets/img/men.png';
  }
}

export interface UserData{
  id: number,
  fullName: string,
  sex: number,
  branchInfo: BadgeInfoDto,
  userTypeInfo: BadgeInfoDto,
  jobPositionInfo: BadgeInfoDto,
  levelInfo: BadgeInfoDto,
  email: string
}