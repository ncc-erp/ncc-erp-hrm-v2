import { Component, OnInit, Injector, Input } from '@angular/core';
import { SalaryChangeRequestService } from '@app/service/api/salary-change-request/salary-change-request.service';
import { MessageResponse } from '@app/service/model/common.dto';
import { ImportCheckpointDto } from '@app/service/model/salary-change-request/AddEmployeeToSalaryChangeRequestDto';
import { DialogComponentBase } from '@shared/dialog-component-base';

@Component({
  selector: 'app-import-checkpoint',
  templateUrl: './import-checkpoint.component.html',
  styleUrls: ['./import-checkpoint.component.css']
})
export class ImportCheckpointComponent extends DialogComponentBase<any> implements OnInit {

  public selectedFile: File;
  public changeRequestId: string = ""
  public results = {} as MessageResponse;
  constructor(injector: Injector, private changeRequestService: SalaryChangeRequestService) { super(injector) }

  ngOnInit(): void {
    this.changeRequestId = this.dialogData
  }

  public onSelectFile(e) {
    this.selectedFile = e.target.files.item(0);
  }

  public onCreateEmployeeFromFile() {
    this.isLoading = true;
    let input = new FormData()
    input.append("salaryChangeRequestId", this.changeRequestId)
    input.append("file", this.selectedFile);
    this.subscription.push(
      this.changeRequestService.ImportCheckpoint(input).subscribe((res) => {
        if (res) {
          this.results = res.result;
          abp.message.success(`<p class="successList" style='color:#28a745'>Success: <b>${this.results.successList.length}</b>  ${this.results.successList.length > 1 ? ' employees' : ' employee'}</p>
          <p style='color:red'>Failed: <b>${this.results.failedList.length}</b>${this.results.failedList.length > 1 ? ' employees' : ' employee'}</p>
          <div style = 'max-height: 500px !important ; overflow-y: auto; overflow-x: hidden'>${this.getFailMessage(this.results.failedList)}</div>
        `,
            'Import checkpoint', true);
          this.isLoading = false;
          this.dialogRef.close(this.results);
        }
      }, () => this.isLoading = false)
    )
  }

  public getFailMessage(failedList) {

    let failMessage = ""
    failedList.forEach(mess => {
      failMessage += `
      <tr>
        <td class='row-index'>${mess.row}</td>
        <td class='text-center'>${mess.email}</td>
        <td>${mess.reasonFail}</td>
      </tr>
      `
    });

    let messages = failedList.length > 0 ?
  `<table class='w-100'>
      <thead>
        <tr>
            <th>Row</th>
            <th>Email</th>
            <th>Error</th>
        </tr>
      </thead>

      <tbody>
        ${failMessage}
      </tbody>
    </table>
  `
      : ""
    return messages;
  }
}
