import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';

@Component({
  selector: 'app-edit-delete-button',
  templateUrl: './edit-delete-button.component.html',
  styleUrls: ['./edit-delete-button.component.css']
})
export class EditDeleteButtonComponent implements OnInit {
@Output() update = new EventEmitter();
@Output() delete = new EventEmitter();
@Input() isDisable: boolean;
@Input() item: any;
  constructor() { }

  ngOnInit(): void {
  }

  public onUpdate(item){
    this.update.emit();
  }

  public onDelete(item){
    this.delete.emit();
  }

}
