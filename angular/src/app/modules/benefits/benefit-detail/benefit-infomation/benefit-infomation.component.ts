import { MatDialog } from '@angular/material/dialog';
import { Component, Injector, OnInit, EventEmitter, Output } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { BenefitService } from '@app/service/api/benefits/benefit.service';
import { benefitDto } from '@app/service/model/benefits/beneft.dto';
import { AppComponentBase } from '@shared/app-component-base';
import { CreateEditBenefitDialogComponent } from '../../create-edit-benefit-dialog/create-edit-benefit-dialog.component';
import { BenefitType } from '@shared/AppConsts';
import { PERMISSIONS_CONSTANT } from '@app/permission/permission';
@Component({
  selector: 'app-benefit-infomation',
  templateUrl: './benefit-infomation.component.html',
  styleUrls: ['./benefit-infomation.component.css']
})
export class BenefitInfomationComponent extends AppComponentBase implements OnInit {
  @Output() onUpdate: EventEmitter<boolean> = new EventEmitter()
  private benefitId: number;
  public benefitName: string = ""
  public benefit = {} as benefitDto;
  public readMode: boolean = true;
  public isActive: boolean = true;
  public benefitTypeList: any[] = []
  constructor(injector: Injector, private router: Router,
    private benefitService: BenefitService,
    private route: ActivatedRoute,
    private dialog: MatDialog
  ) {
    super(injector)
  }

  ngOnInit(): void {
    this.benefitTypeList = this.getListFormEnum(BenefitType, true)
    this.benefitName = this.route.snapshot.queryParamMap.get("name")
    this.benefitId = Number(this.route.snapshot.queryParamMap.get("id"));
    this.isActive = this.route.snapshot.queryParamMap.get("active") == "true" ? true : false
    this.getBenefitDetail();
  }

  public getBenefitDetail(): void {
    this.subscription.push(this.benefitService.get(this.benefitId).subscribe((res) => {
      this.benefit = res.result;
      if (this.benefit.type == this.APP_ENUM.BenefitType.CheDoChung) {
        this.benefit.applyDate = this.formatDateYMD(new Date())
      }
    }))
  }

  public onEdit(): void {
    this.readMode = false
  }

  public saveAndClose(): void {
    this.isLoading = true;
    if (this.benefit.type == this.APP_ENUM.BenefitType.CheDoChung) this.benefit.applyDate = null
    if (this.benefit.applyDate) this.benefit.applyDate = this.formatDateYMD(this.benefit.applyDate)
    this.subscription.push(this.benefitService.update(this.benefit).subscribe(rs => {
      this.isLoading = false;
      this.readMode = true;
      abp.notify.success(`Update benefit successfull`);
      this.getBenefitDetail()
      this.onUpdate.emit(true)
    }, () => {
      this.isLoading = false
    }))
  }

  onClone() {
    let item = { ...this.benefit }
    let ref = this.dialog.open(CreateEditBenefitDialogComponent, {
      data: {
        benefit: item,
        isClone: true,
      },
      width: "700px"
    })
    ref.afterClosed().subscribe(rs => {
      if (rs) {
        this.navigateListBenefit()
      }
    })
  }

  onDelete() {
    abp.message.confirm(
      `Delete benefit <strong>${this.benefit.name}</strong>`,
      "",
      async (result: boolean) => {
        if (result) {
          this.subscription.push(
            this.benefitService.delete(this.benefit.id).subscribe(rs => {
              abp.notify.success(`Deleted benefit ${this.benefit.name}`)
              this.navigateListBenefit()
            })
          )
        }
      },
      true
    )
  }

  onCancel() {
    this.readMode = true;
    this.getBenefitDetail()
  }

  public navigateListBenefit() {
    this.router.navigate(['/app/benefits/list-benefit'])
  }
  isShowEditBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Benefit_BenefitDetail_TabInformation_Edit);
  }
  isShowDeleteBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Benefit_BenefitDetail_TabInformation_Delete);
  }
  isShowCloneBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Benefit_BenefitDetail_TabInformation_Clone);
  }
  isAllowRoutingTabInfo(){
    return this.isGranted(PERMISSIONS_CONSTANT.Benefit_BenefitDetail_TabInformation_View);
  }
}
