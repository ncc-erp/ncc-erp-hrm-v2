import { Component, EventEmitter, Input, OnInit, Output, Injector, ChangeDetectionStrategy } from '@angular/core';
import { APP_ENUMS } from '@shared/AppEnums';

@Component({
  selector: 'pagination-control',
  templateUrl: './pagination-control.component.html',
  styleUrls: ['./pagination-control.component.css'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class PaginationControlComponent {
  public pagesizeOption: number[] = [5, 10, 20, 50, 100, 500]
  private pagingAction = APP_ENUMS.PagingActionEnum
  @Input() pageSize: number = 0
  @Input() totalItems: number = 0
  @Input() id?:string
  @Output() pageAction = new EventEmitter()

  public onPageSelect(pageNumber: number) {
    this.pageAction.emit({ action: this.pagingAction.PAGE_CHANGE, value: pageNumber })
  }

  public onPageSizeSelect() {
    this.pageAction.emit({ action: this.pagingAction.PAGE_SIZE_CHANGE, value: this.pageSize })
  }

  public onRefresh() {
    this.pageAction.emit({ action: this.pagingAction.REFRESH })
  }
}
