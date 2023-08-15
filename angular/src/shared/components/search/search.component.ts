import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';

@Component({
  selector: 'app-search',
  templateUrl: './search.component.html',
  styleUrls: ['./search.component.css']
})
export class SearchComponent implements OnInit {
  @Input() seachText:string
  @Input() placeholder:string
  @Output() onSearch = new EventEmitter()
  constructor() { }

  ngOnInit(): void {
  }

  onSearchEnter(){
    this.onSearch.emit(this.seachText)
  }
  
  onClick(){
    this.onSearch.emit(this.seachText)
  }
}
