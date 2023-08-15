import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'app-dialog-header',
  templateUrl: './dialog-header.component.html',
  styleUrls: ['./dialog-header.component.css']
})
export class DialogHeaderComponent implements OnInit {
@Input() dialogTitle:string;
  constructor() { }

  ngOnInit(): void {
  }

}
