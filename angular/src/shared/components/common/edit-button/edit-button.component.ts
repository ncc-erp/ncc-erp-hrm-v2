import { Component, Input, OnInit, Output ,EventEmitter} from '@angular/core';
@Component({
  selector: 'app-edit-button',
  templateUrl: './edit-button.component.html',
  styleUrls: ['./edit-button.component.css']
})
export class EditButtonComponent implements OnInit {
  @Output() update = new EventEmitter();
  @Input() isDisable: boolean;
  // @Input() item: any;
    constructor() { }
  
    ngOnInit(): void {
    }
  
    public onUpdate(){
      this.update.emit();
    }
}
