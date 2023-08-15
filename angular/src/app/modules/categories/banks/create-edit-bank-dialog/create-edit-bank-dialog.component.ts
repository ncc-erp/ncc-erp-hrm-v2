import { Component, Injector, OnInit } from '@angular/core';
import { BankService } from '@app/service/api/categories/bank.service';
import { BankDto } from '@app/service/model/categories/bank.dto';
import { DialogComponentBase } from '@shared/dialog-component-base';
import { startWithTap } from '@shared/helpers/observerHelper';
import { finalize } from 'rxjs/operators'
@Component({
  selector: 'app-create-edit-bank-dialog',
  templateUrl: './create-edit-bank-dialog.component.html',
  styleUrls: ['./create-edit-bank-dialog.component.css']
})
export class CreateEditBankDialogComponent extends DialogComponentBase<BankDto> implements OnInit {
  public bank = {} as BankDto;
  constructor(injector: Injector, private bankService: BankService) {
    super(injector)
  }

  ngOnInit(): void {
    if (this.dialogData?.id) {
      this.bank = this.dialogData
      this.title = `Edit bank ${this.bank.name}`;
    } else {
      this.title = 'Create new bank';
    }
  }
  
  nameChange(event){
    if(!this.dialogData?.id){
      this.bank.code = event
    }
  }

  saveAndClose() {
    this.trimData(this.bank)
    if (this.dialogData?.id) {
      this.subscription.push(
        this.bankService.update(this.bank)
        .pipe(startWithTap(() => { this.isLoading = true }))
        .pipe(finalize(() => { this.isLoading = false }))
        .subscribe(rs => {
          abp.notify.success(`Update bank successful`)
          this.dialogRef.close(true)
        }))
    }
    else {
      this.subscription.push(
        this.bankService.create(this.bank)
        .pipe(startWithTap(() => { this.isLoading = true }))
        .pipe(finalize(() => { this.isLoading = false }))
        .subscribe(rs => {
          abp.notify.success(`Created new bank ${this.bank.name}`)
          this.dialogRef.close(true)
        })
      )
    }
  }
}
