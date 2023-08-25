import { Component, EventEmitter, Input, OnChanges, OnInit, Output, SimpleChanges, ViewChild } from '@angular/core';
import { FormControl } from '@angular/forms';
import { MatSelect } from '@angular/material/select';
import { ReplaySubject, Subject } from 'rxjs';
import { take, takeUntil } from 'rxjs/operators'
@Component({
  selector: 'app-multi-select-box',
  templateUrl: './multi-select-box.component.html',
  styleUrls: ['./multi-select-box.component.css']
})
export class MultiSelectBoxComponent implements OnInit, OnChanges {
  @Input() list: any
  @Input() multiControl: FormControl
  @Input() searchPlaceholder: string
  @Input() isFilter:boolean
  @Input() label: string
  @ViewChild("multiSelect") multiSelect: MatSelect;
  @Output() onTableFilter: EventEmitter<MultiFilterData> = new EventEmitter()
  public readonly ESearchType = ESearchType
  public filterControl: FormControl = new FormControl()
  public searchType: number = ESearchType.AND
  public filteredItemSubject: ReplaySubject<any> = new ReplaySubject()
  protected _onDestroy = new Subject<void>();

  constructor() { }

  ngOnInit(): void {
    if (this.list) {
      this.filteredItemSubject.next(this.list)
    }
    this.filterControl.valueChanges.subscribe(value => {
      this.filterListMulti()
    })
    if (this.isFilter) {
      if (this.multiControl) {
        this.multiControl.valueChanges.subscribe(rs => {
          let controlOptions = this.multiControl.value.map(item => item.value)
          this.onTableFilter.emit({ options: controlOptions, searchType: this.searchType })
        })
      }
    }
  }
  ngOnChanges(changes: SimpleChanges): void {
    if (changes.list) {
      this.list = changes.list.currentValue
      this.filteredItemSubject.next(this.list)
    }

  }
  protected setInitialValue() {
    this.filteredItemSubject.pipe(
      take(1),
      takeUntil(this._onDestroy)
    ).subscribe(() => {
      this.multiSelect.compareWith = ((a, b) =>
        a && b && a.id === b.id)
    })
  }

  clearAll() {
    this.multiControl.reset()
    this.filteredItemSubject.next([...this.list]);
  }

  clearSelected(item, controlValue) {
    this.multiControl.value.splice(this.multiControl.value.indexOf(item), 1)
    let index = this.list.indexOf(item)
    let itemToinsert = { ...item }
    this.list.splice(this.list.indexOf(item), 1)
    this.list.splice(index, 0, itemToinsert)
    let search = this.filterControl.value;
    if (!search) {
      this.filteredItemSubject.next(this.list.slice());
      return;
    } else {
      search = search.toLowerCase();
    }
    this.filteredItemSubject.next(
      this.list.filter(item => item.key.toLowerCase().indexOf(search) > -1)
    );
  }

  filterListMulti() {
    if (!this.list) {
      return;
    }
    let search = this.filterControl.value
    if (!search) {
      this.filteredItemSubject.next(this.list.slice())
    } else {
      search = search.toLowerCase()
    }
    this.filteredItemSubject.next(this.list.filter(item => item.key.toLowerCase().indexOf(search) > -1))
  }

  onSearchTypeChange(event) {
    let controlOptions = this.multiControl.value.map(item => item.value)
    this.onTableFilter.emit({ options: controlOptions, searchType: this.searchType })
  }
  
  ngOnDestroy(){
    this._onDestroy.next()
  }
}

export enum ESearchType {
  AND = 0,
  OR = 1,
}
export interface MultiFilterData {
  options: number[],
  searchType: ESearchType
}