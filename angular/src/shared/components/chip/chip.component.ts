import { Component, OnInit, Input, Output, EventEmitter, OnChanges, SimpleChanges } from '@angular/core';
import {ENTER, COMMA} from '@angular/cdk/keycodes'
import { FormControl } from '@angular/forms';
import { MatChipInputEvent } from '@angular/material/chips';

@Component({
  selector: 'app-chip',
  templateUrl: './chip.component.html',
  styleUrls: ['./chip.component.css']
})
export class ChipComponent implements OnInit, OnChanges {
  @Input() label: string
  @Input() defaultData: any[]
  @Output() onValueChange: EventEmitter<any> = new EventEmitter<any>()
  list: any[] = []
  separatorKeysCodes: number[] = [ENTER, COMMA];
  itemInput: FormControl = new FormControl('')

  constructor() { }

  ngOnInit(): void {
    this.list = this.defaultData?.length ? [...this.defaultData] : []
  }

  ngOnChanges(changes: SimpleChanges): void {
    if('defaultData' in changes) {
      if(changes.defaultData.currentValue?.length) {
        this.list = [...changes.defaultData.currentValue]
      }
    }
  }

  add(event: MatChipInputEvent): void {
    const value = (event.value || '').trim();

    // Add our fruit
    if (value) {
      this.list.push(value);
    }

    // Clear the input value
    event.chipInput!.clear();

    this.itemInput.setValue(null);
    this.onValueChange.emit(this.list)
  }

  remove(fruit: string): void {
    const index = this.list.indexOf(fruit);

    if (index >= 0) {
      this.list.splice(index, 1);
    }
    this.onValueChange.emit(this.list)
  }

}
