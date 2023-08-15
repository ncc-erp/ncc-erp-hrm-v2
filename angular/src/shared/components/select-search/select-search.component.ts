import { Component, ElementRef, Input, OnChanges, OnInit ,Output,SimpleChanges, ViewChild,EventEmitter } from '@angular/core';
import { AbstractControl, FormControl } from '@angular/forms';
import { MatSelect } from '@angular/material/select';
import { ReplaySubject, Subject } from 'rxjs';
import { take, takeUntil } from 'rxjs/operators';

@Component({
  selector: 'app-select-search',
  templateUrl: './select-search.component.html',
  styleUrls: ['./select-search.component.css']
})
export class SelectSearchComponent implements OnInit, OnChanges {
  @Input() selectControl: AbstractControl
  @Input() list: any
  @Input() defaultValue: number
  @Input() searchLabel: string = ""
  @Output() selectionValue = new EventEmitter();
  @ViewChild('singleSelect') singleSelect: MatSelect;
  public searchControl: FormControl = new FormControl("")
  public filteredList: ReplaySubject<any> = new ReplaySubject<any>()
  public _onDestroy: Subject<void> = new Subject<void>()
  constructor(public _elementRef: ElementRef) {
   }

  ngOnInit(): void {
    if(this.list){
      this.filteredList.next(this.list)
    }
    this.searchControl.valueChanges
    .pipe(takeUntil(this._onDestroy))
    .subscribe(() => {
      this.filterList();
    });
  }
  ngOnChanges(changes: SimpleChanges): void {
    if(changes.list){
      this.list = changes.list.currentValue
      this.filteredList.next(this.list)
    }
  }
  ngOnDestroy() {
    this._onDestroy.next();
    this._onDestroy.complete();
  }
  filterList(){
    if(!this.list){
      return;
    }
    let search = this.searchControl.value;
    if (!search) {
      this.filteredList.next(this.list.slice());
      return;
    } else {
      search = search.toLowerCase();
    }
    // filter the banks
    this.filteredList.next(
      this.list.filter(item => item.key.toLowerCase().indexOf(search) > -1)
    );
  }
  changeValue(value){
    this.selectionValue.emit(value.value);
  }

}
