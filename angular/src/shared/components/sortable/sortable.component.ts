import { APP_ENUMS } from './../../AppEnums';
import { AppComponentBase } from '@shared/app-component-base';
import { Component, EventEmitter, Injector, Input, OnInit, Output } from '@angular/core';

@Component({
  selector: 'sortable',
  templateUrl: './sortable.component.html',
  styleUrls: ['./sortable.component.css']
})
export class SortableComponent {
  @Input() sortProperty: string = "";
  @Input() sortDirection: number
  @Input() name: string = ""

  sortDirectionEnum = APP_ENUMS.SortDirectionEnum
}


