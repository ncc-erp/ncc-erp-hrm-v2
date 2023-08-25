import { Component, Injector, OnInit } from '@angular/core';
import { PunishmentFundsService } from '@app/service/api/punishment-funds/punishment-funds.service';
import { PunishmentFundDto } from '@app/service/model/punishment-fund/punishment-fund.dto';
import { DialogComponentBase } from '@shared/dialog-component-base';
import * as moment from 'moment';

@Component({
  selector: 'app-create-edit-disburse',
  templateUrl: './create-edit-disburse.component.html',
  styleUrls: ['./create-edit-disburse.component.css']
})
export class CreateEditDisburseComponent extends DialogComponentBase<any> implements OnInit {

  public title: string = "";
  public isAllowNavigateNumber: boolean = true;
  public punishmentFund = {} as PunishmentFundDto;
  public dialogType ={
    ADD : 1,
    DISBURSE: 2,
    EDIT: 3
  }
  constructor(injector: Injector,
    private punishmentFundService: PunishmentFundsService) {
    super(injector);
  }

  ngOnInit(): void {
    this.title = this.dialogData.title;
    if (this.dialogData.punishmentFund) {
      this.punishmentFund = this.dialogData.punishmentFund;
      console.log(this.punishmentFund)
    }
    if (this.dialogData.title.includes("Disburse new punishment fund")) {
      this.isAllowNavigateNumber = false;
    }
  }
  saveAndClose() {
    let type = this.dialogData.type;
    let input = {
      date : moment(this.punishmentFund.date).format("YYYY/MM/DD"),
      id: this.punishmentFund.id,
      amount : this.punishmentFund.amount,
      note: this.punishmentFund.note

    } as PunishmentFundDto;
    if(type == this.dialogType.ADD){
      this.onAdd(input);
      return;
    }
    if(type == this.dialogType.DISBURSE){
      this.onDisburse(input);
      return;
    }
    if(type == this.dialogType.EDIT){
      this.onEdit(input);
      return;
    }

  }
  onAdd(input) {
    this.isLoading = true;
    this.subscription.push(
      this.punishmentFundService.add(input).subscribe((rs) => {
        if(rs){
          abp.notify.success("Add punishment fund successful");
          this.isLoading = false;
          this.dialogRef.close(true);
        }
      },()=> this.isLoading = false)
    )
  }
  onDisburse(input){
    this.isLoading = true;
    this.subscription.push(
      this.punishmentFundService.disburse(input).subscribe((rs) => {
        if(rs){
          abp.notify.success("Disburse punishment fund successful");
          this.isLoading = false;
          this.dialogRef.close(true);
        }
      },()=> this.isLoading = false)
    )
  }

  onEdit(input){
    this.isLoading = true;
    this.subscription.push(
      this.punishmentFundService.update(input).subscribe((rs) => {
        if(rs){
          abp.notify.success("Update punishment fund successful");
          this.isLoading = false;
          this.dialogRef.close(true);
        }
      },()=> this.dialogRef.close(false),()=> this.isLoading = false)
    )
  }

  

}
