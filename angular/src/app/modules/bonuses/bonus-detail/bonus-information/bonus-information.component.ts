import { Component, Injector, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { PERMISSIONS_CONSTANT } from '@app/permission/permission';
import { BonusService } from '@app/service/api/bonuses/bonus.service';
import { BonusDto, EditBonusDto, GetBonusDetailDto } from '@app/service/model/bonuses/bonus.dto';
import { AppComponentBase } from '@shared/app-component-base';
import * as moment from 'moment';

@Component({
  selector: 'app-bonus-information',
  templateUrl: './bonus-information.component.html',
  styleUrls: ['./bonus-information.component.css']
})
export class BonusInformationComponent extends AppComponentBase implements OnInit {

  private bonusId: number;
  public bonus = {} as GetBonusDetailDto;
  public readMode: boolean = true;
  public isApply: boolean = false;
  public listDate : string[] = [];
  public bonusStatusList: string[] = Object.keys(this.APP_ENUM.BonusStatus)
  constructor(injector: Injector,
    private bonusService: BonusService,
    private route: ActivatedRoute
  ) {
    super(injector)
  }

  ngOnInit(): void {
    this.bonusId = Number(this.route.snapshot.queryParamMap.get("id"));
    this.getBonusDetail();
    this.getListDate();
  }

  public getBonusDetail(): void {
    this.bonusService.getBonusDetail(this.bonusId).subscribe((res) => {
      this.bonus = res.result;
      this.bonus.applyMonth = this.formatDateMY(res.result.applyMonth);
    })
  }

  getListDate() {
    this.bonusService.getListDate().subscribe((res) => {
      this.listDate = res.result.map(date => {
        return moment(date).format("MM/YYYY");
      });
    })
  }

  public editRequest(): void {
    this.readMode = false
  }

  public saveAndClose(): void {
    this.isLoading=true;
    let item = {} as EditBonusDto;
    item = {
      id: this.bonus.id,
      name: this.bonus.name,
      isApply: this.isApply,
      isActive: this.bonus.isActive,
      applyMonth: this.bonus.applyMonth
    }
    this.subscription.push(
      this.bonusService.update(item).subscribe(rs => {
      this.isLoading=false;
      this.readMode = true;
      this.getBonusDetail();
      abp.notify.success(`Update bonus successfull`);
      },() => this.isLoading = false)
    )
  }

  isShowEditBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Bonus_BonusDetail_TabInformation_Edit);
  }
  isAllowRoutingTabInfo(){
    return this.isGranted(PERMISSIONS_CONSTANT.Bonus_BonusDetail_TabInformation_View);
  }
}
