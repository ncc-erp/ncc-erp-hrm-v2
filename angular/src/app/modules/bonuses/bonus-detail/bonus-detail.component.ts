import { Component, Injector, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { PERMISSIONS_CONSTANT } from '@app/permission/permission';
import { BonusService } from '@app/service/api/bonuses/bonus.service';
import { BonusDto } from '@app/service/model/bonuses/bonus.dto';
import { AppComponentBase } from '@shared/app-component-base';

@Component({
  selector: 'app-bonus-detail',
  templateUrl: './bonus-detail.component.html',
  styleUrls: ['./bonus-detail.component.css']
})
export class BonusDetailComponent extends AppComponentBase implements OnInit {

  currentUrl: string = "";
  requestId: any;
  public name: string = ""
  public applyMonth: string = ""
  public isActive:boolean = true;
  public bonus = {} as BonusDto;

  constructor(
    private route: ActivatedRoute, 
    private router: Router, 
    injector: Injector,
    private bonusService: BonusService,
    ) 
  {
    super(injector)
  }

  ngOnInit(): void {
    this.currentUrl = this.router.url
    this.router.events.subscribe(res => this.currentUrl = this.router.url)
    this.requestId = this.route.snapshot.queryParamMap.get("id");
    this.getBonusById();
  }

  public routingInformationTab() {
    this.router.navigate(['bonus-information'], {
      relativeTo: this.route,
      queryParams: {
        id: this.requestId,
      },
    })
  }
  public routingEmployeeTab() {
    this.router.navigate(['bonus-employee'], {
      relativeTo: this.route,
      queryParams: {
        id: this.requestId,
      },
    })
  }

  public getBonusById() {
    this.bonusService.getBonusDetail(Number(this.requestId)).subscribe((res) => {
      this.bonus = res.result;
      this.listBreadCrumb = [
      {name: '<i class="fa-solid fa-house fa-sm"></i>',url:''}, 
      {name: ' <i class="fa-solid fa-chevron-right"></i> '},
      {name:'Bonus',url:'/app/bonuses/list-bonus'},
      {name: '<i class="fa-solid fa-chevron-right fa-sm"></i>'} , 
      {name: this.bonus.name }]
    })
  }
  isAllowRoutingTabInfo(){
    return this.isGranted(PERMISSIONS_CONSTANT.Bonus_BonusDetail_TabInformation_View);
  }

  isAllowRoutingTabEmployee(){
    return this.isGranted(PERMISSIONS_CONSTANT.Bonus_BonusDetail_TabEmployee_View)
  }
}
