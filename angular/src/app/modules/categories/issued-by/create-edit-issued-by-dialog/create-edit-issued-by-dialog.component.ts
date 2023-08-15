import { Component, Injector, OnInit } from '@angular/core';
import { IssuedByService } from '@app/service/api/categories/issuedBy.service';
import { IssuedByDto } from '@app/service/model/categories/issuedBy.dto';
import { DialogComponentBase } from '@shared/dialog-component-base';
import { startWithTap } from '@shared/helpers/observerHelper';
import { finalize } from 'rxjs/operators';

@Component({
  selector: 'app-create-edit-issued-by-dialog',
  templateUrl: './create-edit-issued-by-dialog.component.html',
  styleUrls: ['./create-edit-issued-by-dialog.component.css']
})
export class CreateEditIssuedByDialogComponent extends DialogComponentBase<IssuedByDto> implements OnInit {

  public issuedBy = {} as IssuedByDto;

  constructor(injector: Injector , private issuedByService: IssuedByService) {
    super(injector)
  }

  ngOnInit(): void {
    if (this.dialogData?.id) {
      this.issuedBy = this.dialogData
      this.title = `Edit Issued By`;
    } else {
      this.title = 'Create new Issued By';
    }
  }

  nameChange(event) {
    if (!this.dialogData?.id) {
      this.issuedBy.name = event;
    }
  }

  saveAndClose() {
    this.trimData(this.issuedBy)
    if (this.dialogData?.id) {
      this.subscription.push(
        this.issuedByService.update(this.issuedBy)
        .pipe(startWithTap(() => { this.isLoading = true }))
        .pipe(finalize(() => { this.isLoading = false }))
        .subscribe(rs => {
          abp.notify.success(`Update issued by successful`)
          this.dialogRef.close(true)
        }))
    }
    else {
      this.subscription.push(
        this.issuedByService.create(this.issuedBy)
        .pipe(startWithTap(() => { this.isLoading = true }))
        .pipe(finalize(() => { this.isLoading = false }))
        .subscribe(rs => {
          abp.notify.success(`Created new issued by ${this.issuedBy.name}`)
          this.dialogRef.close(true)
        })
      )
    }
  }
}