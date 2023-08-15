import { Component, OnInit, Output, EventEmitter } from '@angular/core';

@Component({
  selector: 'app-create-button',
  templateUrl: './create-button.component.html',
  styleUrls: ['./create-button.component.css']
})
export class CreateButtonComponent implements OnInit {
  @Output() create = new EventEmitter()
  constructor() { }

  ngOnInit(): void {
  }

  onCreate(){
    this.create.emit();
  }

}
