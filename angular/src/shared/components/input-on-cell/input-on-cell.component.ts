import { Component, Injector, Input, OnInit } from '@angular/core';
import { FormControl } from '@angular/forms';
import { AppComponentBase } from '@shared/app-component-base';
import { SelectOptionDto } from '@shared/dto/selectOptionDto';

@Component({
  selector: 'app-input-on-cell',
  templateUrl: './input-on-cell.component.html',
  styleUrls: ['./input-on-cell.component.css']
})
export class InputOnCellComponent extends AppComponentBase implements OnInit {

  constructor(inject : Injector) {
    super(inject);
  }

  @Input() isEdit : boolean
  @Input() hasButton : boolean = false
  @Input() list : SelectOptionDto[]
  @Input('form-control') formControl : FormControl

  ngOnInit(): void {
  }

  getKeyOption(){
    return this.list.find(x => x.value == this.formControl.value)?.key
  }
}
