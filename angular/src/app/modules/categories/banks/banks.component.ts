import { Component, Injector, OnInit } from '@angular/core';
import { PERMISSIONS_CONSTANT } from '@app/permission/permission';
import { BankService } from '@app/service/api/categories/bank.service'
import { BankDto } from '@app/service/model/categories/bank.dto'
import { PagedListingComponentBase, PagedRequestDto } from '@shared/paged-listing-component-base';
import { finalize } from 'rxjs/operators';
import { CreateEditBankDialogComponent } from './create-edit-bank-dialog/create-edit-bank-dialog.component';
@Component({
  selector: 'app-banks',
  templateUrl: './banks.component.html',
  styleUrls: ['./banks.component.css']
})
export class BanksComponent extends PagedListingComponentBase<BankDto> implements OnInit {
  public bankList: BankDto[] = [];
  protected list(request: PagedRequestDto, pageNumber: number, finishedCallback: Function): void {
    this.subscription.push(
      this.bankService.getAllPagging(request).pipe(finalize(() => {
        finishedCallback()
      })).subscribe(rs => {
        this.bankList = rs.result.items
        this.showPaging(rs.result, pageNumber)
      })
    )
  }

  constructor(injector: Injector, private bankService: BankService) {
    super(injector);
  }

  ngOnInit(): void {
    this.listBreadCrumb = [
      {name: '<i class="fa-solid fa-house fa-sm"></i>',url:''}, 
      {name: ' <i class="fa-solid fa-chevron-right"></i> '}, 
      {name:'Banks'}],
    this.refresh()
  }

  isShowCreateBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Category_Bank_Create);
  }
  isShowEditBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Category_Bank_Edit);
  }
  isShowDeleteBtn(){
    return this.isGranted(PERMISSIONS_CONSTANT.Category_Bank_Delete);
  }

  onCreate() {
    this.openDialog(CreateEditBankDialogComponent)
  }

  onUpdate(bank: BankDto) {
    this.openDialog(CreateEditBankDialogComponent, { ...bank })
  }

  onDelete(bank: BankDto) {
    this.confirmDelete(`Delete bank <strong>${bank.name}</strong>`,
      () => this.bankService.delete(bank.id).toPromise().then(rs => {
        abp.notify.success(`Deleted bank ${bank.name}`)
      }))
  }
}
