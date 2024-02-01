import { Component, Injector, Input, OnInit } from '@angular/core';
import { RouterLink } from '@angular/router';
import { AppComponentBase } from '@shared/app-component-base';

@Component({
  selector: 'app-user-data-for-chart-detail',
  templateUrl: './user-data-for-chart-detail.component.html',
  styleUrls: ['./user-data-for-chart-detail.component.css']
})
export class UserDataForChartDetailComponent extends AppComponentBase {
  @Input() userData: UserDataForChartDetail | unknown
  @Input() link: RouterLink
  @Input() queryParams: Object
  @Input() isAllowRouting : boolean
  @Input() isAllowAvatarRouting: boolean
  public user: UserDataForChartDetail
  constructor(injector: Injector) {
    super(injector)
  }

  ngOnChanges(): void {
    this.user = this.userData as UserDataForChartDetail;
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

export interface UserDataForChartDetail{
  id: number,
  fullName: string,
  sex: number,
  avatarFullPath: string;
  email: string
}
