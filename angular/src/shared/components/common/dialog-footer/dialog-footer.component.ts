import { Component, OnInit, Output, EventEmitter, Input } from '@angular/core';

@Component({
  selector: 'app-dialog-footer',
  templateUrl: './dialog-footer.component.html',
  styleUrls: ['./dialog-footer.component.css']
})
export class DialogFooterComponent implements OnInit {
  @Output() onSave = new EventEmitter()
  @Input() isDisable: boolean
  constructor() { }

  ngOnInit(): void {
  }

  onSaveClick() {
    this.onSave.emit()
  }
}
