import { Injector } from '@angular/core';
import { Component, OnInit } from '@angular/core';
import { BenefitService } from '@app/service/api/benefits/benefit.service';
import { benefitDto } from '@app/service/model/benefits/beneft.dto';
import { CloneBenefitDto } from '@app/service/model/benefits/CloneBenefitDto';
import { BenefitType } from '@shared/AppConsts';
import { DialogComponentBase } from '@shared/dialog-component-base';

@Component({
  selector: 'app-create-edit-benefit-dialog',
  templateUrl: './create-edit-benefit-dialog.component.html',
  styleUrls: ['./create-edit-benefit-dialog.component.css']
})
export class CreateEditBenefitDialogComponent extends DialogComponentBase<CreateEditBenefitDialogData> implements OnInit {
  public benefit = {} as benefitDto
  public isCloneEmployee: boolean = true
  public listBenefitType = []
  constructor(injector: Injector, private benefitService: BenefitService) {
    super(injector)
  }

  ngOnInit(): void {
    this.benefit = this.dialogData?.benefit?.name ? this.dialogData.benefit : {} as benefitDto
    this.listBenefitType = this.getListFormEnum(BenefitType, true)
    if (this.dialogData?.benefit?.id) {
      this.title = `Edit benefit <strong>${this.dialogData.benefit.name}</strong>`
    }
    else {
      this.title = "Create new benefit"
    }
    if (this.dialogData?.isClone) {
      this.title = `Clone benefit <strong>${this.benefit.name}</strong>`
    }
  }

  saveAndClose() {
    this.trimData(this.benefit)
    this.benefit.applyDate = this.benefit.applyDate ? this.formatDateYMD(this.benefit.applyDate) : null
    if (this.dialogData?.isClone) {
      let cloneBenefit = {
        benefitId: this.benefit.id,
        applyDate: this.benefit.applyDate,
        isActive: this.benefit.isActive,
        isBelongToAllEmployee: this.benefit.isBelongToAllEmployee,
        money: this.benefit.money,
        name: this.benefit.name,
        type: this.benefit.type,
        isCloneEmployee: this.isCloneEmployee
      } as CloneBenefitDto
      this.benefitService.CloneBenefit(cloneBenefit).subscribe(rs => {
        abp.notify.success(`Benefit cloned`);
        this.dialogRef.close(true)
      })
    } else {
      if (this.dialogData?.benefit?.id) {
        this.subscription.push(this.benefitService.update(this.benefit).subscribe(rs => {
          abp.notify.success(`Update benefit successfull`)
          this.dialogRef.close(true)
        }))
      }
      else {
        delete this.benefit["id"]
        this.subscription.push(
          this.benefitService.create(this.benefit).subscribe(rs => {
            abp.notify.success(`Created new benefit ${this.benefit.name}`)
            this.dialogRef.close(true)
          })
        )
      }
    }
  }

}

export interface CreateEditBenefitDialogData {
  benefit: benefitDto,
  isClone: boolean
}

