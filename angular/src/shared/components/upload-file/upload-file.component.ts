import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';


@Component({
  selector: 'app-upload-file',
  templateUrl: './upload-file.component.html',
  styleUrls: ['./upload-file.component.css']
})
export class UploadFileComponent implements OnInit {

  constructor() { }

  ngOnInit(): void {
  }
  @Output() selectFile = new EventEmitter();
  @Output() importFile = new EventEmitter();
  @Input() event: any;
  @Input() isDisable: boolean;

  public onSelectFile(event) {
    this.selectFile.emit();
  }

  public onImportFile(){
    this.importFile.emit();
  }

}
