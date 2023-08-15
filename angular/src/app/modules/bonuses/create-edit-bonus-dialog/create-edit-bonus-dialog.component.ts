import { Component, Injector, OnInit } from '@angular/core';
import { DialogComponentBase } from '@shared/dialog-component-base';
import * as moment from 'moment';
import { BonusService } from '../../../service/api/bonuses/bonus.service';
import { BonusDto, EditBonusDto } from '../../../service/model/bonuses/bonus.dto';
@Component({
  selector: 'app-create-edit-bonus-dialog',
  templateUrl: './create-edit-bonus-dialog.component.html',
  styleUrls: ['./create-edit-bonus-dialog.component.css']
})

export class CreateEditBonusDialogComponent extends DialogComponentBase<BonusDto> implements OnInit {
  public bonus = {} as BonusDto;
  public isApply: boolean = false;
  listDate: any = [];
  constructor(injector: Injector, private bonusService: BonusService) {
    super(injector)
  }

  ngOnInit(): void {
    this.getListDate();
    if (this.dialogData?.id) {
      this.bonus = this.dialogData
      this.title = `Edit bonus <strong>${this.dialogData.name}</strong>`
      this.bonus.applyMonth = this.formatDateMY(this.dialogData.applyMonth);
    }
    else {
      this.title = "Create new bonus"
      this.bonus.applyMonth = this.dialogData.applyMonth
    }
  }

  chooseDate(date) {
    this.bonus.applyMonth = date;
  }

  getListDate() {
    this.bonusService.getListDate().subscribe((res) => {
      this.listDate = res.result.map(date => {
        return moment(date).format("MM/YYYY");
      });
    })
  }

  getListMonthAgoFilter() {
    this.bonusService.getListMonthFilter().subscribe((res) => {
      this.listDate = res.result;
      this.listDate = res.result.map(x => {
        return {
          key: moment(x).format("MM/YYYY"),
          value: moment(x).format("DD/MM/YYYY"),
        }
      });
      this.listDate.unshift({ key: "All", value: "all" })
    })
  }
  saveAndClose() {
    this.trimData(this.bonus);
    if (this.dialogData?.id) {
      let item = {} as EditBonusDto;
      item = {
        id:this.bonus.id,
        name:this.bonus.name,
        isApply:this.isApply,
        isActive:this.bonus.isActive,
        applyMonth : this.bonus.applyMonth
      }
      this.subscription.push(this.bonusService.update(item).subscribe(rs => {
        abp.notify.success(`Update bonus successfull`)
        this.dialogRef.close(true)
      }))
    }
    else {
      this.subscription.push(
        this.bonusService.create(this.bonus).subscribe(rs => {
          abp.notify.success(rs.result)
          abp.notify.success(`Created new bonus ${this.bonus.name}`)
          this. getListMonthAgoFilter();
          this.dialogRef.close(true)
        })
      )
    }
  }
}
