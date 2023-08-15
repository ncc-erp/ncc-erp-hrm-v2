import { Component, Injector, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { PERMISSIONS_CONSTANT } from '@app/permission/permission';
import { BenefitService } from '@app/service/api/benefits/benefit.service';
import { benefitDto } from '@app/service/model/benefits/beneft.dto';
import { AppComponentBase } from '@shared/app-component-base';
import { BenefitInfomationComponent } from './benefit-infomation/benefit-infomation.component';
@Component({
  selector: 'app-benefit-detail',
  templateUrl: './benefit-detail.component.html',
  styleUrls: ['./benefit-detail.component.css']
})
export class BenefitDetailComponent extends AppComponentBase implements OnInit {

  public currentUrl: string = ""
  private id: number
  public name: string = ""
  public typeName: string = ""
  public money: string = ""
  private isActive: boolean = true;
  public benefit: benefitDto = {} as benefitDto
  public initListBreadCrumb = [
    { name: '<i class="fa-solid fa-house fa-sm"></i>', url: '/app/home' },
    { name: ' <i class="fa-solid fa-chevron-right"></i> ', url: null },
    { name: ' Benefit ', url: '/app/benefits/list-benefit' },
    { name: ' <i class="fa-solid fa-chevron-right"></i> ', url: null },
  ];
  public listBreadCrumb = []
  constructor(private route: ActivatedRoute, private router: Router, injector: Injector, private benefitService: BenefitService) {
    super(injector)

  }

  ngOnInit(): void {
    this.currentUrl = this.router.url
    this.router.events.subscribe(res => this.currentUrl = this.router.url)
    this.id = Number(this.route.snapshot.queryParamMap.get("id"));
    this.name = this.route.snapshot.queryParamMap.get("name");
    this.typeName = this.route.snapshot.queryParamMap.get("typeName");
    this.money = this.route.snapshot.queryParamMap.get("money");
    this.isActive = this.route.snapshot.queryParamMap.get("active") == "true" ? true : false
    this.listBreadCrumb = [...this.initListBreadCrumb]
    this.getBenefitInfo();
  }

  public routingInformationTab() {
    this.router.navigate(['benefit-infomation'], {
      relativeTo: this.route,
      queryParams: {
        id: this.id,
        name: this.name,
        active: this.isActive
      },
    })
  }

  public routingEmployeeTab() {
    this.router.navigate(['benefit-employee'], {
      relativeTo: this.route,
      queryParams: {
        id: this.id,
        name: this.benefit.name,
        active: this.benefit.isActive,
        type: this.benefit.type
      },
    })
  }

  getBenefitInfo() {
    this.benefitService.get(this.id).subscribe(rs => {
      this.benefit = rs.result
      this.listBreadCrumb = [...this.initListBreadCrumb, {
        name: `${this.benefit.name} - <b>${this.benefit.money.toLocaleString()}</b> VND - ${this.benefit.benefitTypeName}`, url: null },
    ]
    })
  }

  onActive(elementRef) {
    if(elementRef instanceof BenefitInfomationComponent){
      elementRef.onUpdate.subscribe(value => {
        this.getBenefitInfo()
      })
    }
  }

  isAllowRoutingTabInfo(){
    return this.isGranted(PERMISSIONS_CONSTANT.Benefit_BenefitDetail_TabInformation_View);
  }

  isAllowRoutingTabEmployee(){
    return this.isGranted(PERMISSIONS_CONSTANT.Benefit_BenefitDetail_TabEmployee_View);
  }
}
