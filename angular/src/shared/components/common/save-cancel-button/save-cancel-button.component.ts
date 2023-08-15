import { Component, OnInit, Output, EventEmitter, Input, OnChanges, SimpleChanges, ChangeDetectionStrategy } from '@angular/core';
import { isNull } from 'lodash-es';

@Component({
  selector: 'app-save-cancel-button',
  templateUrl: './save-cancel-button.component.html',
  styleUrls: ['./save-cancel-button.component.css'],
  changeDetection: ChangeDetectionStrategy.Default
})
export class SaveCancelButtonComponent implements OnInit, OnChanges {
  @Output() save = new EventEmitter();
  @Output() cancel = new EventEmitter();
  @Input() item: any;
  @Input() isDisable: boolean;
  constructor() { }
  ngOnChanges(changes: SimpleChanges): void {
    if('isDisable' in changes){
      if(!isNull(changes.isDisable.currentValue)){
        this.isDisable = changes.isDisable.currentValue
      }
    }
  }

  ngOnInit(): void {
  }

  onSave(item){
    this.save.emit();
  }

  onCancel(item){
    this.cancel.emit();
  }

}
