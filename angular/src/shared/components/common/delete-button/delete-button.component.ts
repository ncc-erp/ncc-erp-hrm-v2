import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';

@Component({
  selector: 'app-delete-button',
  templateUrl: './delete-button.component.html',
  styleUrls: ['./delete-button.component.css']
})
export class DeleteButtonComponent implements OnInit {
  @Output() delete = new EventEmitter();
  @Input() isDisable: boolean;
  @Input() item: any;
  constructor() { }

  ngOnInit(): void {
  }

  public onDelete(item) {
    this.delete.emit();
  }

}
