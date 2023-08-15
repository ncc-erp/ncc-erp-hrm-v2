import { Component, Injector, OnInit } from '@angular/core';
import { EmployeeContractService } from '@app/service/api/employee/employeeContract.service';
import { UpdateContractNoteDto } from '@app/service/model/employee/UpdateContracNoteDto';
import { DialogComponentBase } from '@shared/dialog-component-base';
@Component({
  selector: 'app-employee-contract-edit-note',
  templateUrl: './employee-contract-edit-note.component.html',
  styleUrls: ['./employee-contract-edit-note.component.css']
})
export class EmployeeContractEditNoteComponent extends DialogComponentBase<DialogData> implements OnInit {
  public contractNote : UpdateContractNoteDto = {} as UpdateContractNoteDto
  constructor(
    injector: Injector,
    private employeeContractService: EmployeeContractService
  ) {
    super(injector)
  }

  ngOnInit(): void {
    if (this.dialogData) {
      this.contractNote.note = this.dialogData.note
      this.contractNote.contractId = this.dialogData.contractId
    }
  }

  onSave() {
    this.employeeContractService.updateNote(this.contractNote).subscribe(value => {
      this.notify.success("Contract note updated")
      this.dialogRef.close()
    })
  }

  onClose() {
    this.dialogRef.close()
  }
}

interface DialogData {
  code: string,
  contractId: number,
  note: string
}
